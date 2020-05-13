using System;
using System.Globalization;
using System.Windows.Controls;

namespace Presentation.Model
{
    class UsernameValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string strValue = Convert.ToString(value);
            if (string.IsNullOrEmpty(strValue))
            {
                return new ValidationResult(false, null);
            }
            if (strValue.Length < 3)
            {
                return new ValidationResult(false, "Username is too short");
            }
            return new ValidationResult(true, null);
        }
    }
}
