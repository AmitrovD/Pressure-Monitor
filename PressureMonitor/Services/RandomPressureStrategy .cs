using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
