using System.Collections.Generic;

using Logic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicTest
{
    [TestClass]
    public class DataValidationTest
    {
        [TestMethod]
        public void IsUsernameValid_ValidValue_ReturnsTrue()
        {
            Assert.IsTrue(DataValidationUtil.IsUsernameValid("Username"));
        }

        [TestMethod]
        public void IsUsernameValid_InvalidValue_ReturnsFalse()
        {
            Assert.IsFalse(DataValidationUtil.IsUsernameValid(""));
        }

        [TestMethod]
        public void IsFirstNameValid_ValidValue_ReturnsTrue()
        {
            Assert.IsTrue(DataValidationUtil.IsFirstNameValid("First name"));
        }

        [TestMethod]
        public void IsFirstNameValid_InvalidValue_ReturnsFalse()
        {
            Assert.IsFalse(DataValidationUtil.IsFirstNameValid(""));
        }

        [TestMethod]
        public void IsLastNameValid_ValidValue_ReturnsTrue()
        {
            Assert.IsTrue(DataValidationUtil.IsLastNameValid("Last name"));
        }

        [TestMethod]
        public void IsLastNameValid_InvalidValue_ReturnsFalse()
        {
            Assert.IsFalse(DataValidationUtil.IsLastNameValid(""));
        }

        [TestMethod]
        public void IsStreetValid_ValidValue_ReturnsTrue()
        {
            Assert.IsTrue(DataValidationUtil.IsStreetValid("Street"));
        }

        [TestMethod]
        public void IsStreetValid_InvalidValue_ReturnsFalse()
        {
            Assert.IsFalse(DataValidationUtil.IsStreetValid(""));
        }

        [TestMethod]
        public void IsStreetNumberValid_ValidValue_ReturnsTrue()
        {
            Assert.IsTrue(DataValidationUtil.IsStreetNumberValid(30U));
        }

        [TestMethod]
        public void IsStreetNumberValid_InvalidValue_ReturnsFalse()
        {
            Assert.IsFalse(DataValidationUtil.IsStreetNumberValid(0U));
        }

        [TestMethod]
        public void IsPhoneNumberValid_ValidValue_ReturnsTrue()
        {
            Assert.IsTrue(DataValidationUtil.IsPhoneNumberValid("+20 500 500 500"));
        }

        [TestMethod]
        public void IsPhoneNumberValid_InvalidValue_ReturnsFalse()
        {
            Assert.IsFalse(DataValidationUtil.IsPhoneNumberValid(""));
        }

        [TestMethod]
        public void IsPriceValid_ValidValue_ReturnsTrue()
        {
            Assert.IsTrue(DataValidationUtil.IsPriceValid(50.0));
        }

        [TestMethod]
        public void IsPriceValid_InvalidValue_ReturnsFalse()
        {
            Assert.IsFalse(DataValidationUtil.IsPriceValid(0.0));
            Assert.IsFalse(DataValidationUtil.IsPriceValid(-10.0));
        }

        [TestMethod]
        public void IsProductIdQuantityMapValid_ValidValue_ReturnsTrue()
        {
            Assert.IsTrue(DataValidationUtil.IsProductIdQuantityMapValid(new Dictionary<uint, uint> { { 1U, 3U }, { 2U, 1U } }));
        }

        [TestMethod]
        public void IsProductIdQuantityMapValid_InvalidValue_ReturnsFalse()
        {
            Assert.IsFalse(DataValidationUtil.IsProductIdQuantityMapValid(null));
            Assert.IsFalse(DataValidationUtil.IsProductIdQuantityMapValid(new Dictionary<uint, uint>()));
            Assert.IsFalse(DataValidationUtil.IsProductIdQuantityMapValid(new Dictionary<uint, uint> { { 1U, 0U }, { 5U, 0U } }));
        }
    }
}
