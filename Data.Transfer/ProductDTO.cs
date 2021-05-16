using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Data.DTO
{
    public class ProductDTO
    {
        public ProductDTO() { }

        public ProductDTO(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Price = product.Price;
            ProductType = product.ProductType;
        }

        public Product ToProduct()
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
