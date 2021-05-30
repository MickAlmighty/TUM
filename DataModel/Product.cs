using Data;

namespace DataModel {
    public class Product : IProduct
    {
        public Product(uint id, string name, double price, ProductType productType) : base(id, name, price, productType) { }
    }
}