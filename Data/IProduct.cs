using System;

namespace Data
{
    public abstract class IProduct : IUpdatable<IProduct>
    {
        [Id]
        public uint Id { get; protected set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public ProductType ProductType { get; set; }

        protected IProduct(uint id, string name, double price, ProductType productType)
        {
            Id = id;
            Name = name;
            Price = price;
            ProductType = productType;
        }

        public void Update(IProduct product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            if (Id != product.Id)
            {
                throw new ArgumentException(nameof(product));
            }
            Name = product.Name;
            Price = product.Price;
            ProductType = product.ProductType;
        }
    }
}
