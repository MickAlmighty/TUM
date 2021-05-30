using System;

using Data;

using System.Collections.Generic;

using DataModel;

namespace Logic
{
    public class ClientManager : DataManager<IClient, string>
    {
        public ClientManager() { }
        public ClientManager(HashSet<IClient> data) : base(data) { }

        public string Create(string username, string firstName, string lastName, string street, uint streetNumber, string phoneNumber)
        {
            IClient client = new Client(username.Trim(), firstName.Trim(), lastName.Trim(), street.Trim(), streetNumber, phoneNumber.Trim());
            if (!client.IsValid())
            {
                throw new ArgumentException($"Provided {nameof(IClient)} data is invalid!");
            }
            return Add(client);
        }

        public override bool Update(IClient client)
        {
            if (!client.IsValid())
            {
                throw new ArgumentException($"Provided {nameof(IClient)} is invalid!");
            }
            return base.Update(client);
        }
    }
}
