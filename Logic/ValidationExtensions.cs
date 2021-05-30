using Data;

namespace Logic
{
    public static class ValidationExtensions
    {
        public static bool IsTrimmedNonEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str?.Trim());
        }

        public static bool IsValid(this IClient client)
        {
            return DataValidationUtil.IsUsernameValid(client.Username) &&
                   DataValidationUtil.IsFirstNameValid(client.FirstName) &&
                   DataValidationUtil.IsLastNameValid(client.LastName) &&
                   DataValidationUtil.IsStreetValid(client.Street) &&
                   DataValidationUtil.IsStreetNumberValid(client.StreetNumber) &&
                   DataValidationUtil.IsPhoneNumberValid(client.PhoneNumber);
        }

        public static bool IsValid(this IOrder order)
        {
            return DataValidationUtil.IsUsernameValid(order.ClientUsername) &&
                   DataValidationUtil.IsProductIdQuantityMapValid(order.ProductIdQuantityMap) &&
                   DataValidationUtil.IsPriceValid(order.Price) &&
                   (!order.DeliveryDate.HasValue || (order.DeliveryDate >= order.OrderDate));
        }

        public static bool IsValid(this IProduct product)
        {
            return product.Name.IsTrimmedNonEmpty() &&
                   DataValidationUtil.IsPriceValid(product.Price);
        }
    }
}
