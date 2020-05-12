using Data;
using System;
using System.Diagnostics;

namespace Logic
{
    public class ClientManager : DataManager<Client, string>
    {
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
