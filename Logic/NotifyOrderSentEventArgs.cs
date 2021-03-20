using Data;
using System;

namespace Logic
{
    public class NotifyOrderSentEventArgs : EventArgs
    {
        public NotifyOrderSentEventArgs(Order order)
        {
            Order = order;
        }

        public Order Order
        {
            get;
        }
    }
}