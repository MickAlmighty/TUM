using Data;

using Logic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicTest
{
    [TestClass]
    public class ProductValidationExtensionsTest
    {
        private const uint ID = 1U;
        private const string NAME = "Product";
        private const double PRICE = 50.0;
        private const ProductType TYPE = ProductType.Toy;

        [TestMethod]
        public void IsValid_ValidProduct_ReturnsTrue()
        {
            Assert.IsTrue(new Product(ID, NAME, PRICE, TYPE).IsValid());
        }

        [TestMethod]
        public void IsValid_InvalidName_ReturnsFalse()
        {
            Assert.IsFalse(new Product(ID, "", PRICE, TYPE).IsValid());
        }

        [TestMethod]
        public void IsValid_InvalidPrice_ReturnsFalse()
        {
            Assert.IsFalse(new Product(ID, NAME, 0.0, TYPE).IsValid());
        }

        private class Product : IProduct
        {
            public Product(uint id, string name, double price, ProductType productType) : base(id, name, price, productType) { }
        }
    }
}
