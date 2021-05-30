using Data;

using Logic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicTest
{
    [TestClass]
    public class ClientValidationExtensionsTest
    {
        private const string USERNAME = "Username", FIRST_NAME = "First name", LAST_NAME = "Last name", STREET = "Street", PHONE_NUMBER = "+20 500 500 500";
        private const uint STREET_NUMBER = 30U;

        [TestMethod]
        public void IsValid_ValidClient_ReturnsTrue()
        {
            Assert.IsTrue(new Client(USERNAME, FIRST_NAME, LAST_NAME, STREET, STREET_NUMBER, PHONE_NUMBER).IsValid());
        }

        [TestMethod]
        public void IsValid_InvalidUsername_ReturnsFalse()
        {
            Assert.IsFalse(new Client("", FIRST_NAME, LAST_NAME, STREET, STREET_NUMBER, PHONE_NUMBER).IsValid());
        }

        [TestMethod]
        public void IsValid_InvalidFirstName_ReturnsFalse()
        {
            Assert.IsFalse(new Client(USERNAME, "", LAST_NAME, STREET, STREET_NUMBER, PHONE_NUMBER).IsValid());
        }

        [TestMethod]
        public void IsValid_InvalidLastName_ReturnsFalse()
        {
            Assert.IsFalse(new Client(USERNAME, FIRST_NAME, "", STREET, STREET_NUMBER, PHONE_NUMBER).IsValid());
        }

        [TestMethod]
        public void IsValid_InvalidStreet_ReturnsFalse()
        {
            Assert.IsFalse(new Client(USERNAME, FIRST_NAME, LAST_NAME, "", STREET_NUMBER, PHONE_NUMBER).IsValid());
        }

        [TestMethod]
        public void IsValid_InvalidStreetNumber_ReturnsFalse()
        {
            Assert.IsFalse(new Client(USERNAME, FIRST_NAME, LAST_NAME, STREET, 0U, PHONE_NUMBER).IsValid());
        }

        [TestMethod]
        public void IsValid_InvalidPhoneNumber_ReturnsFalse()
        {
            Assert.IsFalse(new Client(USERNAME, FIRST_NAME, LAST_NAME, STREET, STREET_NUMBER, "131 000000").IsValid());
        }

        private class Client : IClient
        {
            public Client(string username, string firstName, string lastName, string street, uint streetNumber, string phoneNumber)
                : base(username, firstName, lastName, street, streetNumber, phoneNumber) { }
        }
    }
}
