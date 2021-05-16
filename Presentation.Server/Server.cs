using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Data;
using Data.Transfer;

using Logic;

namespace Presentation.Server
{
    internal class Server : WebReceiver, IObserver<OrderSent>
    {
        private WebSerializer WebSerializer { get; } = new WebSerializer();
        private HashSet<WebSocketConnection> Connections { get; } = new HashSet<WebSocketConnection>();

        private IDataRepository DataRepository { get; }

        public Server(IDataRepository dataRepository)
        {
            DataRepository = dataRepository;
        }

        public async Task RunServer()
        {
            await WebSocketServer.Server(this, 4444, wsc => {
                Connections.Add(wsc);
                Console.WriteLine("A new client has connected!");
            });
        }

        private async void ProcessClientMessage(WebSocketConnection connection, string message)
        {
            if (WebSerializer.TryParseRequest(message, out WebSimpleMessageType msgType))
            {
                Console.WriteLine($"New client request: {msgType}.");
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
                                bool result = msgDto.Data is ClientDTO clt &&
                                              await DataRepository.CreateClient(
                                                  clt.Username, clt.FirstName, clt.LastName,
                                                  clt.Street, clt.StreetNumber, clt.PhoneNumber);
                                await connection.SendAsync(result
                                    ? WebSimpleMessageType.Success.ToString()
                                    : WebSimpleMessageType.Failure.ToString());
                                if (result)
                                {
                                    foreach (WebSocketConnection conn in Connections)
                                    {
                                        await conn.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.AddClient, msgDto.Data));
                                    }
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
                                        result = await DataRepository.Update(clt.ToClient());
                                    }
                                    catch (Exception) { }
                                }
                                await connection.SendAsync(result
                                    ? WebSimpleMessageType.Success.ToString()
                                    : WebSimpleMessageType.Failure.ToString());
                                if (result)
                                {
                                    foreach (WebSocketConnection conn in Connections)
                                    {
                                        await conn.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.UpdateClient, msgDto.Data));
                                    }
                                }

                                break;
                            }
                        case WebMessageType.RemoveClient:
                            {
                                bool result = msgDto.Data is ClientDTO clt &&
                                              await DataRepository.RemoveClient(clt.Username);
                                await connection.SendAsync(result
                                    ? WebSimpleMessageType.Success.ToString()
                                    : WebSimpleMessageType.Failure.ToString());
                                if (result)
                                {
                                    foreach (WebSocketConnection conn in Connections)
                                    {
                                        await conn.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.RemoveClient, msgDto.Data));
                                    }
                                }

                                break;
                            }
                        case WebMessageType.AddProduct:
                            {
                                if (!(msgDto.Data is ProductDTO prd))
                                {
                                    await connection.SendAsync(WebSimpleMessageType.Failure.ToString());
                                    break;
                                }
                                bool result = await DataRepository.CreateProduct(
                                                  prd.Name, prd.Price, prd.ProductType);
                                await connection.SendAsync(result
                                    ? WebSimpleMessageType.Success.ToString()
                                    : WebSimpleMessageType.Failure.ToString());
                                if (result)
                                {
                                    ProductDTO dto = new ProductDTO((await DataRepository.GetAllProducts())
                                        .First(p => p.Name == prd.Name));
                                    foreach (WebSocketConnection conn in Connections)
                                    {
                                        await conn.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.AddProduct, dto));
                                    }
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
                                        result = await DataRepository.Update(prd.ToProduct());
                                    }
                                    catch (Exception) { }
                                }
                                await connection.SendAsync(result
                                    ? WebSimpleMessageType.Success.ToString()
                                    : WebSimpleMessageType.Failure.ToString());
                                if (result)
                                {
                                    foreach (WebSocketConnection conn in Connections)
                                    {
                                        await conn.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.UpdateProduct, msgDto.Data));
                                    }
                                }

                                break;
                            }
                        case WebMessageType.RemoveProduct:
                            {
                                bool result = msgDto.Data is ProductDTO prd &&
                                              await DataRepository.RemoveProduct(prd.Id);
                                await connection.SendAsync(result
                                    ? WebSimpleMessageType.Success.ToString()
                                    : WebSimpleMessageType.Failure.ToString());
                                if (result)
                                {
                                    foreach (WebSocketConnection conn in Connections)
                                    {
                                        await conn.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.RemoveProduct, msgDto.Data));
                                    }
                                }

                                break;
                            }
                        case WebMessageType.AddOrder:
                            {
                                if (!(msgDto.Data is OrderDTO ord))
                                {
                                    await connection.SendAsync(WebSimpleMessageType.Failure.ToString());
                                    break;
                                }
                                bool result = await DataRepository.CreateOrder(
                                                  ord.ClientUsername, ord.OrderDate,
                                                  ord.ProductIdQuantityMap, ord.DeliveryDate);
                                await connection.SendAsync(result
                                    ? WebSimpleMessageType.Success.ToString()
                                    : WebSimpleMessageType.Failure.ToString());
                                if (result)
                                {
                                    OrderDTO dto = new OrderDTO((await DataRepository.GetAllOrders())
                                        .First(o => o.ClientUsername == ord.ClientUsername && o.OrderDate == ord.OrderDate));
                                    foreach (WebSocketConnection conn in Connections)
                                    {
                                        await conn.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.AddOrder, dto));
                                    }
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
                                        result = await DataRepository.Update(ord.ToOrder());
                                    }
                                    catch (Exception) { }
                                }
                                await connection.SendAsync(result
                                    ? WebSimpleMessageType.Success.ToString()
                                    : WebSimpleMessageType.Failure.ToString());
                                if (result)
                                {
                                    foreach (WebSocketConnection conn in Connections)
                                    {
                                        await conn.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.UpdateOrder, msgDto.Data));
                                    }
                                }

                                break;
                            }
                        case WebMessageType.RemoveOrder:
                            {
                                bool result = msgDto.Data is OrderDTO ord &&
                                              await DataRepository.RemoveOrder(ord.Id);
                                await connection.SendAsync(result
                                    ? WebSimpleMessageType.Success.ToString()
                                    : WebSimpleMessageType.Failure.ToString());
                                if (result)
                                {
                                    foreach (WebSocketConnection conn in Connections)
                                    {
                                        await conn.SendAsync(
                                            WebSerializer.SerializeWebMessage(WebMessageType.RemoveOrder, msgDto.Data));
                                    }
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
                                Client client = await DataRepository.GetClient(username);
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
                                Product product = await DataRepository.GetProduct(id);
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
                                Order order = await DataRepository.GetOrder(id);
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

        public void OnMessage(WebSocketConnection connection, string message)
        {

        }

        public void OnClose(WebSocketConnection connection)
        {
            Console.WriteLine("A client connection has been closed!");
        }

        public void OnError(WebSocketConnection connection)
        {
            Console.WriteLine("A client has reported an error!");
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
            foreach (WebSocketConnection connection in Connections)
            {
                connection.SendAsync(msg).Start();
            }
        }
    }
}
