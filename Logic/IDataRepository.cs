using Data;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logic
{
    public interface IDataRepository : IObservable<OrderSent>, IObservable<DataChanged<Client>>, IObservable<DataChanged<Product>>, IObservable<DataChanged<Order>>
    {
        Task<bool> OpenRepository();
        Task<HashSet<Client>> GetAllClients();
        Task<HashSet<Order>> GetAllOrders();
        Task<HashSet<Product>> GetAllProducts();
        Task<bool> CreateClient(string username, string firstName, string lastName, string street, uint streetNumber, string phoneNumber);
        Task<bool> CreateOrder(string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap, DateTime? deliveryDate);
        Task<bool> CreateProduct(string name, double price, ProductType productType);
        Task<Client> GetClient(string username);
        Task<Order> GetOrder(uint id);
        Task<Product> GetProduct(uint id);
        Task<bool> Update(Client client);
        Task<bool> Update(Order order);
        Task<bool> Update(Product product);
        Task<bool> RemoveClient(Client client);
        Task<bool> RemoveOrder(Order order);
        Task<bool> RemoveProduct(Product product);
    }
}
