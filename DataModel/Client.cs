using Data;

namespace DataModel {
    public sealed class Client : IClient
    {
        public Client(string username, string firstName, string lastName, string street, uint streetNumber, string phoneNumber)
            : base(username, firstName, lastName, street, streetNumber, phoneNumber) { }
    }
}