using System;

using Data;

namespace Logic
{
    public class OrderSent : EventArgs
    {
        public OrderSent(Order order)
        {
            Order = order;
        }

        public Order Order
        {
            get;
        }
    }
}
