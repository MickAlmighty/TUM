using System;

namespace Data
{
    public class Product
    {
        private string _Name;
        private double _Price;

        public uint Id
        {
            get;
            private set;
        }

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(Name));
                }
                if (value.Length == 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(Name)} cannot be empty!");
                }
                _Name = value;
            }
        }

        public double Price
        {
            get
            {
                return _Price;
            }
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(Price)} must be positive!");
                }
                _Price = value;
            }
        }

        public ProductType ProductType
        {
            get;
            set;
        }

        public Product(uint id, string name, double price, ProductType productType)
        {
            Id = id;
            Name = name;
            Price = price;
            ProductType = productType;
        }
    }
}
