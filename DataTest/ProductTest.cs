using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DataTest
{
    [TestClass]
    public class ProductTest
    {
        private const uint ID = 1U;
        private const string NAME = "Product";
        private const double PRICE = 50.0;
        private const ProductType TYPE = ProductType.Toy;

        private Product CreateProduct()
        {
            return new Product(ID, NAME, PRICE, TYPE);
        }

        [TestMethod]
        public void Construction_ValidValues_NoException()
        {
            CreateProduct();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Construction_InvalidName_Throws()
        {
            new Product(ID, "", PRICE, TYPE);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Construction_InvalidPrice_Throws()
        {
            new Product(ID, NAME, 0.0, TYPE);
        }

        [TestMethod]
        public void Update_ValidId_UpdatesProperly()
        {
            Product a = CreateProduct(), b = new Product(ID, NAME + "1", PRICE + 1.0, TYPE + 1);
            a.Update(b);
            Assert.AreEqual(a.Name, b.Name);
            Assert.AreEqual(a.Price, b.Price);
            Assert.AreEqual(a.ProductType, b.ProductType);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Update_InvalidId_Throws()
        {
            Product a = CreateProduct(), b = new Product(ID + 1U, NAME, PRICE, TYPE);
            a.Update(b);
        }
    }
}
