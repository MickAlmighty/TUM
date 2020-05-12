using Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Logic
{
    public class OrderManager : DataManager<Order, uint>
    {
        public bool Create(string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap, double price, DateTime? deliveryDate)
        {
            try
            {
                uint id = 0;
                foreach (uint orderId in DataSet.Select(o => o.Id).OrderBy(i => i))
                {
                    if (orderId == id)
                    {
                        ++id;
                    }
                }
                Order order = new Order(id, clientUsername, orderDate, productIdQuantityMap, price, deliveryDate);
                return Add(order);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}\n{e.StackTrace}");
                return false;
            }
        }
    }
}
