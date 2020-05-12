using Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    public class FileRepository : IDataRepository
    {
        protected object ClientLock
        {
            get;
        } = new object();

        protected object OrderLock
        {
            get;
        } = new object();

        protected object ProductLock
        {
            get;
        } = new object();

        protected ClientManager ClientManager
        {
            get;
        } = new ClientManager();

        protected OrderManager OrderManager
        {
            get;
        } = new OrderManager();

        protected ProductManager ProductManager
        {
            get;
        } = new ProductManager();

        public bool CreateClient(string username, string firstName, string lastName, string street, uint streetNumber, string phoneNumber)
        {
            lock (ClientLock)
            {
                return ClientManager.Create(username, firstName, lastName, street, streetNumber, phoneNumber);
            }
        }

        public bool CreateOrder(string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap)
        {
            lock (ClientLock)
            {
                if (ClientManager.Get(clientUsername) == null)
                {
                    return false;
                }
                lock (ProductLock)
                {
                    double totalPrice = 0.0;
                    foreach (KeyValuePair<uint, uint> pair in productIdQuantityMap)
                    {
                        Product product = ProductManager.Get(pair.Key);
                        if (product == null)
                        {
                            return false;
                        }
                        totalPrice += product.Price * pair.Value;
                    }
                    lock (OrderLock)
                    {
                        return OrderManager.Create(clientUsername, orderDate, productIdQuantityMap, totalPrice, null);
                    }
                }
            }
        }

        public bool CreateProduct(string name, double price, ProductType productType)
        {
            lock (ProductLock)
            {
                return ProductManager.Create(name, price, productType);
            }
        }

        public HashSet<Client> GetAllClients()
        {
            lock (ClientLock)
            {
                return ClientManager.GetAll();
            }
        }

        public HashSet<Order> GetAllOrders()
        {
            lock (OrderLock)
            {
                return OrderManager.GetAll();
            }
        }

        public HashSet<Product> GetAllProducts()
        {
            lock (ProductLock)
            {
                return ProductManager.GetAll();
            }
        }

        public Client GetClient(string username)
        {
            lock (ClientLock)
            {
                return ClientManager.Get(username);
            }
        }

        public Order GetOrder(uint id)
        {
            lock (OrderLock)
            {
                return OrderManager.Get(id);
            }
        }

        public Product GetProduct(uint id)
        {
            lock (ProductLock)
            {
                return ProductManager.Get(id);
            }
        }

        public bool RemoveClient(string username)
        {
            lock (OrderLock)
            {
                lock (ClientLock)
                {
                    if (ClientManager.Get(username) == null)
                    {
                        return false;
                    }
                    foreach (uint orderId in OrderManager.GetAll().Where(order => order.ClientUsername == username).Select(o => o.Id))
                    {
                        OrderManager.Remove(orderId);
                    }
                    return ClientManager.Remove(username);
                }
            }
        }

        public bool RemoveOrder(uint id)
        {
            lock (OrderLock)
            {
                return OrderManager.Remove(id);
            }
        }

        public bool RemoveProduct(uint id)
        {
            lock (OrderLock)
            {
                lock (ProductLock)
                {
                    if (ProductManager.Get(id) == null)
                    {
                        return false;
                    }
                    foreach (Order order in OrderManager.GetAll())
                    {
                        if (order.ProductIdQuantityMap.ContainsKey(id))
                        {
                            return false;
                        }
                    }
                    return ProductManager.Remove(id);
                }
            }
        }

        public bool Update(Client client)
        {
            lock (OrderLock)
            {
                lock (ClientLock)
                {
                    return ClientManager.Update(client);
                }
            }
        }

        public bool Update(Order order)
        {
            lock (ClientLock)
            {
                if (ClientManager.Get(order.ClientUsername) == null)
                {
                    return false;
                }
                lock (ProductLock)
                {
                    double totalPrice = 0.0;
                    foreach (KeyValuePair<uint, uint> pair in order.ProductIdQuantityMap)
                    {
                        Product product = ProductManager.Get(pair.Key);
                        if (product == null)
                        {
                            return false;
                        }
                        totalPrice += product.Price * pair.Value;
                    }
                    order.Price = totalPrice;
                    lock (OrderLock)
                    {
                        return OrderManager.Update(order);
                    }
                }
            }
        }

        public bool Update(Product product)
        {
            lock (ProductLock)
            {
                lock (OrderLock)
                {
                    return ProductManager.Update(product);
                }
            }
        }
    }
}
