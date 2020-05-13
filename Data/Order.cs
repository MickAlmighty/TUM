using System;
using System.Collections.Generic;
using System.Linq;

namespace Data
{
    public sealed class Order : IUpdatable<Order>
    {
        private string _ClientUsername;
        private DateTime? _DeliveryDate;
        private Dictionary<uint, uint> _ProductIdQuantityMap;
        private double _Price;

        [Id]
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
            get;
            set;
        }

        public Dictionary<uint, uint> ProductIdQuantityMap
        {
            get
            {
                return _ProductIdQuantityMap;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(ProductIdQuantityMap));
                }
                foreach (uint key in value.Where(pair => pair.Value == 0U).Select(pair => pair.Key).ToArray())
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
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(Price)} must be positive!");
                }
                _Price = value;
            }
        }

        public DateTime? DeliveryDate
        {
            get
            {
                return _DeliveryDate;
            }
            set
            {
                if (value.HasValue)
                {
                    if (value < OrderDate)
                    {
                        throw new ArgumentException($"{nameof(OrderDate)} must not be greater than {nameof(DeliveryDate)}!");
                    }
                }
                _DeliveryDate = value;
            }
        }

        public Order(uint id, string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap, double price, DateTime? deliveryDate)
        {
            Id = id;
            ClientUsername = clientUsername;
            OrderDate = orderDate;
            ProductIdQuantityMap = productIdQuantityMap;
            Price = price;
            DeliveryDate = deliveryDate;
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
            DeliveryDate = order.DeliveryDate;
        }
    }
}
