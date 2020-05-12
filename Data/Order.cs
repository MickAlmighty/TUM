using System;
using System.Collections.Generic;
using System.Linq;

namespace Data
{
    public sealed class Order : IUpdatable<Order>
    {
        private string _ClientUsername;
        private DateTime _OrderDate;
        private Dictionary<uint, uint> _ProductIdQuantityMap;
        private double _Price;

        public uint Id
        {
            get;
            private set;
        }

        public string ClientUsername
        {
            get
            {
                return _ClientUsername;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(ClientUsername));
                }
                _ClientUsername = value;
            }
        }

        public DateTime OrderDate
        {
            get
            {
                return _OrderDate;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(OrderDate));
                }
                _OrderDate = value;
            }
        }

        public Dictionary<uint, uint> ProductIdQuantityMap
        {
            get
            {
                return _ProductIdQuantityMap;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(ProductIdQuantityMap));
                }
                foreach (uint key in value.Where((key, quantity) => quantity == 0U).Select(pair => pair.Key).ToArray())
                {
                    value.Remove(key);
                }
                if (!value.Any())
                {
                    throw new ArgumentException($"{nameof(ProductIdQuantityMap)} must not be empty!");
                }
                _ProductIdQuantityMap = value;
            }
        }

        public double Price
        {
            get
            {
                return _Price;
            }
            private set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(Price)} must be positive!");
                }
                _Price = value;
            }
        }

        public Order(uint id, Client client, DateTime orderDate, Dictionary<Product, uint> productQuantityMap)
        {
            Id = id;
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            ClientUsername = client.Username;
            OrderDate = orderDate;
            if (productQuantityMap == null)
            {
                throw new ArgumentNullException(nameof(productQuantityMap));
            }
            Dictionary<uint, uint> productIdQuantityMap = new Dictionary<uint, uint>(productQuantityMap.Count);
            foreach (KeyValuePair<Product, uint> pair in productQuantityMap)
            {
                ProductIdQuantityMap.Add(pair.Key.Id, pair.Value);
            }
            ProductIdQuantityMap = productIdQuantityMap;
            Price = productQuantityMap.Select(pair => pair.Key.Price * pair.Value).Sum();
        }

        public Order(uint id, string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap, double price)
        {
            Id = id;
            ClientUsername = clientUsername;
            OrderDate = orderDate;
            ProductIdQuantityMap = productIdQuantityMap;
            Price = price;
        }

        public void Update(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            if (Id != order.Id)
            {
                throw new ArgumentException(nameof(order));
            }
            ClientUsername = order.ClientUsername;
            OrderDate = order.OrderDate;
            ProductIdQuantityMap = order.ProductIdQuantityMap;
            Price = order.Price;
        }
    }
}
