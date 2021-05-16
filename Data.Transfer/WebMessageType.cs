namespace Data.Transfer
{
    public enum WebMessageType
    {
        AddClient,
        AddProduct,
        AddOrder,
        GetClient,
        GetProduct,
        GetOrder,
        RemoveClient,
        RemoveProduct,
        RemoveOrder,
        UpdateClient,
        UpdateProduct,
        UpdateOrder,
        ProvideClient,
        ProvideProduct,
        ProvideOrder,
        ProvideAllClients,
        ProvideAllProducts,
        ProvideAllOrders,
        OrderSent,
        Error
    }
}
