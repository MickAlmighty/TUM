using System.Collections.Generic;

using Data;

namespace Logic.File
{
    internal class RepositoryDTO
    {
        public HashSet<Client> Clients
        {
            get;
            set;
        }

        public HashSet<Order> Orders
        {
            get;
            set;
        }

        public HashSet<Product> Products
        {
            get;
            set;
        }
    }
}
