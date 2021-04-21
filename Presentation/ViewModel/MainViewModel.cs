using Data;
using Logic;
using Presentation.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace Presentation.ViewModel
{
    internal class MainViewModel : ViewModelBase
    {
        public MainViewModel(IDialogHost dialogHost, IDataRepository dataRepository, object dialogIdentifier0, object dialogIdentifier1)
        {
            DataRepository = dataRepository;
            DialogHost = dialogHost;
            DialogIdentifier0 = dialogIdentifier0;
            DialogIdentifier1 = dialogIdentifier1;
            DialogClientEditViewModel = new DialogClientEditViewModel(dialogHost, dataRepository);
            DialogOrderEditViewModel = new DialogOrderEditViewModel(dialogHost, dataRepository);
            DialogProductEditViewModel = new DialogProductEditViewModel(dialogHost, dataRepository);
            DialogOrderSentViewModel = new DialogInformationViewModel(dialogHost);
            CreateClient = new RelayCommand(ExecuteCreateClient);
            EditClient = new RelayCommand<Client>(ExecuteEditClient);
            RemoveClient = new RelayCommand<Client>(ExecuteRemoveClient);
            CreateOrder = new RelayCommand(ExecuteCreateOrder);
            EditOrder = new RelayCommand<Order>(ExecuteEditOrder);
            RemoveOrder = new RelayCommand<Order>(ExecuteRemoveOrder);
            CreateProduct = new RelayCommand(ExecuteCreateProduct);
            EditProduct = new RelayCommand<Product>(ExecuteEditProduct);
            RemoveProduct = new RelayCommand<Product>(ExecuteRemoveProduct);
            Clients = new ObservableCollection<Client>(dataRepository.GetAllClients());
            Orders = new ObservableCollection<Order>(dataRepository.GetAllOrders());
            Products = new ObservableCollection<Product>(dataRepository.GetAllProducts());
            DataRepository.ClientsChanged += DataRepository_ClientsChanged;
            DataRepository.OrdersChanged += DataRepository_OrdersChanged;
            DataRepository.ProductsChanged += DataRepository_ProductsChanged;
            DataRepository.OrdersSent += DataRepository_OrdersSent;
        }

        static MainViewModel()
        {
            SyncContext = SynchronizationContext.Current;
        }

        private void DataRepository_OrdersSent(object sender, NotifyOrderSentEventArgs e)
        {
            SyncContext.Post(o => { DialogOrderSentViewModel.OpenDialog(DialogIdentifier1, $"Order {e.Order.Id} of {e.Order.ClientUsername} was successfully delivered on {e.Order.DeliveryDate.Value}!"); }, null);
        }

        private void DataRepository_ClientsChanged(object sender, NotifyDataChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyDataChangedAction.Add:
                    foreach (Client c in e.NewItems)
                    {
                        Clients.Add(c);
                    }
                    break;
                case NotifyDataChangedAction.Remove:
                    foreach (Client c in Clients.Where(client => e.OldItems.Cast<Client>().FirstOrDefault(cl => cl.Username == client.Username) != null).ToList())
                    {
                        Clients.Remove(c);
                    }
                    break;
                case NotifyDataChangedAction.Replace:
                    foreach (Client c in Clients.Where(client => e.OldItems.Cast<Client>().FirstOrDefault(cl => cl.Username == client.Username) != null).ToList())
                    {
                        Clients.Remove(c);
                    }
                    foreach (Client c in e.NewItems)
                    {
                        Clients.Add(c);
                    }
                    break;
                case NotifyDataChangedAction.Reset:
                    Clients.Clear();
                    break;
                case NotifyDataChangedAction.Update:
                    foreach (Client item in e.UpdatedItems)
                    {
                        int index = Clients.IndexOf(Clients.FirstOrDefault(c => c.Username == item.Username));
                        Clients.RemoveAt(index);
                        Clients.Insert(index, item);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void DataRepository_OrdersChanged(object sender, NotifyDataChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyDataChangedAction.Add:
                    foreach (Order o in e.NewItems)
                    {
                        Orders.Add(o);
                    }
                    break;
                case NotifyDataChangedAction.Remove:
                    foreach (Order o in Orders.Where(order => e.OldItems.Cast<Order>().FirstOrDefault(or => or.Id == order.Id) != null).ToList())
                    {
                        Orders.Remove(o);
                    }
                    break;
                case NotifyDataChangedAction.Replace:
                    foreach (Order o in Orders.Where(order => e.OldItems.Cast<Order>().FirstOrDefault(or => or.Id == order.Id) != null).ToList())
                    {
                        Orders.Remove(o);
                    }
                    foreach (Order o in e.NewItems)
                    {
                        Orders.Add(o);
                    }
                    break;
                case NotifyDataChangedAction.Reset:
                    Orders.Clear();
                    break;
                case NotifyDataChangedAction.Update:
                    SyncContext.Post(_ => {
                        foreach (Order item in e.UpdatedItems) {
                            int index = Orders.IndexOf(Orders.FirstOrDefault(o => o.Id == item.Id));
                            Orders.RemoveAt(index);
                            Orders.Insert(index, item);
                        }
                    }, null);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void DataRepository_ProductsChanged(object sender, NotifyDataChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyDataChangedAction.Add:
                    foreach (Product p in e.NewItems)
                    {
                        Products.Add(p);
                    }
                    break;
                case NotifyDataChangedAction.Remove:
                    foreach (Product p in Products.Where(product => e.OldItems.Cast<Product>().FirstOrDefault(pr => pr.Id == product.Id) != null).ToList())
                    {
                        Products.Remove(p);
                    }
                    break;
                case NotifyDataChangedAction.Replace:
                    foreach (Product p in Products.Where(product => e.OldItems.Cast<Product>().FirstOrDefault(pr => pr.Id == product.Id) != null).ToList())
                    {
                        Products.Remove(p);
                    }
                    foreach (Product p in e.NewItems)
                    {
                        Products.Add(p);
                    }
                    break;
                case NotifyDataChangedAction.Reset:
                    Products.Clear();
                    break;
                case NotifyDataChangedAction.Update:
                    foreach (Product item in e.UpdatedItems)
                    {
                        int index = Products.IndexOf(Products.FirstOrDefault(p => p.Id == item.Id));
                        Products.RemoveAt(index);
                        Products.Insert(index, item);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ExecuteCreateClient()
        {
            DialogClientEditViewModel.OpenDialog(DialogIdentifier0);
        }
        private void ExecuteEditClient(Client client)
        {
            DialogClientEditViewModel.OpenDialog(client, DialogIdentifier0);
        }
        private void ExecuteRemoveClient(Client client)
        {
            DataRepository.RemoveClient(client.Username);
        }
        private void ExecuteCreateOrder()
        {
            if (Clients.Any() && Products.Any())
            {
                DialogOrderEditViewModel.OpenDialog(DialogIdentifier0);
            }
        }
        private void ExecuteEditOrder(Order order)
        {
            DialogOrderEditViewModel.OpenDialog(order, DialogIdentifier0);
        }
        private void ExecuteRemoveOrder(Order order)
        {
            DataRepository.RemoveOrder(order.Id);
        }
        private void ExecuteCreateProduct()
        {
            DialogProductEditViewModel.OpenDialog(DialogIdentifier0);
        }
        private void ExecuteEditProduct(Product product)
        {
            DialogProductEditViewModel.OpenDialog(product, DialogIdentifier0);
        }
        private void ExecuteRemoveProduct(Product product)
        {
            DataRepository.RemoveProduct(product.Id);
        }

        public ICommand CreateClient { get; }
        public ICommand EditClient { get; }
        public ICommand RemoveClient { get; }
        public ICommand CreateOrder { get; }
        public ICommand EditOrder { get; }
        public ICommand RemoveOrder { get; }
        public ICommand CreateProduct { get; }
        public ICommand EditProduct { get; }
        public ICommand RemoveProduct { get; }

        public IDataRepository DataRepository
        {
            get;
        }
        public IDialogHost DialogHost
        {
            get;
        }

        public ObservableCollection<Client> Clients
        {
            get;
        }

        public ObservableCollection<Order> Orders
        {
            get;
        }

        public ObservableCollection<Product> Products
        {
            get;
        }

        public object DialogIdentifier0
        {
            get;
        }

        public object DialogIdentifier1
        {
            get;
        }

        private static SynchronizationContext SyncContext { get; }

        public DialogClientEditViewModel DialogClientEditViewModel
        {
            get;
        }

        public DialogOrderEditViewModel DialogOrderEditViewModel
        {
            get;
        }

        public DialogProductEditViewModel DialogProductEditViewModel
        {
            get;
        }

        public DialogInformationViewModel DialogOrderSentViewModel
        {
            get;
        }
    }
}
