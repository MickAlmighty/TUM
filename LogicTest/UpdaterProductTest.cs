using System;

using Data;

using Logic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicTest
{
    [TestClass]
    public class UpdaterProductTest
    {
        private const uint ID = 1U;
        private const string NAME = "Product";
        private const double PRICE = 50.0;
        private const ProductType TYPE = ProductType.Toy;

        private IProduct CreateProduct()
        {
            return new Product(ID, NAME, PRICE, TYPE);
        }

        [TestMethod]
        public void Update_ValidId_UpdatesProperly()
        {
            IProduct a = CreateProduct(), b = new Product(ID, NAME + "1", PRICE + 1.0, TYPE + 1);
            Updater.Update(a, b);
            Assert.AreEqual(a.Name, b.Name);
            Assert.AreEqual(a.Price, b.Price);
            Assert.AreEqual(a.ProductType, b.ProductType);
        }

        [TestMethod]
        public void Update_InvalidId_Throws()
        {
            IProduct a = CreateProduct(), b = new Product(ID + 1U, NAME, PRICE, TYPE);
            Assert.ThrowsException<ArgumentException>(() => Updater.Update(a, b));
        }

        private class Product : IProduct
        {
            public Product(uint id, string name, double price, ProductType productType) : base(id, name, price, productType) { }
        }
    }
}
