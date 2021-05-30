using System;
using System.Globalization;
using System.Windows.Controls;

using Logic;

namespace Presentation.View
{
    internal class PhoneNumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string strValue = Convert.ToString(value);
            if (string.IsNullOrEmpty(strValue))
            {
                return new ValidationResult(false, null);
            }
            if (!DataValidationUtil.IsPhoneNumberValid(strValue))
            {
                return new ValidationResult(false, "Invalid phone number format");
            }
            return new ValidationResult(true, null);
        }
    }
}
