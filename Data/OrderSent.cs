using System;

namespace Data
{
    public class OrderSent : EventArgs
    {
        public OrderSent(IOrder order)
        {
            Order = order;
        }

        public IOrder Order
        {
            get;
        }
    }
}
