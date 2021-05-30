using System;
using System.Collections.Generic;

using Data;

using Logic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicTest
{
    [TestClass]
    public class OrderValidationExtensionsTest
    {
        private const uint ID = 1U;
        private const string CLIENT_USERNAME = "Username";
        private readonly DateTime ORDER_DATE = new DateTime(2000, 1, 1);
        private readonly Dictionary<uint, uint> PRODUCT_ID_QUANTITY_MAP = new Dictionary<uint, uint> { { 1U, 3U }, { 2U, 1U } };
        private const double PRICE = 50.0;
        private readonly DateTime? DELIVERY_DATE = null;

        [TestMethod]
        public void IsValid_ValidOrder_ReturnsTrue()
        {
            Assert.IsTrue(new Order(ID, CLIENT_USERNAME, ORDER_DATE, PRODUCT_ID_QUANTITY_MAP, PRICE, DELIVERY_DATE).IsValid());
        }

        [TestMethod]
        public void IsValid_InvalidClientUsername_ReturnsFalse()
        {
            Assert.IsFalse(new Order(ID, "", ORDER_DATE, PRODUCT_ID_QUANTITY_MAP, PRICE, DELIVERY_DATE).IsValid());
        }

        [TestMethod]
        public void IsValid_InvalidProductIdQuantityMap_ReturnsFalse()
        {
            Assert.IsFalse(new Order(ID, CLIENT_USERNAME, ORDER_DATE, null, PRICE, DELIVERY_DATE).IsValid());
            Assert.IsFalse(new Order(ID, CLIENT_USERNAME, ORDER_DATE, new Dictionary<uint, uint>(), PRICE, DELIVERY_DATE).IsValid());
            Assert.IsFalse(new Order(ID, CLIENT_USERNAME, ORDER_DATE, new Dictionary<uint, uint> { { 2U, 0U }, { 5U, 0U } }, PRICE, DELIVERY_DATE).IsValid());
        }

        [TestMethod]
        public void IsValid_InvalidPrice_ReturnsFalse()
        {
            Assert.IsFalse(new Order(ID, CLIENT_USERNAME, ORDER_DATE, PRODUCT_ID_QUANTITY_MAP, 0.0, DELIVERY_DATE).IsValid());
        }

        [TestMethod]
        public void IsValid_DeliveryDateBeforeOrderDate_ReturnsFalse()
        {
            Assert.IsFalse(new Order(ID, CLIENT_USERNAME, ORDER_DATE, PRODUCT_ID_QUANTITY_MAP, PRICE, ORDER_DATE.AddDays(-1.0)).IsValid());
        }

        private class Order : IOrder
        {
            public Order(uint id, string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap, double price, DateTime? deliveryDate)
                : base(id, clientUsername, orderDate, productIdQuantityMap, price, deliveryDate) { }
        }
    }
}
