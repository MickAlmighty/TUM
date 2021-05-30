using System;
using System.Globalization;
using System.Windows.Controls;

using Logic;

namespace Presentation.View
{
    internal class BasicStringValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string strValue = Convert.ToString(value);
            if (!strValue.IsTrimmedNonEmpty())
            {
                return new ValidationResult(false, null);
            }
            return new ValidationResult(true, null);
        }
    }
}
