using OxyPlot;
using PressureMonitor.Models;

namespace PressureMonitor.Services
{
    public class LinearPressureStrategy : IGenerationStrategy
    {
        private double _current;
        private readonly double _step;
        private readonly PressureDirections _direction;

        public LinearPressureStrategy(double startValue, double step, PressureDirections direction)
        {
            _current = startValue;
            _step = step;
            _direction = direction;
        }
        public DataPoint Generate(int time, double limit)
        {
            if (_direction == PressureDirections.Up)
                _current += _step;
            else 
                _current -= _step;

            return new DataPoint(time, _current);
        }
    }
}
