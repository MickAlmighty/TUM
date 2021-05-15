using System;
using System.Globalization;
using System.Windows.Controls;

namespace Presentation.View
{
    class PositiveIntegerValidationRule : ValidationRule
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
                if (result == 0U)
                {
                    return new ValidationResult(false, "Value must be positive");
                }
                return new ValidationResult(true, null);
            }
            return new ValidationResult(false, "Value is not a non-negative integer");
        }
    }
}
