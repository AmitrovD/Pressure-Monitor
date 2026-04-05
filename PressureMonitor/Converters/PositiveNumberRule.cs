using System.Globalization;
using System.Windows.Controls;

namespace PressureMonitor.Converters
{
    public class PositiveNumberRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return new ValidationResult(false, "Поле не заполнено");

            if (!double.TryParse(value.ToString(), out double result))
                return new ValidationResult(false, "Введите число");

            if (result < 0)
                return new ValidationResult(false, "Число должно быть >= 0");

            return ValidationResult.ValidResult;
        }
    }
}
