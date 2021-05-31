using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DataTest
{
    [TestClass]
    public class OrderTest
    {
        private const uint ID = 1U;
        private const string CLIENT_USERNAME = "Username";
        private readonly DateTime ORDER_DATE = new DateTime(2000, 1, 1);
        private readonly Dictionary<uint, uint> PRODUCT_ID_QUANTITY_MAP = new Dictionary<uint, uint> { { 1U, 3U }, { 2U, 1U } };
        private const double PRICE = 50.0;
        private readonly DateTime? DELIVERY_DATE = null;

        private IOrder CreateOrder()
        {
            return new Order(ID, CLIENT_USERNAME, ORDER_DATE, PRODUCT_ID_QUANTITY_MAP, PRICE, DELIVERY_DATE);
        }

        [TestMethod]
        public void Construction_ValidValues_NoException()
        {
            CreateOrder();
        }

        [TestMethod]
        public void Update_ValidId_UpdatesProperly()
        {
            IOrder a = CreateOrder(), b = new Order(ID, CLIENT_USERNAME + "1", ORDER_DATE.AddDays(1.0), PRODUCT_ID_QUANTITY_MAP, PRICE + 1.0, ORDER_DATE.AddDays(2.0));
            a.Update(b);
            Assert.AreEqual(a.ClientUsername, b.ClientUsername);
            Assert.AreEqual(a.OrderDate, b.OrderDate);
            Assert.AreEqual(a.Price, b.Price);
            Assert.AreEqual(a.DeliveryDate, b.DeliveryDate);
        }

        [TestMethod]
        public void Update_InvalidId_Throws()
        {
            IOrder a = CreateOrder(), b = new Order(ID + 1U, CLIENT_USERNAME, ORDER_DATE, PRODUCT_ID_QUANTITY_MAP, PRICE, DELIVERY_DATE);
            Assert.ThrowsException<ArgumentException>(() => a.Update(b));
        }

        private class Order : IOrder
        {
            public Order(uint id, string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap, double price, DateTime? deliveryDate)
                : base(id, clientUsername, orderDate, productIdQuantityMap, price, deliveryDate) { }
        }
    }
}
