using System.Collections.Generic;

using DataModel.Transfer;

namespace Logic.File
{
    internal class RepositoryDTO
    {
        public List<ClientDTO> Clients
        {
            get;
            set;
        }

        public List<OrderDTO> Orders
        {
            get;
            set;
        }

        public List<ProductDTO> Products
        {
            get;
            set;
        }
    }
}
