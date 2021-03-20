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

        private Client CreateClient()
        {
            return new Client(USERNAME, FIRST_NAME, LAST_NAME, STREET, STREET_NUMBER, PHONE_NUMBER);
        }

        [TestMethod]
        public void Construction_ValidValues_NoException()
        {
            CreateClient();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Construction_InvalidUsername_Throws()
        {
            new Client("a", FIRST_NAME, LAST_NAME, STREET, STREET_NUMBER, PHONE_NUMBER);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Construction_InvalidFirstName_Throws()
        {
            new Client(USERNAME, "", LAST_NAME, STREET, STREET_NUMBER, PHONE_NUMBER);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Construction_InvalidLastName_Throws()
        {
            new Client(USERNAME, FIRST_NAME, "", STREET, STREET_NUMBER, PHONE_NUMBER);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Construction_InvalidStreet_Throws()
        {
            new Client(USERNAME, FIRST_NAME, LAST_NAME, "", STREET_NUMBER, PHONE_NUMBER);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Construction_InvalidStreetNumber_Throws()
        {
            new Client(USERNAME, FIRST_NAME, LAST_NAME, STREET, 0U, PHONE_NUMBER);
        }
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Construction_InvalidPhoneNumber_Throws()
        {
            new Client(USERNAME, FIRST_NAME, LAST_NAME, STREET, STREET_NUMBER, "50 500 500");
        }

        [TestMethod]
        public void Update_ValidId_UpdatesProperly()
        {
            Client a = CreateClient(), b = new Client(USERNAME, FIRST_NAME + "1", LAST_NAME + "1", STREET + "1", STREET_NUMBER + 1U, "500 500 501");
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
            Client a = CreateClient(), b = new Client(USERNAME + "1", FIRST_NAME, LAST_NAME, STREET, STREET_NUMBER, PHONE_NUMBER);
            a.Update(b);
        }
    }
}
