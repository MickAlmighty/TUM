using System;
using System.Collections.Generic;

namespace Data.Transfer
{
    public class OrderDTO
    {
        public OrderDTO() { }

        public OrderDTO(Order order)
        {
            Id = order.Id;
            ClientUsername = order.ClientUsername;
            OrderDate = order.OrderDate;
            ProductIdQuantityMap = order.ProductIdQuantityMap;
            Price = order.Price;
            DeliveryDate = order.DeliveryDate;
        }

        public Order ToOrder()
        {
            return new Order(Id, ClientUsername, OrderDate, ProductIdQuantityMap, Price, DeliveryDate);
        }

        public uint Id { get; set; }

        public string ClientUsername { get; set; }

        public DateTime OrderDate { get; set; }

        public Dictionary<uint, uint> ProductIdQuantityMap { get; set; }

        public double Price { get; set; }

        public DateTime? DeliveryDate { get; set; }
    }
}
