using System;
using System.Collections.Generic;

using Data.Transfer;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.TransferTest
{
    [TestClass]
    public class WebSerializerTest
    {
        private WebSerializer Serializer { get; } = new WebSerializer();

        private static ClientDTO SampleClientDTO { get; } = new ClientDTO {
            FirstName = "First name",
            LastName = "Last name",
            PhoneNumber = "123-456-789",
            Street = "Street",
            StreetNumber = 1U,
            Username = "Username"
        };

        private static ProductDTO SampleProductDTO { get; } = new ProductDTO {
            Id = 1U,
            Name = "Product",
            Price = 1.0,
            ProductType = ProductType.Toy
        };

        private static OrderDTO SampleOrderDTO { get; } = new OrderDTO {
            ClientUsername = "Username",
            DeliveryDate = null,
            Id = 1U,
            OrderDate = new DateTime(2020, 1, 1, 12, 0, 0),
            Price = 12.34,
            ProductIdQuantityMap = new Dictionary<uint, uint> {
                {1U, 5U },
                {2U, 3U }
            }
        };

        private Dictionary<WebMessageType, object> SampleWebMessageData
        {
            get;
        } = new Dictionary<WebMessageType, object> {
            { WebMessageType.AddClient, SampleClientDTO },
            { WebMessageType.UpdateClient, SampleClientDTO },
            { WebMessageType.ProvideClient, SampleClientDTO },
            { WebMessageType.AddProduct, SampleProductDTO },
            { WebMessageType.UpdateProduct, SampleProductDTO },
            { WebMessageType.ProvideProduct, SampleProductDTO },
            { WebMessageType.AddOrder, SampleOrderDTO },
            { WebMessageType.UpdateOrder, SampleOrderDTO },
            { WebMessageType.ProvideOrder, SampleOrderDTO },
            { WebMessageType.ProvideAllClients, new HashSet<ClientDTO> {SampleClientDTO} },
            { WebMessageType.ProvideAllProducts, new HashSet<ProductDTO> {SampleProductDTO} },
            { WebMessageType.ProvideAllOrders, new HashSet<OrderDTO> {SampleOrderDTO} },
            { WebMessageType.OrderSent, 1U },
            { WebMessageType.Error, "Sample exception message." }
        };

        [TestMethod]
        public void TryParseRequest_ValidValues_ParseSucceeds()
        {
            foreach (WebRequestType requestType in Enum.GetValues(typeof(WebRequestType)))
            {
                Assert.IsTrue(Serializer.TryParseRequest(requestType.ToString(), out WebRequestType returnedRequestType));
                Assert.AreEqual(requestType, returnedRequestType);
            }
        }

        [TestMethod]
        public void TryParseRequest_InvalidValues_ParseFails()
        {
            foreach (string value in new[] { null, "", "asdf", "Invalid", "    " })
            {
                Assert.IsFalse(Serializer.TryParseRequest(value, out WebRequestType _));
            }
        }

        [TestMethod]
        public void SerializeWebMessage_ValidArguments_ReturnsNonEmptyString()
        {
            foreach (KeyValuePair<WebMessageType, object> pair in SampleWebMessageData)
            {
                Assert.IsTrue(Serializer.SerializeWebMessage(pair.Key, pair.Value).Length > 0);
            }
        }

        [TestMethod]
        public void SerializeWebMessage_InvalidArguments_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(()=>Serializer.SerializeWebMessage(WebMessageType.OrderSent, null));
            Assert.ThrowsException<ArgumentException>(()=>Serializer.SerializeWebMessage(WebMessageType.AddClient, new OrderDTO()));
            Assert.ThrowsException<ArgumentException>(()=>Serializer.SerializeWebMessage(WebMessageType.AddProduct, new ClientDTO()));
            Assert.ThrowsException<ArgumentException>(()=>Serializer.SerializeWebMessage(WebMessageType.AddOrder, new ProductDTO()));
            Assert.ThrowsException<ArgumentException>(()=>Serializer.SerializeWebMessage(WebMessageType.OrderSent, 1.0));
            Assert.ThrowsException<ArgumentException>(()=>Serializer.SerializeWebMessage(WebMessageType.Error, true));
            Assert.ThrowsException<ArgumentException>(()=>Serializer.SerializeWebMessage(WebMessageType.ProvideAllClients, new HashSet<ProductDTO>()));
            Assert.ThrowsException<ArgumentException>(()=>Serializer.SerializeWebMessage(WebMessageType.ProvideAllProducts, new ProductDTO()));
        }

        [TestMethod]
        public void DeserializeWebMessage_PreviouslySerializedData_ReturnsValidObjects()
        {
            foreach (KeyValuePair<WebMessageType, object> pair in SampleWebMessageData)
            {
                string jsonData = Serializer.SerializeWebMessage(pair.Key, pair.Value);
                WebMessageDTO<object> messageDto = Serializer.DeserializeWebMessage(jsonData);
                Assert.IsNotNull(messageDto);
                Assert.IsNotNull(messageDto.Data);
            }
        }
    }
}
