using OxyPlot;

namespace PressureMonitor.Services
{
    public interface IGenerationStrategy
    {
        DataPoint Generate(int time, double limit);
    }
}
