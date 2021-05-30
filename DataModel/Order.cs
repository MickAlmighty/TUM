using System;
using System.Collections.Generic;

using Data;

namespace DataModel {
    public class Order : IOrder
    {
        public Order(uint id, string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap, double price, DateTime? deliveryDate)
            : base(id, clientUsername, orderDate, productIdQuantityMap, price, deliveryDate) { }
    }
}