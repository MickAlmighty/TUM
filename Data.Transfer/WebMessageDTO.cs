using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Data.Transfer
{
    public class WebMessageDTO<T>
    {
        public WebMessageDTO() { }

        public WebMessageDTO(WebMessageType messageType, T data)
        {
            MessageType = messageType;
            Data = data;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public WebMessageType MessageType { get; set; }
        public T Data { get; set; }

        public WebMessageDTO<object> ToObjectWebMessage()
        {
            return new WebMessageDTO<object> {
                MessageType = MessageType,
                Data = Data
            };
        }
    }
}
