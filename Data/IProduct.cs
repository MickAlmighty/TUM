namespace Data
{
    [Updatable]
    public abstract class IProduct
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
    }
}
