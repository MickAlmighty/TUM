using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Data;
using Data.Transfer;

using WebSockets;

using Exception = System.Exception;

namespace Logic.Client
{
    public class WebRepository : IDataRepository, IDisposable
    {
        private const int WEB_TIMEOUT = 5000;

        private WebSocketConnection WebSocketConnection { get; set; }
        private WebSerializer WebSerializer { get; } = new WebSerializer();
        private ConcurrentQueue<string> MessageQueue { get; } = new ConcurrentQueue<string>();
        private ManualResetEvent NewMessageEvent { get; } = new ManualResetEvent(false);

        private HashSet<IObserver<OrderSent>> OrderSentObservers { get; } = new HashSet<IObserver<OrderSent>>();
        private HashSet<IObserver<DataChanged<Data.Client>>> ClientObservers { get; } = new HashSet<IObserver<DataChanged<Data.Client>>>();
        private HashSet<IObserver<DataChanged<Product>>> ProductObservers { get; } = new HashSet<IObserver<DataChanged<Product>>>();
        private HashSet<IObserver<DataChanged<Order>>> OrderObservers { get; } = new HashSet<IObserver<DataChanged<Order>>>();

        public async Task<bool> OpenRepository()
        {
            try
            {
                WebSocketConnection = await WebSocketClient.ConnectAsync(new Uri("ws://localhost:4444"));
                WebSocketConnection.OnMessage += (e, a) => OnMessage(a.Message);
                WebSocketConnection.OnError += (e, a) => OnError(a.Exception);
                WebSocketConnection.OnClose += (e, a) => OnClose();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public void OnMessage(string message)
        {
            Debug.WriteLine($"{WebSocketConnection}> {message}");

            if (WebSerializer.TryParseRequest(message, out WebSimpleMessageType simpleMessage))
            {
                switch (simpleMessage)
                {
                    case WebSimpleMessageType.Failure:
                    case WebSimpleMessageType.Success:
                        MessageQueue.Enqueue(message);
                        NewMessageEvent.Set();
                        break;
                    default:
                        // the server will not ask the client for data
                        throw new NotImplementedException();
                }
            }
            else
            {
                WebMessageDTO<object> msgDto = WebSerializer.DeserializeWebMessage(message);
                switch (msgDto.MessageType)
                {
                    case WebMessageType.ProvideAllClients:
                    case WebMessageType.ProvideAllProducts:
                    case WebMessageType.ProvideAllOrders:
                    case WebMessageType.ProvideClient:
                    case WebMessageType.ProvideProduct:
                    case WebMessageType.ProvideOrder:
                        {
                            MessageQueue.Enqueue(message);
                            NewMessageEvent.Set();
                            break;
                        }
                    case WebMessageType.OrderSent:
                        {
                            OrderSent orderSent = new OrderSent((msgDto.Data as OrderDTO)?.ToOrder());
                            foreach (IObserver<OrderSent> observer in OrderSentObservers)
                            {
                                observer.OnNext(orderSent);
                            }

                            break;
                        }
                    case WebMessageType.AddClient:
                        {
                            DataChanged<Data.Client> change =
                                new DataChanged<Data.Client>(DataChangedAction.Add,
                                    new List<Data.Client> { (msgDto.Data as ClientDTO)?.ToClient() });
                            foreach (IObserver<DataChanged<Data.Client>> observer in ClientObservers)
                            {
                                observer.OnNext(change);
                            }

                            break;
                        }
                    case WebMessageType.UpdateClient:
                        {
                            DataChanged<Data.Client> change =
                                new DataChanged<Data.Client>(DataChangedAction.Update,
                                    new List<Data.Client> { (msgDto.Data as ClientDTO)?.ToClient() });
                            foreach (IObserver<DataChanged<Data.Client>> observer in ClientObservers)
                            {
                                observer.OnNext(change);
                            }

                            break;
                        }
                    case WebMessageType.RemoveClient:
                        {
                            DataChanged<Data.Client> change =
                                new DataChanged<Data.Client>(DataChangedAction.Remove,
                                    new List<Data.Client> { (msgDto.Data as ClientDTO)?.ToClient() });
                            foreach (IObserver<DataChanged<Data.Client>> observer in ClientObservers)
                            {
                                observer.OnNext(change);
                            }

                            break;
                        }
                    case WebMessageType.AddProduct:
                        {
                            DataChanged<Product> change =
                                new DataChanged<Product>(DataChangedAction.Add,
                                    new List<Product> { (msgDto.Data as ProductDTO)?.ToProduct() });
                            foreach (IObserver<DataChanged<Product>> observer in ProductObservers)
                            {
                                observer.OnNext(change);
                            }

                            break;
                        }
                    case WebMessageType.UpdateProduct:
                        {
                            DataChanged<Product> change =
                                new DataChanged<Product>(DataChangedAction.Update,
                                    new List<Product> { (msgDto.Data as ProductDTO)?.ToProduct() });
                            foreach (IObserver<DataChanged<Product>> observer in ProductObservers)
                            {
                                observer.OnNext(change);
                            }

                            break;
                        }
                    case WebMessageType.RemoveProduct:
                        {
                            DataChanged<Product> change =
                                new DataChanged<Product>(DataChangedAction.Remove,
                                    new List<Product> { (msgDto.Data as ProductDTO)?.ToProduct() });
                            foreach (IObserver<DataChanged<Product>> observer in ProductObservers)
                            {
                                observer.OnNext(change);
                            }

                            break;
                        }
                    case WebMessageType.AddOrder:
                        {
                            DataChanged<Order> change =
                                new DataChanged<Order>(DataChangedAction.Add,
                                    new List<Order> { (msgDto.Data as OrderDTO)?.ToOrder() });
                            foreach (IObserver<DataChanged<Order>> observer in OrderObservers)
                            {
                                observer.OnNext(change);
                            }

                            break;
                        }
                    case WebMessageType.UpdateOrder:
                        {
                            DataChanged<Order> change =
                                new DataChanged<Order>(DataChangedAction.Update,
                                    new List<Order> { (msgDto.Data as OrderDTO)?.ToOrder() });
                            foreach (IObserver<DataChanged<Order>> observer in OrderObservers)
                            {
                                observer.OnNext(change);
                            }

                            break;
                        }
                    case WebMessageType.RemoveOrder:
                        {
                            DataChanged<Order> change =
                                new DataChanged<Order>(DataChangedAction.Remove,
                                    new List<Order> { (msgDto.Data as OrderDTO)?.ToOrder() });
                            foreach (IObserver<DataChanged<Order>> observer in OrderObservers)
                            {
                                observer.OnNext(change);
                            }

                            break;
                        }
                    case WebMessageType.Error:
                        {
                            string errorMsg = msgDto.Data as string;
                            Debug.WriteLine($"The server has encountered an exception! {errorMsg}");
                            break;
                        }
                }
            }
        }

        public void OnClose()
        {
            Debug.WriteLine($"Client web socket connection {WebSocketConnection} has been closed.");
        }

        public void OnError(Exception e)
        {
            Debug.WriteLine($"Client web socket connection {WebSocketConnection} error: {e}");
        }

        private string ReadMessage()
        {
            NewMessageEvent.Reset();
            while (!MessageQueue.IsEmpty)
            {
                if (MessageQueue.TryDequeue(out string msg))
                {
                    return msg;
                }
            }

            if (!NewMessageEvent.WaitOne(WEB_TIMEOUT))
            {
                //throw new TimeoutException("Read operation timed out!");
            }

            if (MessageQueue.TryDequeue(out string message))
            {
                return message;
            }

            return null;
        }

        public async Task<HashSet<Data.Client>> GetAllClients()
        {
            await WebSocketConnection.SendAsync(WebSimpleMessageType.GetAllClients.ToString());
            WebMessageDTO<object> msg = WebSerializer.DeserializeWebMessage(ReadMessage());
            if (!(msg.Data is HashSet<ClientDTO> clientData))
            {
                throw new ApplicationException("Provided client data object is invalid!");
            }
            return new HashSet<Data.Client>(clientData.Select(c => c.ToClient()));
        }

        public async Task<HashSet<Order>> GetAllOrders()
        {
            await WebSocketConnection.SendAsync(WebSimpleMessageType.GetAllOrders.ToString());
            WebMessageDTO<object> msg = WebSerializer.DeserializeWebMessage(ReadMessage());
            if (!(msg.Data is HashSet<OrderDTO> orderData))
            {
                throw new ApplicationException("Provided order data object is invalid!");
            }
            return new HashSet<Order>(orderData.Select(o => o.ToOrder()));
        }

        public async Task<HashSet<Product>> GetAllProducts()
        {
            await WebSocketConnection.SendAsync(WebSimpleMessageType.GetAllProducts.ToString());
            WebMessageDTO<object> msg = WebSerializer.DeserializeWebMessage(ReadMessage());
            if (!(msg.Data is HashSet<ProductDTO> productData))
            {
                throw new ApplicationException("Provided product data object is invalid!");
            }
            return new HashSet<Product>(productData.Select(p => p.ToProduct()));
        }

        public async Task<bool> CreateClient(string username, string firstName, string lastName, string street, uint streetNumber,
            string phoneNumber)
        {
            string msg = WebSerializer.SerializeWebMessage(WebMessageType.AddClient, new ClientDTO {
                Username = username,
                FirstName = firstName,
                LastName = lastName,
                Street = street,
                StreetNumber = streetNumber,
                PhoneNumber = phoneNumber
            });
            await WebSocketConnection.SendAsync(msg);
            string response = ReadMessage();
            if (WebSerializer.TryParseRequest(response, out WebSimpleMessageType req) && req == WebSimpleMessageType.Success)
            {
                return true;
            }
            Debug.WriteLine($"Failed to add client! {response}");
            return false;
        }

        public async Task<bool> CreateOrder(string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap, DateTime? deliveryDate)
        {
            string msg = WebSerializer.SerializeWebMessage(WebMessageType.AddOrder, new OrderDTO {
                ClientUsername = clientUsername,
                OrderDate = orderDate,
                ProductIdQuantityMap = productIdQuantityMap,
                DeliveryDate = deliveryDate
            });
            await WebSocketConnection.SendAsync(msg);
            string response = ReadMessage();
            if (WebSerializer.TryParseRequest(response, out WebSimpleMessageType req) && req == WebSimpleMessageType.Success)
            {
                return true;
            }
            Debug.WriteLine($"Failed to add order! {response}");
            return false;
        }

        public async Task<bool> CreateProduct(string name, double price, ProductType productType)
        {
            string msg = WebSerializer.SerializeWebMessage(WebMessageType.AddProduct, new ProductDTO {
                Name = name,
                Price = price,
                ProductType = productType
            });
            await WebSocketConnection.SendAsync(msg);
            string response = ReadMessage();
            if (WebSerializer.TryParseRequest(response, out WebSimpleMessageType req) && req == WebSimpleMessageType.Success)
            {
                return true;
            }
            Debug.WriteLine($"Failed to add product! {response}");
            return false;
        }

        public async Task<Data.Client> GetClient(string username)
        {
            string msg = WebSerializer.SerializeWebMessage(WebMessageType.GetClient, username);
            await WebSocketConnection.SendAsync(msg);
            WebMessageDTO<object> response = WebSerializer.DeserializeWebMessage(ReadMessage());
            if (!(response.Data is ClientDTO clientData))
            {
                throw new ApplicationException("Provided client data object is invalid!");
            }

            return clientData.ToClient();
        }

        public async Task<Order> GetOrder(uint id)
        {
            string msg = WebSerializer.SerializeWebMessage(WebMessageType.GetOrder, id);
            await WebSocketConnection.SendAsync(msg);
            WebMessageDTO<object> response = WebSerializer.DeserializeWebMessage(ReadMessage());
            if (!(response.Data is OrderDTO orderData))
            {
                throw new ApplicationException("Provided order data object is invalid!");
            }

            return orderData.ToOrder();
        }

        public async Task<Product> GetProduct(uint id)
        {
            string msg = WebSerializer.SerializeWebMessage(WebMessageType.GetProduct, id);
            await WebSocketConnection.SendAsync(msg);
            WebMessageDTO<object> response = WebSerializer.DeserializeWebMessage(ReadMessage());
            if (!(response.Data is ProductDTO productData))
            {
                throw new ApplicationException("Provided product data object is invalid!");
            }

            return productData.ToProduct();
        }

        public async Task<bool> Update(Data.Client client)
        {
            string msg = WebSerializer.SerializeWebMessage(WebMessageType.UpdateClient, new ClientDTO(client));
            await WebSocketConnection.SendAsync(msg);
            string response = ReadMessage();
            if (WebSerializer.TryParseRequest(response, out WebSimpleMessageType req) && req == WebSimpleMessageType.Success)
            {
                return true;
            }
            Debug.WriteLine($"Failed to update client! {response}");
            return false;
        }

        public async Task<bool> Update(Order order)
        {
            string msg = WebSerializer.SerializeWebMessage(WebMessageType.UpdateOrder, new OrderDTO(order));
            await WebSocketConnection.SendAsync(msg);
            string response = ReadMessage();
            if (WebSerializer.TryParseRequest(response, out WebSimpleMessageType req) && req == WebSimpleMessageType.Success)
            {
                return true;
            }
            Debug.WriteLine($"Failed to update order! {response}");
            return false;
        }

        public async Task<bool> Update(Product product)
        {
            string msg = WebSerializer.SerializeWebMessage(WebMessageType.UpdateProduct, new ProductDTO(product));
            await WebSocketConnection.SendAsync(msg);
            string response = ReadMessage();
            if (WebSerializer.TryParseRequest(response, out WebSimpleMessageType req) && req == WebSimpleMessageType.Success)
            {
                return true;
            }
            Debug.WriteLine($"Failed to update product! {response}");
            return false;
        }

        public async Task<bool> RemoveClient(Data.Client client)
        {
            string msg = WebSerializer.SerializeWebMessage(WebMessageType.RemoveClient, new ClientDTO(client));
            await WebSocketConnection.SendAsync(msg);
            string response = ReadMessage();
            if (WebSerializer.TryParseRequest(response, out WebSimpleMessageType req) && req == WebSimpleMessageType.Success)
            {
                return true;
            }
            Debug.WriteLine($"Failed to remove client! {response}");
            return false;
        }

        public async Task<bool> RemoveOrder(Order order)
        {
            string msg = WebSerializer.SerializeWebMessage(WebMessageType.RemoveOrder, new OrderDTO(order));
            await WebSocketConnection.SendAsync(msg);
            string response = ReadMessage();
            if (WebSerializer.TryParseRequest(response, out WebSimpleMessageType req) && req == WebSimpleMessageType.Success)
            {
                return true;
            }
            Debug.WriteLine($"Failed to remove order! {response}");
            return false;
        }

        public async Task<bool> RemoveProduct(Product product)
        {
            string msg = WebSerializer.SerializeWebMessage(WebMessageType.RemoveProduct, new ProductDTO(product));
            await WebSocketConnection.SendAsync(msg);
            string response = ReadMessage();
            if (WebSerializer.TryParseRequest(response, out WebSimpleMessageType req) && req == WebSimpleMessageType.Success)
            {
                return true;
            }
            Debug.WriteLine($"Failed to remove product! {response}");
            return false;
        }

        public IDisposable Subscribe(IObserver<OrderSent> observer)
        {
            OrderSentObservers.Add(observer);
            return new Unsubscriber<OrderSent>(OrderSentObservers, observer);
        }

        public IDisposable Subscribe(IObserver<DataChanged<Data.Client>> observer)
        {
            ClientObservers.Add(observer);
            return new Unsubscriber<DataChanged<Data.Client>>(ClientObservers, observer);
        }

        public IDisposable Subscribe(IObserver<DataChanged<Product>> observer)
        {
            ProductObservers.Add(observer);
            return new Unsubscriber<DataChanged<Product>>(ProductObservers, observer);
        }

        public IDisposable Subscribe(IObserver<DataChanged<Order>> observer)
        {
            OrderObservers.Add(observer);
            return new Unsubscriber<DataChanged<Order>>(OrderObservers, observer);
        }

        public void Dispose()
        {
            WebSocketConnection?.Dispose();
            NewMessageEvent?.Dispose();
        }
    }
}
