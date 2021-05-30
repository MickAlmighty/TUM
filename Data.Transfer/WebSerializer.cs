using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

using DataModel.Transfer;

using Newtonsoft.Json;

using JsonException = System.Text.Json.JsonException;

namespace Data.Transfer
{
    public sealed class WebSerializer
    {
        private Dictionary<WebMessageType, Type> MessageDataTypes { get; } = new Dictionary<WebMessageType, Type> {
            { WebMessageType.AddClient, typeof(ClientDTO) },
            { WebMessageType.UpdateClient, typeof(ClientDTO) },
            { WebMessageType.ProvideClient, typeof(ClientDTO) },
            { WebMessageType.RemoveClient, typeof(ClientDTO) },
            { WebMessageType.AddProduct, typeof(ProductDTO) },
            { WebMessageType.UpdateProduct, typeof(ProductDTO) },
            { WebMessageType.ProvideProduct, typeof(ProductDTO) },
            { WebMessageType.RemoveProduct, typeof(ProductDTO) },
            { WebMessageType.AddOrder, typeof(OrderDTO) },
            { WebMessageType.UpdateOrder, typeof(OrderDTO) },
            { WebMessageType.ProvideOrder, typeof(OrderDTO) },
            { WebMessageType.RemoveOrder, typeof(OrderDTO) },
            { WebMessageType.OrderSent, typeof(OrderDTO) },
            { WebMessageType.ProvideAllClients, typeof(HashSet<ClientDTO>) },
            { WebMessageType.ProvideAllProducts, typeof(HashSet<ProductDTO>) },
            { WebMessageType.ProvideAllOrders, typeof(HashSet<OrderDTO>) },
            { WebMessageType.GetProduct, typeof(uint) },
            { WebMessageType.GetOrder, typeof(uint) },
            { WebMessageType.GetClient, typeof(string) },
            { WebMessageType.Error, typeof(string) }
        };

        private Dictionary<WebMessageType, Type> MessageDTOTypes { get; } = new Dictionary<WebMessageType, Type>();

        public WebSerializer()
        {
            GenerateMessageDTOTypes();
        }

        private void GenerateMessageDTOTypes()
        {
            Type baseType = typeof(WebMessageDTO<>);
            foreach (KeyValuePair<WebMessageType, Type> pair in MessageDataTypes)
            {
                MessageDTOTypes.Add(pair.Key, baseType.MakeGenericType(pair.Value));
            }
        }

        private JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            // DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            DateFormatHandling = DateFormatHandling.IsoDateFormat
        };

        /// <summary>
        /// Attempts to parse provided web message to a <see cref="WebSimpleMessageType"/>.
        /// If the parse fails, the message is expected to be a <see cref="WebMessageDTO{T}"/> json.
        /// </summary>
        /// <param name="webMessage">The web message.</param>
        /// <param name="simpleMessageType">The parse result.</param>
        /// <returns>True if the parse succeeded, false otherwise.</returns>
        public bool TryParseRequest(string webMessage, out WebSimpleMessageType simpleMessageType)
        {
            return Enum.TryParse(webMessage, out simpleMessageType);
        }

        private WebMessageType GetMessageType(string jsonData)
        {
            if (string.IsNullOrWhiteSpace(jsonData))
            {
                throw new ArgumentException("Provided JSON data is either empty or null!");
            }
            JsonDocument jsonDocument = JsonDocument.Parse(jsonData);
            if (!jsonDocument.RootElement.TryGetProperty(nameof(WebMessageDTO<object>.MessageType), out JsonElement element))
            {
                throw new JsonException($"Provided JSON data does not specify {nameof(WebMessageType)}!");
            }

            if (element.ValueKind != JsonValueKind.String)
            {
                throw new JsonException($"Provided {nameof(WebMessageDTO<object>.MessageType)} is not a string ({element})!");
            }

            if (!Enum.TryParse(element.GetString(), out WebMessageType messageType))
            {
                throw new ArgumentOutOfRangeException($"Provided value '{element.GetString()}' is not a valid {nameof(WebMessageType)}!");
            }
            return messageType;
        }

        /// <summary>
        /// Returns a json string representing a <see cref="WebMessageDTO{T}"/> with given <see cref="WebMessageType"/> and <see cref="data"/> object.
        /// Throws an exception if provided data is null or of an invalid type.
        /// </summary>
        /// <param name="messageType">Web message type.</param>
        /// <param name="data">Web message data.</param>
        /// <returns>A json string representing a <see cref="WebMessageDTO{T}"/>.</returns>
        public string SerializeWebMessage(WebMessageType messageType, object data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!MessageDataTypes.ContainsKey(messageType))
            {
                throw new InvalidEnumArgumentException($"Encountered unsupported message type '{messageType}'!");
            }

            Type dataType = MessageDataTypes[messageType];

            if (data.GetType() != dataType && !data.GetType().IsInstanceOfType(dataType))
            {
                throw new ArgumentException($"Invalid data type! Expected {dataType}, got {data.GetType()}.");
            }

            Type genericType = MessageDTOTypes[messageType];

            return JsonConvert.SerializeObject(
                Activator.CreateInstance(genericType, messageType, data), SerializerSettings);
        }

        /// <summary>
        /// Deserializes provided json string into a <see cref="WebMessageDTO{T}"/> object.
        /// Before attempting to invoke this method on an aquired web message you should first use <see cref="TryParseRequest"/>
        ///     to check if it is a web request instead of a json web message.
        /// This method also verifies that the type of the web message data is appropriate for its <see cref="WebMessageType"/>.
        /// Throws an exception if the deserialization fails.
        /// </summary>
        /// <param name="jsonData">A json string representation of <see cref="WebMessageDTO{T}"/>.</param>
        /// <returns>A valid <see cref="WebMessageDTO{T}"/> instance.</returns>
        public WebMessageDTO<object> DeserializeWebMessage(string jsonData)
        {
            if (string.IsNullOrWhiteSpace(jsonData))
            {
                throw new ArgumentException("Provided JSON data is either empty or null!");
            }
            WebMessageType messageType = GetMessageType(jsonData);

            if (!MessageDataTypes.ContainsKey(messageType))
            {
                throw new InvalidEnumArgumentException($"Encountered unsupported message type '{messageType}'!");
            }

            Type genericType = MessageDTOTypes[messageType];
            return RecastDeserializedMessage(JsonConvert.DeserializeObject(jsonData, genericType, SerializerSettings));
        }

        private WebMessageDTO<object> RecastDeserializedMessage(object obj)
        {
            MethodInfo method = obj.GetType().GetMethod(nameof(WebMessageDTO<object>.ToObjectWebMessage));
            return (WebMessageDTO<object>) method?.Invoke(obj, null);
        }
    }
}
