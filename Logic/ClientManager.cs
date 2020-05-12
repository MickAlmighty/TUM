using Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Logic
{
    public class ClientManager : DataManager<Client, string>
    {
        public ClientManager() : base() { }
        public ClientManager(HashSet<Client> data) : base(data) { }

        public bool Create(string username, string firstName, string lastName, string street, uint streetNumber, string phoneNumber)
        {
            try
            {
                Client client = new Client(username, firstName, lastName, street, streetNumber, phoneNumber);
                return Add(client);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}\n{e.StackTrace}");
                return false;
            }
        }
    }
}
