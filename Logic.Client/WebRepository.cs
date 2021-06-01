using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Data;
using Data.Transfer;

using DataModel;
using DataModel.Transfer;

using WebSockets;

using Exception = System.Exception;

namespace Logic.Client
{
    public class WebRepository : IDataRepository, IDisposable
    {
        private const int WEB_TIMEOUT = 5000;

        public WebSocketConnection WebSocketConnection { get; private set; }
        private WebSerializer WebSerializer { get; } = new WebSerializer();
        private ConcurrentQueue<string> MessageQueue { get; } = new ConcurrentQueue<string>();
        private ManualResetEvent NewMessageEvent { get; } = new ManualResetEvent(false);

        private HashSet<IObserver<OrderSent>> OrderSentObservers { get; } = new HashSet<IObserver<OrderSent>>();
        private HashSet<IObserver<DataChanged<IClient>>> ClientObservers { get; } = new HashSet<IObserver<DataChanged<IClient>>>();
        private HashSet<IObserver<DataChanged<IProduct>>> ProductObservers { get; } = new HashSet<IObserver<DataChanged<IProduct>>>();
        private HashSet<IObserver<DataChanged<IOrder>>> OrderObservers { get; } = new HashSet<IObserver<DataChanged<IOrder>>>();

        public event OnRepositoryClosedEventHandler OnRepositoryClosed;

        public async Task<bool> OpenRepository(string connectionUri)
        {
            try
            {
                WebSocketConnection = await WebSocketClient.ConnectAsync(new Uri(connectionUri));
                WebSocketConnection.OnMessage += (e, a) => OnMessage(a.Message);
                WebSocketConnection.OnError += (e, a) => OnError(a.Exception);
                WebSocketConnection.OnClose += WebSocketConnection_OnClose;
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        private void WebSocketConnection_OnClose(object sender, OnCloseEventHandlerArgs args)
        {
            Debug.WriteLine($"Client web socket connection {WebSocketConnection} has been closed.");
            OnRepositoryClosed?.Invoke(this, new OnRepositoryClosedEventHandlerArgs(this));
        }

        public async Task CloseRepository()
        {
            if (WebSocketConnection != null)
            {
                WebSocketConnection.OnClose -= WebSocketConnection_OnClose;
                await WebSocketConnection.DisconnectAsync();
                WebSocketConnection = null;
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
                            OrderSent orderSent = new OrderSent((msgDto.Data as OrderDTO)?.ToIOrder());
                            foreach (IObserver<OrderSent> observer in OrderSentObservers)
                            {
                                observer.OnNext(orderSent);
                            }

                            break;
                        }
                    case WebMessageType.AddClient:
                        {
                            DataChanged<IClient> change =
                                new DataChanged<IClient>(DataChangedAction.Add,
                                    new List<IClient> { (msgDto.Data as ClientDTO)?.ToIClient() });
                            foreach (IObserver<DataChanged<IClient>> observer in ClientObservers)
                            {
                                observer.OnNext(change);
                            }

                            break;
                        }
                    case WebMessageType.UpdateClient:
                        {
                            DataChanged<IClient> change =
                                new DataChanged<IClient>(DataChangedAction.Update,
                                    new List<IClient> { (msgDto.Data as ClientDTO)?.ToIClient() });
                            foreach (IObserver<DataChanged<IClient>> observer in ClientObservers)
                            {
                                observer.OnNext(change);
                            }

                            break;
                        }
                    case WebMessageType.RemoveClient:
                        {
                            DataChanged<IClient> change =
                                new DataChanged<IClient>(DataChangedAction.Remove,
                                    new List<IClient> { (msgDto.Data as ClientDTO)?.ToIClient() });
                            foreach (IObserver<DataChanged<IClient>> observer in ClientObservers)
                            {
                                observer.OnNext(change);
                            }

                            break;
                        }
                    case WebMessageType.AddProduct:
                        {
                            DataChanged<IProduct> change =
                                new DataChanged<IProduct>(DataChangedAction.Add,
                                    new List<IProduct> { (msgDto.Data as ProductDTO)?.ToIProduct() });
                            foreach (IObserver<DataChanged<IProduct>> observer in ProductObservers)
                            {
                                observer.OnNext(change);
                            }

                            break;
                        }
                    case WebMessageType.UpdateProduct:
                        {
                            DataChanged<IProduct> change =
                                new DataChanged<IProduct>(DataChangedAction.Update,
                                    new List<IProduct> { (msgDto.Data as ProductDTO)?.ToIProduct() });
                            foreach (IObserver<DataChanged<IProduct>> observer in ProductObservers)
                            {
                                observer.OnNext(change);
                            }

                            break;
                        }
                    case WebMessageType.RemoveProduct:
                        {
                            DataChanged<IProduct> change =
                                new DataChanged<IProduct>(DataChangedAction.Remove,
                                    new List<IProduct> { (msgDto.Data as ProductDTO)?.ToIProduct() });
                            foreach (IObserver<DataChanged<IProduct>> observer in ProductObservers)
                            {
                                observer.OnNext(change);
                            }

                            break;
                        }
                    case WebMessageType.AddOrder:
                        {
                            DataChanged<IOrder> change =
                                new DataChanged<IOrder>(DataChangedAction.Add,
                                    new List<IOrder> { (msgDto.Data as OrderDTO)?.ToIOrder() });
                            foreach (IObserver<DataChanged<IOrder>> observer in OrderObservers)
                            {
                                observer.OnNext(change);
                            }

                            break;
                        }
                    case WebMessageType.UpdateOrder:
                        {
                            DataChanged<IOrder> change =
                                new DataChanged<IOrder>(DataChangedAction.Update,
                                    new List<IOrder> { (msgDto.Data as OrderDTO)?.ToIOrder() });
                            foreach (IObserver<DataChanged<IOrder>> observer in OrderObservers)
                            {
                                observer.OnNext(change);
                            }

                            break;
                        }
                    case WebMessageType.RemoveOrder:
                        {
                            DataChanged<IOrder> change =
                                new DataChanged<IOrder>(DataChangedAction.Remove,
                                    new List<IOrder> { (msgDto.Data as OrderDTO)?.ToIOrder() });
                            foreach (IObserver<DataChanged<IOrder>> observer in OrderObservers)
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

        public async Task<HashSet<IClient>> GetAllClients()
        {
            await WebSocketConnection.SendAsync(WebSimpleMessageType.GetAllClients.ToString());
            WebMessageDTO<object> msg = WebSerializer.DeserializeWebMessage(ReadMessage());
            if (!(msg.Data is HashSet<ClientDTO> clientData))
            {
                throw new ApplicationException("Provided client data object is invalid!");
            }
            return new HashSet<IClient>(clientData.Select(c => c.ToIClient()));
        }

        public async Task<HashSet<IOrder>> GetAllOrders()
        {
            await WebSocketConnection.SendAsync(WebSimpleMessageType.GetAllOrders.ToString());
            WebMessageDTO<object> msg = WebSerializer.DeserializeWebMessage(ReadMessage());
            if (!(msg.Data is HashSet<OrderDTO> orderData))
            {
                throw new ApplicationException("Provided order data object is invalid!");
            }
            return new HashSet<IOrder>(orderData.Select(o => o.ToIOrder()));
        }

        public async Task<HashSet<IProduct>> GetAllProducts()
        {
            await WebSocketConnection.SendAsync(WebSimpleMessageType.GetAllProducts.ToString());
            WebMessageDTO<object> msg = WebSerializer.DeserializeWebMessage(ReadMessage());
            if (!(msg.Data is HashSet<ProductDTO> productData))
            {
                throw new ApplicationException("Provided product data object is invalid!");
            }
            return new HashSet<IProduct>(productData.Select(p => p.ToIProduct()));
        }

        public async Task<IClient> CreateClient(string username, string firstName, string lastName, string street, uint streetNumber,
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
            string responseMsg;
            try
            {
                responseMsg = ReadMessage();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to add client! {e}");
                return null;
            }
            WebMessageDTO<object> response = WebSerializer.DeserializeWebMessage(responseMsg);
            if (!(response.Data is ClientDTO clientData))
            {
                throw new ApplicationException("Provided client data object is invalid!");
            }

            return clientData.ToIClient();
        }

        public async Task<IOrder> CreateOrder(string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap, DateTime? deliveryDate)
        {
            string msg = WebSerializer.SerializeWebMessage(WebMessageType.AddOrder, new OrderDTO {
                ClientUsername = clientUsername,
                OrderDate = orderDate,
                ProductIdQuantityMap = productIdQuantityMap,
                DeliveryDate = deliveryDate
            });
            await WebSocketConnection.SendAsync(msg);
            string responseMsg;
            try
            {
                responseMsg = ReadMessage();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to add order! {e}");
                return null;
            }
            WebMessageDTO<object> response = WebSerializer.DeserializeWebMessage(responseMsg);
            if (!(response.Data is OrderDTO orderData))
            {
                throw new ApplicationException("Provided order data object is invalid!");
            }

            return orderData.ToIOrder();
        }

        public async Task<IProduct> CreateProduct(string name, double price, ProductType productType)
        {
            string msg = WebSerializer.SerializeWebMessage(WebMessageType.AddProduct, new ProductDTO {
                Name = name,
                Price = price,
                ProductType = productType
            });
            await WebSocketConnection.SendAsync(msg);
            string responseMsg;
            try
            {
                responseMsg = ReadMessage();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to add product! {e}");
                return null;
            }
            WebMessageDTO<object> response = WebSerializer.DeserializeWebMessage(responseMsg);
            if (!(response.Data is ProductDTO productData))
            {
                throw new ApplicationException("Provided product data object is invalid!");
            }

            return productData.ToIProduct();
        }

        public async Task<IClient> GetClient(string username)
        {
            string msg = WebSerializer.SerializeWebMessage(WebMessageType.GetClient, username);
            await WebSocketConnection.SendAsync(msg);
            WebMessageDTO<object> response = WebSerializer.DeserializeWebMessage(ReadMessage());
            if (!(response.Data is ClientDTO clientData))
            {
                throw new ApplicationException("Provided client data object is invalid!");
            }

            return clientData.ToIClient();
        }

        public async Task<IOrder> GetOrder(uint id)
        {
            string msg = WebSerializer.SerializeWebMessage(WebMessageType.GetOrder, id);
            await WebSocketConnection.SendAsync(msg);
            WebMessageDTO<object> response = WebSerializer.DeserializeWebMessage(ReadMessage());
            if (!(response.Data is OrderDTO orderData))
            {
                throw new ApplicationException("Provided order data object is invalid!");
            }

            return orderData.ToIOrder();
        }

        public async Task<IProduct> GetProduct(uint id)
        {
            string msg = WebSerializer.SerializeWebMessage(WebMessageType.GetProduct, id);
            await WebSocketConnection.SendAsync(msg);
            WebMessageDTO<object> response = WebSerializer.DeserializeWebMessage(ReadMessage());
            if (!(response.Data is ProductDTO productData))
            {
                throw new ApplicationException("Provided product data object is invalid!");
            }

            return productData.ToIProduct();
        }

        public async Task<bool> Update(IClient client)
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

        public async Task<bool> Update(IOrder order)
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

        public async Task<bool> UpdateClient(string username, string firstName, string lastName, string street, uint streetNumber,
            string phoneNumber)
        {
            return await Update(new DataModel.Client(username, firstName, lastName, street, streetNumber, phoneNumber));
        }

        public async Task<bool> UpdateOrder(uint id, string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap,
            double price, DateTime? deliveryDate)
        {
            return await Update(new Order(id, clientUsername, orderDate, productIdQuantityMap, price, deliveryDate));
        }

        public async Task<bool> UpdateProduct(uint id, string name, double price, ProductType productType)
        {
            return await Update(new Product(id, name, price, productType));
        }

        public async Task<bool> Update(IProduct product)
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

        public async Task<bool> RemoveClient(IClient client)
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

        public async Task<bool> RemoveOrder(IOrder order)
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

        public async Task<bool> RemoveProduct(IProduct product)
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

        public IDisposable Subscribe(IObserver<DataChanged<IClient>> observer)
        {
            ClientObservers.Add(observer);
            return new Unsubscriber<DataChanged<IClient>>(ClientObservers, observer);
        }

        public IDisposable Subscribe(IObserver<DataChanged<IProduct>> observer)
        {
            ProductObservers.Add(observer);
            return new Unsubscriber<DataChanged<IProduct>>(ProductObservers, observer);
        }

        public IDisposable Subscribe(IObserver<DataChanged<IOrder>> observer)
        {
            OrderObservers.Add(observer);
            return new Unsubscriber<DataChanged<IOrder>>(OrderObservers, observer);
        }

        public void Dispose()
        {
            WebSocketConnection?.Dispose();
            NewMessageEvent?.Dispose();
        }
    }
}
