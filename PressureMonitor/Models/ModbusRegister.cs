using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressureMonitor.Models
{
    public class ModbusRegister
    {
        public string Name { get; set; } = "";
        public ushort Address { get; set; }
        public RegisterType Type { get; set; }
        public string Value { get; set; } = "";
    }
}
