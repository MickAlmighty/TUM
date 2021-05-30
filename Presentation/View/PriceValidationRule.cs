using System;
using System.Globalization;
using System.Windows.Controls;

using Logic;

namespace Presentation.View
{
    internal class PriceValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string strValue = Convert.ToString(value);
            if (string.IsNullOrEmpty(strValue))
            {
                return new ValidationResult(false, null);
            }
            if (double.TryParse(strValue, out double price))
            {
                if (!DataValidationUtil.IsPriceValid(price))
                {
                    return new ValidationResult(false, "Price must be positive");
                }
                return new ValidationResult(true, null);
            }
            return new ValidationResult(false, "Invalid number format");
        }
    }
}
