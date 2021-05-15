using Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Logic
{
    public class ProductManager : DataManager<Product, uint>
    {
        public ProductManager() { }
        public ProductManager(HashSet<Product> data) : base(data) { }

        public bool Create(string name, double price, ProductType productType)
        {
            try
            {
                uint id = 0;
                foreach (uint productId in DataSet.Select(p => p.Id).OrderBy(i => i))
                {
                    if (productId == id)
                    {
                        ++id;
                    }
                }
                Product product = new Product(id, name, price, productType);
                return Add(product);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}\n{e.StackTrace}");
                return false;
            }
        }
    }
}
