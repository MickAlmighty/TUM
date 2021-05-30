using Data;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DataModel.Transfer
{
    public class ProductDTO
    {
        public ProductDTO() { }

        public ProductDTO(IProduct product)
        {
            Id = product.Id;
            Name = product.Name;
            Price = product.Price;
            ProductType = product.ProductType;
        }

        public IProduct ToIProduct()
        {
            return new Product(Id, Name, Price, ProductType);
        }

        public uint Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ProductType ProductType { get; set; }
    }
}
