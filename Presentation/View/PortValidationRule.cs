using System;
using System.Globalization;
using System.Windows.Controls;

namespace Presentation.View
{
    internal class PortValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string strValue = Convert.ToString(value);
            if (string.IsNullOrEmpty(strValue))
            {
                return new ValidationResult(false, null);
            }
            if (uint.TryParse(strValue, out uint result))
            {
                if (result < 1024U || result > 49151U)
                {
                    return new ValidationResult(false, "The port must be withing range [1024;49151]");
                }
                return new ValidationResult(true, null);
            }
            return new ValidationResult(false, "Value is not an integer");
        }
    }
}