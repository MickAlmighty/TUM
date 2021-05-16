using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;

using Newtonsoft.Json;

using JsonException = System.Text.Json.JsonException;

namespace Data.Transfer
{
    public sealed class WebSerializer
    {
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

        private string SerializeWebMessage<T>(WebMessageType messageType, object data)
        {
            if (!(data is T validData))
            {
                throw new ArgumentException($"Invalid data type! Expected {typeof(T)}, got {data.GetType()}.");
            }

            return JsonConvert.SerializeObject(
                new WebMessageDTO<T> { MessageType = messageType, Data = validData }, SerializerSettings);
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
            switch (messageType)
            {
                case WebMessageType.AddClient:
                case WebMessageType.UpdateClient:
                case WebMessageType.ProvideClient:
                case WebMessageType.RemoveClient:
                    return SerializeWebMessage<ClientDTO>(messageType, data);
                case WebMessageType.AddProduct:
                case WebMessageType.UpdateProduct:
                case WebMessageType.ProvideProduct:
                case WebMessageType.RemoveProduct:
                    return SerializeWebMessage<ProductDTO>(messageType, data);
                case WebMessageType.AddOrder:
                case WebMessageType.UpdateOrder:
                case WebMessageType.ProvideOrder:
                case WebMessageType.RemoveOrder:
                case WebMessageType.OrderSent:
                    return SerializeWebMessage<OrderDTO>(messageType, data);
                case WebMessageType.ProvideAllClients:
                    return SerializeWebMessage<HashSet<ClientDTO>>(messageType, data);
                case WebMessageType.ProvideAllProducts:
                    return SerializeWebMessage<HashSet<ProductDTO>>(messageType, data);
                case WebMessageType.ProvideAllOrders:
                    return SerializeWebMessage<HashSet<OrderDTO>>(messageType, data);
                case WebMessageType.GetProduct:
                case WebMessageType.GetOrder:
                    return SerializeWebMessage<uint>(messageType, data);
                case WebMessageType.GetClient:
                case WebMessageType.Error:
                    return SerializeWebMessage<string>(messageType, data);
                default:
                    throw new InvalidEnumArgumentException($"Encountered unsupported message type '{messageType}'!");
            }
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
            WebMessageType messageType = GetMessageType(jsonData);
            switch (messageType)
            {
                case WebMessageType.AddClient:
                case WebMessageType.UpdateClient:
                case WebMessageType.ProvideClient:
                case WebMessageType.RemoveClient:
                    return JsonConvert.DeserializeObject<WebMessageDTO<ClientDTO>>(jsonData, SerializerSettings).ToObjectWebMessage();
                case WebMessageType.AddProduct:
                case WebMessageType.UpdateProduct:
                case WebMessageType.ProvideProduct:
                case WebMessageType.RemoveProduct:
                    return JsonConvert.DeserializeObject<WebMessageDTO<ProductDTO>>(jsonData, SerializerSettings).ToObjectWebMessage();
                case WebMessageType.AddOrder:
                case WebMessageType.UpdateOrder:
                case WebMessageType.ProvideOrder:
                case WebMessageType.RemoveOrder:
                case WebMessageType.OrderSent:
                    return JsonConvert.DeserializeObject<WebMessageDTO<OrderDTO>>(jsonData, SerializerSettings).ToObjectWebMessage();
                case WebMessageType.ProvideAllClients:
                    return JsonConvert.DeserializeObject<WebMessageDTO<HashSet<ClientDTO>>>(jsonData, SerializerSettings).ToObjectWebMessage();
                case WebMessageType.ProvideAllProducts:
                    return JsonConvert.DeserializeObject<WebMessageDTO<HashSet<ProductDTO>>>(jsonData, SerializerSettings).ToObjectWebMessage();
                case WebMessageType.ProvideAllOrders:
                    return JsonConvert.DeserializeObject<WebMessageDTO<HashSet<OrderDTO>>>(jsonData, SerializerSettings).ToObjectWebMessage();
                case WebMessageType.GetProduct:
                case WebMessageType.GetOrder:
                    return JsonConvert.DeserializeObject<WebMessageDTO<uint>>(jsonData, SerializerSettings).ToObjectWebMessage();
                case WebMessageType.GetClient:
                case WebMessageType.Error:
                    return JsonConvert.DeserializeObject<WebMessageDTO<string>>(jsonData, SerializerSettings).ToObjectWebMessage();
                default:
                    throw new InvalidEnumArgumentException($"Encountered unsupported message type '{messageType}'!");
            }
        }
    }
}
