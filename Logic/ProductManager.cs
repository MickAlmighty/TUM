using Data;

using System;
using System.Collections.Generic;
using System.Linq;

using DataModel;

namespace Logic
{
    public class ProductManager : DataManager<IProduct, uint>
    {
        public ProductManager() { }
        public ProductManager(HashSet<IProduct> data) : base(data) { }

        public uint Create(string name, double price, ProductType productType)
        {
            uint id = 0;
            foreach (uint productId in DataSet.Select(p => p.Id).OrderBy(i => i))
            {
                if (productId == id)
                {
                    ++id;
                }
            }
            IProduct product = new Product(id, name.Trim(), price, productType);
            if (!product.IsValid())
            {
                throw new ArgumentException($"Provided {nameof(IProduct)} data is invalid!");
            }
            return Add(product);
        }

        public override bool Update(IProduct product)
        {
            if (!product.IsValid())
            {
                throw new ArgumentException($"Provided {nameof(IProduct)} is invalid!");
            }
            return base.Update(product);
        }
    }
}
