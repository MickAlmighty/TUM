using System;

namespace Data
{
    public abstract class IClient : IUpdatable<IClient>
    {
        [Id]
        public string Username { get; protected set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Street { get; set; }

        public uint StreetNumber { get; set; }

        public string PhoneNumber { get; set; }

        protected IClient(string username, string firstName, string lastName, string street, uint streetNumber, string phoneNumber)
        {
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            Street = street;
            StreetNumber = streetNumber;
            PhoneNumber = phoneNumber;
        }

        public void Update(IClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (Username != client.Username)
            {
                throw new ArgumentException(nameof(client));
            }
            FirstName = client.FirstName;
            LastName = client.LastName;
            Street = client.Street;
            StreetNumber = client.StreetNumber;
            PhoneNumber = client.PhoneNumber;
        }
    }
}
