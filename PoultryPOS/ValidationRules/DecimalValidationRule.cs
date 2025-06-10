using System.Globalization;
using System.Windows.Controls;

namespace PoultryPOS.Views
{
    public class DecimalValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return new ValidationResult(true, null);

            if (decimal.TryParse(value.ToString(), out decimal result) && result >= 0)
                return new ValidationResult(true, null);

            return new ValidationResult(false, "Please enter a valid decimal value.");
        }
    }
}