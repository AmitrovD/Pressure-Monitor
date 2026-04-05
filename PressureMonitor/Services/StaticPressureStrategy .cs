using OxyPlot;

namespace PressureMonitor.Services
{
    public class StaticPressureStrategy : IGenerationStrategy
    {
        public DataPoint Generate(int time, double limit)
        {
            return new DataPoint(time, limit);
        }
    }
}
