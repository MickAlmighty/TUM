using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data
{
    public interface IDataRepository : IObservable<OrderSent>, IObservable<DataChanged<IClient>>, IObservable<DataChanged<IProduct>>, IObservable<DataChanged<IOrder>>
    {
        Task<bool> OpenRepository(string openParam);
        Task<HashSet<IClient>> GetAllClients();
        Task<HashSet<IOrder>> GetAllOrders();
        Task<HashSet<IProduct>> GetAllProducts();
        Task<IClient> CreateClient(string username, string firstName, string lastName, string street, uint streetNumber, string phoneNumber);
        Task<IOrder> CreateOrder(string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap, DateTime? deliveryDate);
        Task<IProduct> CreateProduct(string name, double price, ProductType productType);
        Task<IClient> GetClient(string username);
        Task<IOrder> GetOrder(uint id);
        Task<IProduct> GetProduct(uint id);
        Task<bool> Update(IClient client);
        Task<bool> Update(IOrder order);
        Task<bool> UpdateClient(string username, string firstName, string lastName, string street, uint streetNumber, string phoneNumber);
        Task<bool> UpdateOrder(uint id, string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap, double price, DateTime? deliveryDate);
        Task<bool> UpdateProduct(uint id, string name, double price, ProductType productType);
        Task<bool> Update(IProduct product);
        Task<bool> RemoveClient(IClient client);
        Task<bool> RemoveOrder(IOrder order);
        Task<bool> RemoveProduct(IProduct product);
    }
}
