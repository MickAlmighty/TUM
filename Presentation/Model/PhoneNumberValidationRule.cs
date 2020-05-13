using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Presentation.Model
{
    class PhoneNumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string strValue = Convert.ToString(value);
            if (string.IsNullOrEmpty(strValue))
            {
                return new ValidationResult(false, null);
            }
            if (!Regex.IsMatch(strValue, @"^((\+[0-9]{1,3}\ )?[0-9]{3}\ [0-9]{3}\ [0-9]{3,4})|((\+[0-9]{1,3}-)?[0-9]{3}-[0-9]{3}-[0-9]{3,4})$"))
            {
                return new ValidationResult(false, "Invalid phone number format");
            }
            return new ValidationResult(true, null);
        }
    }
}
