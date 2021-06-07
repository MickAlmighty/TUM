using System;

using Data;

using Logic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicTest
{
    [TestClass]
    public class UpdaterClientTest
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
            Updater.Update(a, b);
            Assert.AreEqual(a.FirstName, b.FirstName);
            Assert.AreEqual(a.LastName, b.LastName);
            Assert.AreEqual(a.Street, b.Street);
            Assert.AreEqual(a.StreetNumber, b.StreetNumber);
            Assert.AreEqual(a.PhoneNumber, b.PhoneNumber);
        }

        [TestMethod]
        public void Update_InvalidId_Throws()
        {
            IClient a = CreateClient(), b = new Client(USERNAME + "1", FIRST_NAME, LAST_NAME, STREET, STREET_NUMBER, PHONE_NUMBER);
            Assert.ThrowsException<ArgumentException>(() => Updater.Update(a, b));
        }

        private class Client : IClient
        {
            public Client(string username, string firstName, string lastName, string street, uint streetNumber, string phoneNumber)
                : base(username, firstName, lastName, street, streetNumber, phoneNumber) { }
        }
    }
}
