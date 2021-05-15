using System;
using System.Globalization;
using System.Windows.Controls;

namespace Presentation.View
{
    class BasicStringValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string strValue = Convert.ToString(value);
            if (string.IsNullOrEmpty(strValue))
            {
                return new ValidationResult(false, null);
            }
            return new ValidationResult(true, null);
        }
    }
}
