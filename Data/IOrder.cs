using System;
using System.Collections.Generic;

namespace Data
{
    [Updatable]
    public abstract class IOrder
    {
        [Id]
        public uint Id { get; protected set; }

        public string ClientUsername { get; set; }

        public DateTime OrderDate { get; set; }

        public Dictionary<uint, uint> ProductIdQuantityMap { get; set; }

        public double Price { get; set; }

        public DateTime? DeliveryDate { get; set; }

        protected IOrder(uint id, string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap, double price, DateTime? deliveryDate)
        {
            Id = id;
            ClientUsername = clientUsername;
            OrderDate = orderDate;
            ProductIdQuantityMap = productIdQuantityMap;
            Price = price;
            DeliveryDate = deliveryDate;
        }
    }
}
