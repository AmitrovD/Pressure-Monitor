using Microsoft.Extensions.Logging;
using NModbus;
using PressureMonitor.Helpers;
using PressureMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PressureMonitor.Services
{
    public class ModbusService
    {
        private TcpClient _tcpClient;
        private IModbusMaster _master;
        private ModbusSettings _settings;
        private CancellationTokenSource _cts;
        private bool _isRunning = false;
        private int _reconnectAttempts = 0;

        public event Action<double> OnValueReceived;
        public event Action<ConnectionStatus> OnStatusChanged;

        private readonly ILogger<ModbusService> _logger
            = AppLoggerFactory.CreateLogger<ModbusService>();

        public ModbusService()
        {
            _settings = new ModbusSettings();
        }

        public void SetSettings(ModbusSettings settings)
        {
            _settings = settings;
        }

        private bool TryConnect()
        {
            try
            {
                if (_tcpClient != null)
                {
                    _tcpClient.Close();
                    _tcpClient = null;
                }

                _logger.LogInformation("Подключение к {Ip}:{Port}",
                _settings.IpAddress, _settings.Port);

                _tcpClient = new TcpClient();
                var connected = _tcpClient.ConnectAsync(_settings.IpAddress, _settings.Port)
                    .Wait(3000);

                if (!connected)
                {
                    OnStatusChanged?.Invoke(ConnectionStatus.Timeout);
                    return false;
                }

                var factory = new ModbusFactory();
                _master = factory.CreateMaster(_tcpClient);
                _master.Transport.ReadTimeout = 2000;
                _master.Transport.WriteTimeout = 2000;

                _reconnectAttempts = 0;
                OnStatusChanged?.Invoke(ConnectionStatus.Connected);

                _logger.LogInformation("Успешно подключено");
                return true;
            }
            catch (Exception ex)
            {
                OnStatusChanged?.Invoke(ConnectionStatus.Error);
                _logger.LogError(ex, "Ошибка подключения к {Ip}:{Port}",
                _settings.IpAddress, _settings.Port);
                return false;
            }
        }
        public async Task StartAsync(ushort registerAddress)
        {
            if (_isRunning)
                return;

            _isRunning = true;
            _cts = new CancellationTokenSource();

            if (!TryConnect())
            {
                _logger.LogInformation("Не удалось подключиться к устройству!");
            }

            await Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        if (_tcpClient == null || !_tcpClient.Connected)
                        {
                            OnStatusChanged?.Invoke(ConnectionStatus.Disconnected);
                            _reconnectAttempts++;
                            Console.WriteLine($"Попытка переподключения #{_reconnectAttempts}");

                            bool reconnected = TryConnect();
                            if (!reconnected)
                            {
                                await Task.Delay(3000, _cts.Token);
                                continue;
                            }
                        }

                        var registers = _master.ReadHoldingRegisters(
                            _settings.SlaveId,
                            registerAddress,
                            2);

                        double value = ConvertRegistersToFloat(registers);
                        OnValueReceived?.Invoke(value);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка чтения: " + ex.Message);
                        OnStatusChanged?.Invoke(ConnectionStatus.Error);
                        _tcpClient = null;
                    }

                    await Task.Delay(_settings.PollingInterval, _cts.Token);
                }
            }, _cts.Token);
        }
        private float ConvertRegistersToFloat(ushort[] registers)
        {
            if (registers.Length < 2)
                return 0f;

            byte[] bytes = new byte[4];
            bytes[0] = (byte)(registers[0] & 0xFF);
            bytes[1] = (byte)(registers[0] >> 8);
            bytes[2] = (byte)(registers[1] & 0xFF);
            bytes[3] = (byte)(registers[1] >> 8);

            return BitConverter.ToSingle(bytes, 0);
        }
        public void Stop()
        {
            _isRunning = false;
            _cts?.Cancel();

            if (_tcpClient != null)
            {
                _tcpClient.Close();
                _tcpClient = null;
            }

            OnStatusChanged?.Invoke(ConnectionStatus.Disconnected);
        }
    }
}
