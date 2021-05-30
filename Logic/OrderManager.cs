using Data;

using System;
using System.Collections.Generic;
using System.Linq;

using DataModel;

namespace Logic
{
    public class OrderManager : DataManager<IOrder, uint>
    {
        public OrderManager() { }
        public OrderManager(HashSet<IOrder> data) : base(data) { }

        public uint Create(string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap, double price, DateTime? deliveryDate)
        {
            uint id = 0;
            foreach (uint orderId in DataSet.Select(o => o.Id).OrderBy(i => i))
            {
                if (orderId == id)
                {
                    ++id;
                }
            }
            IOrder order = new Order(id, clientUsername.Trim(), orderDate, productIdQuantityMap, price, deliveryDate);
            if (!order.IsValid())
            {
                throw new ArgumentException($"Provided {nameof(IOrder)} data is invalid!");
            }
            return Add(order);
        }

        public override bool Update(IOrder order)
        {
            if (!order.IsValid())
            {
                throw new ArgumentException($"Provided {nameof(IOrder)} is invalid!");
            }
            return base.Update(order);
        }
    }
}
