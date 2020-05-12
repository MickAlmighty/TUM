using Data;
using System;
using System.Collections.Generic;

namespace Logic
{
    public interface IDataRepository
    {
        HashSet<Client> GetAllClients();
        HashSet<Order> GetAllOrders();
        HashSet<Product> GetAllProducts();
        bool CreateClient(string username, string firstName, string lastName, string street, uint streetNumber, string phoneNumber);
        bool CreateOrder(string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap);
        bool CreateProduct(string name, double price, ProductType productType);
        Client GetClient(string username);
        Order GetOrder(uint id);
        Product GetProduct(uint id);
        bool Update(Client client);
        bool Update(Order order);
        bool Update(Product product);
        bool RemoveClient(string username);
        bool RemoveOrder(uint id);
        bool RemoveProduct(uint id);
    }
}
