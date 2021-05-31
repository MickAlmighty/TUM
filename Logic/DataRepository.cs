using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Data;

using DataModel;

namespace Logic
{
    public sealed class DataRepository : IDisposable, IDataRepository, IObserver<DataChanged<IOrder>>
    {
        public DataRepository(bool fillSampleData = true)
        {
            OrderUnsubscriber = OrderManager.Subscribe(this);
            if (fillSampleData)
            {
                SampleDataFiller.FillData(ClientManager, ProductManager, OrderManager);
            }
        }

        private IDisposable OrderUnsubscriber { get; }

        public void OnCompleted() { }

        public void OnError(Exception error)
        {
            Console.WriteLine($"Order subscription error: {error}.");
        }

        public void OnNext(DataChanged<IOrder> value)
        {
            if (value.Action == DataChangedAction.Add)
            {
                List<uint> newOrders = value.NewItems.Select(o => o.Id).ToList();
                Task.Run(async delegate
                {
                    await Task.Delay(5000);
                    lock (OrderLock)
                    {
                        foreach (uint id in newOrders)
                        {
                            IOrder order = OrderManager.Get(id);
                            if (order != null && !order.DeliveryDate.HasValue)
                            {
                                order.DeliveryDate = DateTime.Now;
                                foreach (IObserver<OrderSent> observer in OrderSentObservers.ToList())
                                {
                                    observer.OnNext(new OrderSent(order));
                                }
                                Update(order).GetAwaiter().GetResult();
                            }
                        }
                    }
                });
            }
        }

        private object ClientLock
        {
            get;
        } = new object();

        private object OrderLock
        {
            get;
        } = new object();

        private object ProductLock
        {
            get;
        } = new object();

        private ClientManager ClientManager
        {
            get;
        } = new ClientManager();

        private OrderManager OrderManager
        {
            get;
        } = new OrderManager();

        private ProductManager ProductManager
        {
            get;
        } = new ProductManager();

        public Task<bool> OpenRepository(string openParam)
        {
            return Task.FromResult(true);
        }

        public Task<HashSet<IClient>> GetAllClients()
        {
            lock (ClientLock)
            {
                return Task.FromResult(ClientManager.GetAll());
            }
        }

        public Task<HashSet<IOrder>> GetAllOrders()
        {
            lock (OrderLock)
            {
                return Task.FromResult(OrderManager.GetAll());
            }
        }

        public Task<HashSet<IProduct>> GetAllProducts()
        {
            lock (ProductLock)
            {
                return Task.FromResult(ProductManager.GetAll());
            }
        }

        public Task<IClient> GetClient(string username)
        {
            lock (ClientLock)
            {
                return Task.FromResult(ClientManager.Get(username));
            }
        }

        public Task<IOrder> GetOrder(uint id)
        {
            lock (OrderLock)
            {
                return Task.FromResult(OrderManager.Get(id));
            }
        }

        public Task<IProduct> GetProduct(uint id)
        {
            lock (ProductLock)
            {
                return Task.FromResult(ProductManager.Get(id));
            }
        }

        public Task<IClient> CreateClient(string username, string firstName, string lastName, string street, uint streetNumber,
            string phoneNumber)
        {
            lock (ClientLock)
            {
                try
                {
                    return Task.FromResult(ClientManager.Get(ClientManager.Create(username, firstName, lastName, street, streetNumber, phoneNumber)));
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public Task<IOrder> CreateOrder(string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap, DateTime? deliveryDate)
        {
            lock (ClientLock)
            {
                if (ClientManager.Get(clientUsername) == null)
                {
                    return null;
                }
                lock (ProductLock)
                {
                    double totalPrice = 0.0;
                    foreach (KeyValuePair<uint, uint> pair in productIdQuantityMap)
                    {
                        IProduct product = ProductManager.Get(pair.Key);
                        if (product == null)
                        {
                            return null;
                        }
                        totalPrice += product.Price * pair.Value;
                    }
                    lock (OrderLock)
                    {
                        try
                        {
                            return Task.FromResult(OrderManager.Get(OrderManager.Create(
                                clientUsername, orderDate, productIdQuantityMap, totalPrice, deliveryDate)));
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public Task<IProduct> CreateProduct(string name, double price, ProductType productType)
        {
            lock (ProductLock)
            {
                try
                {
                    return Task.FromResult(ProductManager.Get(ProductManager.Create(name, price, productType)));
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public Task<bool> Update(IClient client)
        {
            lock (ClientLock)
            {
                lock (OrderLock)
                {
                    return Task.FromResult(ClientManager.Update(client));
                }
            }
        }

        public Task<bool> Update(IOrder order)
        {
            lock (ClientLock)
            {
                if (ClientManager.Get(order.ClientUsername) == null)
                {
                    return Task.FromResult(false);
                }
                lock (ProductLock)
                {
                    double totalPrice = 0.0;
                    foreach (KeyValuePair<uint, uint> pair in order.ProductIdQuantityMap)
                    {
                        IProduct product = ProductManager.Get(pair.Key);
                        if (product == null)
                        {
                            return Task.FromResult(false);
                        }
                        totalPrice += product.Price * pair.Value;
                    }
                    order.Price = totalPrice;
                    lock (OrderLock)
                    {
                        return Task.FromResult(OrderManager.Update(order));
                    }
                }
            }
        }

        public Task<bool> UpdateClient(string username, string firstName, string lastName, string street, uint streetNumber,
            string phoneNumber)
        {
            return Update(new Client(username, firstName, lastName, street, streetNumber, phoneNumber));
        }

        public Task<bool> UpdateOrder(uint id, string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap, double price,
            DateTime? deliveryDate)
        {
            return Update(new Order(id, clientUsername, orderDate, productIdQuantityMap, price, deliveryDate));
        }

        public Task<bool> UpdateProduct(uint id, string name, double price, ProductType productType)
        {
            return Update(new Product(id, name, price, productType));
        }

        public Task<bool> Update(IProduct product)
        {

            lock (ProductLock)
            {
                lock (OrderLock)
                {
                    return Task.FromResult(ProductManager.Update(product));
                }
            }
        }

        public Task<bool> RemoveClient(IClient client)
        {
            lock (ClientLock)
            {
                lock (OrderLock)
                {
                    if (ClientManager.Get(client.Username) == null)
                    {
                        return Task.FromResult(false);
                    }

                    foreach (uint orderId in OrderManager.GetAll().Where(order => order.ClientUsername == client.Username).Select(o => o.Id))
                    {
                        OrderManager.Remove(orderId);
                    }

                    return Task.FromResult(ClientManager.Remove(client.Username));
                }
            }
        }

        public Task<bool> RemoveOrder(IOrder order)
        {
            lock (OrderLock)
            {
                return Task.FromResult(OrderManager.Remove(order.Id));
            }
        }

        public Task<bool> RemoveProduct(IProduct product)
        {
            lock (ProductLock)
            {
                lock (OrderLock)
                {
                    if (ProductManager.Get(product.Id) == null)
                    {
                        return Task.FromResult(false);
                    }
                    foreach (IOrder order in OrderManager.GetAll())
                    {
                        if (order.ProductIdQuantityMap.ContainsKey(product.Id))
                        {
                            return Task.FromResult(false);
                        }
                    }
                    return Task.FromResult(ProductManager.Remove(product.Id));
                }
            }
        }

        #region IObservable implementation

        private HashSet<IObserver<OrderSent>> OrderSentObservers { get; } = new HashSet<IObserver<OrderSent>>();

        public IDisposable Subscribe(IObserver<OrderSent> observer)
        {
            OrderSentObservers.Add(observer);
            return new Unsubscriber<OrderSent>(OrderSentObservers, observer);
        }

        public IDisposable Subscribe(IObserver<DataChanged<IClient>> observer)
        {
            return ClientManager.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<DataChanged<IProduct>> observer)
        {
            return ProductManager.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<DataChanged<IOrder>> observer)
        {
            return OrderManager.Subscribe(observer);
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue;

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OrderUnsubscriber.Dispose();
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
