using Data;
using System.Collections.Generic;

namespace Logic
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
