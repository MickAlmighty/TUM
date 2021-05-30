using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Logic
{
    public static class DataValidationUtil
    {
        public static uint MinUsernameLength { get; } = 3;
        public static string PhoneNumberRegex { get; } = @"^((\+[0-9]{1,3}\ )?[0-9]{3}\ [0-9]{3}\ [0-9]{3,4})|((\+[0-9]{1,3}-)?[0-9]{3}-[0-9]{3}-[0-9]{3,4})$";

        public static bool IsUsernameValid(string username)
        {
            return username.IsTrimmedNonEmpty() && username.Trim().Length >= MinUsernameLength;
        }

        public static bool IsFirstNameValid(string firstName)
        {
            return firstName.IsTrimmedNonEmpty();
        }

        public static bool IsLastNameValid(string lastName)
        {
            return lastName.IsTrimmedNonEmpty();
        }

        public static bool IsStreetValid(string street)
        {
            return street.IsTrimmedNonEmpty();
        }

        public static bool IsStreetNumberValid(uint streetNumber)
        {
            return streetNumber != 0U;
        }

        public static bool IsPhoneNumberValid(string phoneNumber)
        {
            return phoneNumber.IsTrimmedNonEmpty() && Regex.IsMatch(phoneNumber.Trim(), PhoneNumberRegex);
        }

        public static bool IsProductIdQuantityMapValid(Dictionary<uint, uint> productIdQuantityMap)
        {
            if (productIdQuantityMap == null)
            {
                return false;
            }
            foreach (uint key in productIdQuantityMap.Where(pair => pair.Value == 0U).Select(pair => pair.Key).ToList())
            {
                productIdQuantityMap.Remove(key);
            }

            return productIdQuantityMap.Any();
        }

        public static bool IsPriceValid(double price)
        {
            return price > 0.0;
        }
    }
}
