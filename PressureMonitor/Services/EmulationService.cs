using OxyPlot;
using PressureMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PressureMonitor.Services
{
    public class EmulationService
    {
        private DispatcherTimer _timer = new();
        private Random random = new Random();
        private DataPoint dataPoint = new DataPoint();
        private DateTime _startTime;
        private double limit;
        public IGenerationStrategy _strategy = new StaticPressureStrategy();
        public void StartGeneration(double limit)
        {
            this.limit = limit;

            _startTime = DateTime.Now;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTick;
            _timer.Start();
        }
        public void SetStrategy(IGenerationStrategy strategy)
        {
            _strategy = strategy;
        }
        private void OnTick(object? sender, EventArgs e)
        {
            if (_strategy == null) return;

            int time = (int)(DateTime.Now - _startTime).TotalSeconds;
            var point = _strategy.Generate(time, limit);

            OnDataGenerated?.Invoke(point);
        }
        public void StopGeneration()
        { 
            _timer.Stop(); 
        }
        public event Action<DataPoint>? OnDataGenerated;

    }
}
