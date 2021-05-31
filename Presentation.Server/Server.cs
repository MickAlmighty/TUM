using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Data;
using Data.Transfer;

using DataModel.Transfer;

using WebSockets;

namespace Presentation.Server
{
    public class Server : IDisposable, IObserver<OrderSent>
    {
        public ServerWebSocketConnection ServerWebSocketConnection { get; }
        public IDataRepository DataRepository { get; }
        private string RepositoryParam { get; }

        private WebSerializer WebSerializer { get; } = new WebSerializer();

        private IDisposable OrderSentUnsubscriber { get; }

        public Server(uint port, IDataRepository dataRepository, string repositoryParam)
        {
            DataRepository = dataRepository;
            RepositoryParam = repositoryParam;
            OrderSentUnsubscriber = DataRepository.Subscribe(this);
            ServerWebSocketConnection = WebSocketServer.CreateServer(port);
        }

        public async Task RunServer()
        {
            if (!await DataRepository.OpenRepository(RepositoryParam))
            {
                Console.WriteLine("Failed to open the data repository!");
                return;
            }

            ServerWebSocketConnection.OnClientConnected += ServerWebSocketConnection_OnClientConnected;
            ServerWebSocketConnection.OnMessage += (e, a) => OnMessage(a.Connection, a.Message);
            ServerWebSocketConnection.OnError += (e, a) => OnError(a.Connection, a.Exception);
            ServerWebSocketConnection.OnClose += (e, a) => OnClose(a.Connection);
            await ServerWebSocketConnection.RunServerLoop();
        }

        private void ServerWebSocketConnection_OnClientConnected(object sender, OnClientConnectedEventArgs args)
        {
            Console.WriteLine($"Client {args.ClientConnection} connected!");
        }

        private async Task ProcessClientMessage(WebSocketConnection connection, string message)
        {
            Console.WriteLine($"{connection}> {message}");
            if (WebSerializer.TryParseRequest(message, out WebSimpleMessageType msgType))
            {
                switch (msgType)
                {
                    case WebSimpleMessageType.GetAllClients:
                        {
                            string response =
                                WebSerializer.SerializeWebMessage(WebMessageType.ProvideAllClients,
                                    new HashSet<ClientDTO>(
                                        (await DataRepository.GetAllClients())
                                        .Select(c => new ClientDTO(c))));
                            await connection.SendAsync(response);
                            break;
                        }
                    case WebSimpleMessageType.GetAllProducts:
                        {
                            string response =
                                WebSerializer.SerializeWebMessage(WebMessageType.ProvideAllProducts,
                                    new HashSet<ProductDTO>(
                                        (await DataRepository.GetAllProducts())
                                            .Select(p => new ProductDTO(p))));
                            await connection.SendAsync(response);
                            break;
                        }
                    case WebSimpleMessageType.GetAllOrders:
                        {
                            string response =
                                WebSerializer.SerializeWebMessage(WebMessageType.ProvideAllOrders,
                                    new HashSet<OrderDTO>(
                                        (await DataRepository.GetAllOrders())
                                            .Select(o => new OrderDTO(o))));
                            await connection.SendAsync(response);
                            break;
                        }
                }
            }
            else
            {
                try
                {
                    WebMessageDTO<object> msgDto = WebSerializer.DeserializeWebMessage(message);
                    switch (msgDto.MessageType)
                    {
                        case WebMessageType.AddClient:
                            {
                                if (msgDto.Data is ClientDTO clt)
                                {
                                    IClient client = await DataRepository.CreateClient(
                                        clt.Username, clt.FirstName, clt.LastName,
                                        clt.Street, clt.StreetNumber, clt.PhoneNumber);
                                    if (client == null)
                                    {
                                        await connection.SendAsync(WebSimpleMessageType.Failure.ToString());
                                    }
                                    else
                                    {
                                        ClientDTO newDto = new ClientDTO(client);
                                        await connection.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.ProvideClient, newDto));
                                        await ServerWebSocketConnection.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.AddClient, newDto));
                                    }
                                }
                                else
                                {
                                    await connection.SendAsync(WebSimpleMessageType.Failure.ToString());
                                }
                                break;
                            }
                        case WebMessageType.UpdateClient:
                            {
                                bool result = false;
                                if (msgDto.Data is ClientDTO clt)
                                {
                                    try
                                    {
                                        result = await DataRepository.Update(clt.ToIClient());
                                    }
                                    catch (Exception e)
                                    {
                                        await connection.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.Error, e.ToString()));
                                    }
                                }
                                await connection.SendAsync(result
                                    ? WebSimpleMessageType.Success.ToString()
                                    : WebSimpleMessageType.Failure.ToString());
                                if (result)
                                {
                                    await ServerWebSocketConnection.SendAsync(
                                        WebSerializer.SerializeWebMessage(WebMessageType.UpdateClient, msgDto.Data));
                                }

                                break;
                            }
                        case WebMessageType.RemoveClient:
                            {
                                bool result = false;
                                if (msgDto.Data is ClientDTO clt)
                                {
                                    try
                                    {
                                        result = await DataRepository.RemoveClient(clt.ToIClient());
                                    }
                                    catch (Exception e)
                                    {
                                        await connection.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.Error, e.ToString()));
                                    }
                                }
                                await connection.SendAsync(result
                                    ? WebSimpleMessageType.Success.ToString()
                                    : WebSimpleMessageType.Failure.ToString());
                                if (result)
                                {
                                    await ServerWebSocketConnection.SendAsync(
                                        WebSerializer.SerializeWebMessage(WebMessageType.RemoveClient, msgDto.Data));
                                }

                                break;
                            }
                        case WebMessageType.AddProduct:
                            {
                                if (msgDto.Data is ProductDTO prd)
                                {
                                    IProduct product = await DataRepository.CreateProduct(
                                        prd.Name, prd.Price, prd.ProductType);
                                    if (product == null)
                                    {
                                        await connection.SendAsync(WebSimpleMessageType.Failure.ToString());
                                    }
                                    else
                                    {
                                        ProductDTO newDto = new ProductDTO(product);
                                        await connection.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.ProvideProduct, newDto));
                                        await ServerWebSocketConnection.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.AddProduct, newDto));
                                    }
                                }
                                else
                                {
                                    await connection.SendAsync(WebSimpleMessageType.Failure.ToString());
                                }
                                break;
                            }
                        case WebMessageType.UpdateProduct:
                            {
                                bool result = false;
                                if (msgDto.Data is ProductDTO prd)
                                {
                                    try
                                    {
                                        result = await DataRepository.Update(prd.ToIProduct());
                                    }
                                    catch (Exception e)
                                    {
                                        await connection.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.Error, e.ToString()));
                                    }
                                }
                                await connection.SendAsync(result
                                    ? WebSimpleMessageType.Success.ToString()
                                    : WebSimpleMessageType.Failure.ToString());
                                if (result)
                                {
                                    await ServerWebSocketConnection.SendAsync(
                                        WebSerializer.SerializeWebMessage(WebMessageType.UpdateProduct, msgDto.Data));
                                }

                                break;
                            }
                        case WebMessageType.RemoveProduct:
                            {
                                bool result = false;
                                if (msgDto.Data is ProductDTO prd)
                                {
                                    try
                                    {
                                        result = await DataRepository.RemoveProduct(prd.ToIProduct());
                                    }
                                    catch (Exception e)
                                    {
                                        await connection.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.Error, e.ToString()));
                                    }
                                }
                                await connection.SendAsync(result
                                    ? WebSimpleMessageType.Success.ToString()
                                    : WebSimpleMessageType.Failure.ToString());
                                if (result)
                                {
                                    await ServerWebSocketConnection.SendAsync(
                                        WebSerializer.SerializeWebMessage(WebMessageType.RemoveProduct, msgDto.Data));
                                }

                                break;
                            }
                        case WebMessageType.AddOrder:
                            {
                                if (msgDto.Data is OrderDTO ord)
                                {
                                    IOrder order = await DataRepository.CreateOrder(
                                        ord.ClientUsername, ord.OrderDate,
                                        ord.ProductIdQuantityMap, ord.DeliveryDate);
                                    if (order == null)
                                    {
                                        await connection.SendAsync(WebSimpleMessageType.Failure.ToString());
                                    }
                                    else
                                    {
                                        OrderDTO newDto = new OrderDTO(order);
                                        await connection.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.ProvideOrder, newDto));
                                        await ServerWebSocketConnection.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.AddOrder, newDto));
                                    }
                                }
                                else
                                {
                                    await connection.SendAsync(WebSimpleMessageType.Failure.ToString());
                                }
                                break;
                            }
                        case WebMessageType.UpdateOrder:
                            {
                                bool result = false;
                                if (msgDto.Data is OrderDTO ord)
                                {
                                    try
                                    {
                                        result = await DataRepository.Update(ord.ToIOrder());
                                    }
                                    catch (Exception e)
                                    {
                                        await connection.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.Error, e.ToString()));
                                    }
                                }
                                await connection.SendAsync(result
                                    ? WebSimpleMessageType.Success.ToString()
                                    : WebSimpleMessageType.Failure.ToString());
                                if (result)
                                {
                                    await ServerWebSocketConnection.SendAsync(
                                        WebSerializer.SerializeWebMessage(WebMessageType.UpdateOrder, msgDto.Data));
                                }

                                break;
                            }
                        case WebMessageType.RemoveOrder:
                            {
                                bool result = false;
                                if (msgDto.Data is OrderDTO ord)
                                {
                                    try
                                    {
                                        result = await DataRepository.RemoveOrder(ord.ToIOrder());
                                    }
                                    catch (Exception e)
                                    {
                                        await connection.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.Error, e.ToString()));
                                    }
                                }
                                await connection.SendAsync(result
                                    ? WebSimpleMessageType.Success.ToString()
                                    : WebSimpleMessageType.Failure.ToString());
                                if (result)
                                {
                                    await ServerWebSocketConnection.SendAsync(
                                        WebSerializer.SerializeWebMessage(WebMessageType.RemoveOrder, msgDto.Data));
                                }

                                break;
                            }
                        case WebMessageType.GetClient:
                            {
                                if (!(msgDto.Data is string username))
                                {
                                    await connection.SendAsync(WebSimpleMessageType.Failure.ToString());
                                    break;
                                }
                                IClient client = await DataRepository.GetClient(username);
                                if (client == null)
                                {
                                    await connection.SendAsync(WebSimpleMessageType.Failure.ToString());
                                    break;
                                }

                                string response = WebSerializer.SerializeWebMessage(WebMessageType.ProvideClient,
                                    new ClientDTO(client));

                                await connection.SendAsync(response);
                                break;
                            }
                        case WebMessageType.GetProduct:
                            {
                                if (!(msgDto.Data is uint id))
                                {
                                    await connection.SendAsync(WebSimpleMessageType.Failure.ToString());
                                    break;
                                }
                                IProduct product = await DataRepository.GetProduct(id);
                                if (product == null)
                                {
                                    await connection.SendAsync(WebSimpleMessageType.Failure.ToString());
                                    break;
                                }

                                string response = WebSerializer.SerializeWebMessage(WebMessageType.ProvideProduct,
                                    new ProductDTO(product));

                                await connection.SendAsync(response);
                                break;
                            }
                        case WebMessageType.GetOrder:
                            {
                                if (!(msgDto.Data is uint id))
                                {
                                    await connection.SendAsync(WebSimpleMessageType.Failure.ToString());
                                    break;
                                }
                                IOrder order = await DataRepository.GetOrder(id);
                                if (order == null)
                                {
                                    await connection.SendAsync(WebSimpleMessageType.Failure.ToString());
                                    break;
                                }

                                string response = WebSerializer.SerializeWebMessage(WebMessageType.ProvideOrder,
                                    new OrderDTO(order));

                                await connection.SendAsync(response);
                                break;
                            }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to parse message '{message}' from a client! {e}");
                }
            }
        }

        public async void OnMessage(WebSocketConnection connection, string message)
        {
            await ProcessClientMessage(connection, message);
        }

        public void OnClose(WebSocketConnection connection)
        {
            Console.WriteLine($"Client {connection} has been closed!");
        }

        public void OnError(WebSocketConnection connection, Exception exception)
        {
            Console.WriteLine($"Client {connection} has reported an error: {exception}");
            Task.Run(connection.DisconnectAsync);
        }

        public void OnCompleted()
        {
            Console.WriteLine("OrderSent subscription has ended.");
        }

        public void OnError(Exception error)
        {
            Console.WriteLine($"An error occurred during OrderSent subscription: {error}");
        }

        public void OnNext(OrderSent value)
        {
            string msg = WebSerializer.SerializeWebMessage(WebMessageType.OrderSent, new OrderDTO(value.Order));
            Task.Run(() => ServerWebSocketConnection.SendAsync(msg));
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                OrderSentUnsubscriber?.Dispose();
                ServerWebSocketConnection?.Dispose();
                (DataRepository as IDisposable)?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Server()
        {
            Dispose(false);
        }
    }
}
