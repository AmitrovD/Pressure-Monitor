using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressureMonitor.Models
{
    public class ModbusSettings
    {
        public string IpAddress { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 502;
        public byte SlaveId { get; set; } = 1;
        public int PollingInterval { get; set; } = 1000;
    }
}
