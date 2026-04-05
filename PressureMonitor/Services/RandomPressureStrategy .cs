using OxyPlot;

namespace PressureMonitor.Services
{
    public class RandomPressureStrategy : IGenerationStrategy
    {
        public DataPoint Generate(int time, double limit)
        {
            var random = new Random();
            return new DataPoint(time, random.NextDouble()*limit);
        }
    }
}
