using Data;

namespace DataModel.Transfer {
    public class ClientDTO {

        public ClientDTO() { }

        public ClientDTO(IClient client)
        {
            Username = client.Username;
            FirstName = client.FirstName;
            LastName = client.LastName;
            Street = client.Street;
            StreetNumber = client.StreetNumber;
            PhoneNumber = client.PhoneNumber;
        }

        public IClient ToIClient()
        {
            return new Client(Username, FirstName, LastName, Street, StreetNumber, PhoneNumber);
        }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Street { get; set; }

        public uint StreetNumber { get; set; }

        public string PhoneNumber { get; set; }
    }
}
