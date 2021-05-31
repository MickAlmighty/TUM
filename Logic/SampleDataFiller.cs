using System;
using System.Collections.Generic;
using System.Linq;

using Data;

namespace Logic
{
    internal static class SampleDataFiller
    {
        public static void FillData(ClientManager clientManager, ProductManager productManager, OrderManager orderManager)
        {
            clientManager.Create("Jan123", "Jan", "Kowalski", "Sezamkowa", 13U, "+48 501 502 503");
            clientManager.Create("Maciek", "Maciej", "Sklanu", "Testowa", 17U, "+48 504 505 506");
            productManager.Create("Zabawka", 5.5, ProductType.Toy);
            productManager.Create("Batonik", 2.5, ProductType.Food);
            IClient[] clients = clientManager.GetAll().ToArray();
            IProduct[] products = productManager.GetAll().ToArray();
            orderManager.Create(clients[0].Username,
                new DateTime(2021, 3, 14, 15, 9, 2),
                new Dictionary<uint, uint> { { products[0].Id, 1U }, { products[1].Id, 2U } },
                products[0].Price + products[1].Price * 2.0,
                null
            );
        }
    }
}
