namespace Data.Transfer {
    public class ClientDTO {

        public ClientDTO() { }

        public ClientDTO(Client client)
        {
            Username = client.Username;
            FirstName = client.FirstName;
            LastName = client.LastName;
            Street = client.Street;
            StreetNumber = client.StreetNumber;
            PhoneNumber = client.PhoneNumber;
        }

        public Client ToClient()
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
