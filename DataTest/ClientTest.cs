using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DataTest
{
    [TestClass]
    public class ClientTest
    {
        private const string USERNAME = "Username", FIRST_NAME = "First name", LAST_NAME = "Last name", STREET = "Street", PHONE_NUMBER = "+20 500 500 500";
        private const uint STREET_NUMBER = 30U;

        private IClient CreateClient()
        {
            return new Client(USERNAME, FIRST_NAME, LAST_NAME, STREET, STREET_NUMBER, PHONE_NUMBER);
        }

        [TestMethod]
        public void Update_ValidId_UpdatesProperly()
        {
            IClient a = CreateClient(), b = new Client(USERNAME, FIRST_NAME + "1", LAST_NAME + "1", STREET + "1", STREET_NUMBER + 1U, "500 500 501");
            a.Update(b);
            Assert.AreEqual(a.FirstName, b.FirstName);
            Assert.AreEqual(a.LastName, b.LastName);
            Assert.AreEqual(a.Street, b.Street);
            Assert.AreEqual(a.StreetNumber, b.StreetNumber);
            Assert.AreEqual(a.PhoneNumber, b.PhoneNumber);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Update_InvalidId_Throws()
        {
            IClient a = CreateClient(), b = new Client(USERNAME + "1", FIRST_NAME, LAST_NAME, STREET, STREET_NUMBER, PHONE_NUMBER);
            a.Update(b);
        }

        private class Client : IClient
        {
            public Client(string username, string firstName, string lastName, string street, uint streetNumber, string phoneNumber)
                : base(username, firstName, lastName, street, streetNumber, phoneNumber) { }
        }
    }
}
