using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressureMonitor.Services
{
    public interface IGenerationStrategy
    {
        DataPoint Generate(int time, double limit);
    }
}
