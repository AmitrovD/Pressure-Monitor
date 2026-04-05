using OxyPlot;
using System;
namespace PressureMonitor.Models
{
    public class Trial
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public List<TrialPoint> Points { get; set; } = new();
    }
}
