using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
