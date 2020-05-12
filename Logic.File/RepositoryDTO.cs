using Data;
using System.Collections.Generic;

namespace Logic
{
    internal class RepositoryDTO
    {
        public List<Client> Clients
        {
            get;
            set;
        }

        public List<Order> Orders
        {
            get;
            set;
        }

        public List<Product> Products
        {
            get;
            set;
        }
    }
}
