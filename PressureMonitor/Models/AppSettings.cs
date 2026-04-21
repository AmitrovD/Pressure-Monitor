namespace PressureMonitor.Models
{
    public class AppSettings
    {
        public GenerationType SelectedType { get; set; } = GenerationType.Static;
        public double? StaticValue { get; set; } = 25.5;
        public double? RandomValue { get; set; } = 250;
        public double? LinearStartValue { get; set; } = 0;
        public double? LinearStep { get; set; } = 1;
        public PressureDirections SelectedDirection { get; set; } = PressureDirections.Up;
        public ModbusSettings ModbusSettings { get; set; } = new ModbusSettings();
    }
}
