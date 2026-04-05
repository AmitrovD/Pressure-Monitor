using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressureMonitor.Models
{
    public class AppSettings
    {
        public GenerationType SelectedType { get; set; }
        public double? StaticValue { get; set; }
        public double? RandomValue { get; set; }
        public double? LinearStartValue { get; set; }
        public double? LinearStep {  get; set; }
        public PressureDirections SelectedDirection { get; set; }
    }
}
