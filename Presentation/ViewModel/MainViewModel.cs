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
    internal class MainViewModel : ViewModelBase, IObserver<OrderSent>, IObserver<DataChanged<Client>>, IObserver<DataChanged<Product>>, IObserver<DataChanged<Order>>
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
            Connect = new RelayCommand(ExecuteConnect);
            CreateClient = new RelayCommand(ExecuteCreateClient);
            EditClient = new RelayCommand<Client>(ExecuteEditClient);
            RemoveClient = new RelayCommand<Client>(ExecuteRemoveClient);
            CreateOrder = new RelayCommand(ExecuteCreateOrder);
            EditOrder = new RelayCommand<Order>(ExecuteEditOrder);
            RemoveOrder = new RelayCommand<Order>(ExecuteRemoveOrder);
            CreateProduct = new RelayCommand(ExecuteCreateProduct);
            EditProduct = new RelayCommand<Product>(ExecuteEditProduct);
            RemoveProduct = new RelayCommand<Product>(ExecuteRemoveProduct);
        }


        static MainViewModel()
        {
            SyncContext = SynchronizationContext.Current;
        }

        private async void ExecuteConnect()
        {
            if (!await DataRepository.OpenRepository())
            {
                throw new ApplicationException("Failed to open the data repository!");
            }
            Clients = new ObservableCollection<Client>(await DataRepository.GetAllClients());
            Orders = new ObservableCollection<Order>(await DataRepository.GetAllOrders());
            Products = new ObservableCollection<Product>(await DataRepository.GetAllProducts());
            RaisePropertyChanged(nameof(Clients));
            RaisePropertyChanged(nameof(Orders));
            RaisePropertyChanged(nameof(Products));
            OrderSentUnsubscriber = DataRepository.Subscribe((IObserver<OrderSent>)this);
            ClientUnsubscriber = DataRepository.Subscribe((IObserver<DataChanged<Client>>)this);
            ProductUnsubscriber = DataRepository.Subscribe((IObserver<DataChanged<Product>>)this);
            OrderUnsubscriber = DataRepository.Subscribe((IObserver<DataChanged<Order>>)this);
        }

        private void ExecuteCreateClient()
        {
            DialogClientEditViewModel.OpenDialog(DialogIdentifier0);
        }
        private void ExecuteEditClient(Client client)
        {
            DialogClientEditViewModel.OpenDialog(client, DialogIdentifier0);
        }
        private async void ExecuteRemoveClient(Client client)
        {
            await DataRepository.RemoveClient(client.Username);
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
        private async void ExecuteRemoveOrder(Order order)
        {
            await DataRepository.RemoveOrder(order.Id);
        }
        private void ExecuteCreateProduct()
        {
            DialogProductEditViewModel.OpenDialog(DialogIdentifier0);
        }
        private void ExecuteEditProduct(Product product)
        {
            DialogProductEditViewModel.OpenDialog(product, DialogIdentifier0);
        }
        private async void ExecuteRemoveProduct(Product product)
        {
            await DataRepository.RemoveProduct(product.Id);
        }

        public ICommand Connect { get; }
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
            private set;
        }

        public ObservableCollection<Order> Orders
        {
            get;
            private set;
        }

        public ObservableCollection<Product> Products
        {
            get;
            private set;
        }

        public IDisposable OrderSentUnsubscriber { get; private set; }
        public IDisposable ClientUnsubscriber { get; private set; }
        public IDisposable ProductUnsubscriber { get; private set; }
        public IDisposable OrderUnsubscriber { get; private set; }

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

        public void OnNext(OrderSent value)
        {
            SyncContext.Post(o => { DialogOrderSentViewModel.OpenDialog(DialogIdentifier1, $"Order {value.Order.Id} of {value.Order.ClientUsername} was successfully delivered on {(value.Order.DeliveryDate.HasValue ? value.Order.DeliveryDate.Value.ToString() : "")}!"); }, null);
        }

        public void OnNext(DataChanged<Client> value)
        {
            switch (value.Action)
            {
                case DataChangedAction.Add:
                    foreach (Client c in value.NewItems)
                    {
                        Clients.Add(c);
                    }
                    break;
                case DataChangedAction.Remove:
                    foreach (Client c in Clients.Where(client => value.OldItems.FirstOrDefault(cl => cl.Username == client.Username) != null).ToList())
                    {
                        Clients.Remove(c);
                    }
                    break;
                case DataChangedAction.Replace:
                    foreach (Client c in Clients.Where(client => value.OldItems.FirstOrDefault(cl => cl.Username == client.Username) != null).ToList())
                    {
                        Clients.Remove(c);
                    }
                    foreach (Client c in value.NewItems)
                    {
                        Clients.Add(c);
                    }
                    break;
                case DataChangedAction.Reset:
                    Clients.Clear();
                    break;
                case DataChangedAction.Update:
                    foreach (Client item in value.UpdatedItems)
                    {
                        int index = Clients.IndexOf(Clients.FirstOrDefault(c => c.Username == item.Username));
                        Clients.RemoveAt(index);
                        Clients.Insert(index, item);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnNext(DataChanged<Product> value)
        {
            switch (value.Action)
            {
                case DataChangedAction.Add:
                    foreach (Product p in value.NewItems)
                    {
                        Products.Add(p);
                    }
                    break;
                case DataChangedAction.Remove:
                    foreach (Product p in Products.Where(product => value.OldItems.FirstOrDefault(pr => pr.Id == product.Id) != null).ToList())
                    {
                        Products.Remove(p);
                    }
                    break;
                case DataChangedAction.Replace:
                    foreach (Product p in Products.Where(product => value.OldItems.FirstOrDefault(pr => pr.Id == product.Id) != null).ToList())
                    {
                        Products.Remove(p);
                    }
                    foreach (Product p in value.NewItems)
                    {
                        Products.Add(p);
                    }
                    break;
                case DataChangedAction.Reset:
                    Products.Clear();
                    break;
                case DataChangedAction.Update:
                    foreach (Product item in value.UpdatedItems)
                    {
                        int index = Products.IndexOf(Products.FirstOrDefault(p => p.Id == item.Id));
                        Products.RemoveAt(index);
                        Products.Insert(index, item);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnNext(DataChanged<Order> value)
        {
            switch (value.Action)
            {
                case DataChangedAction.Add:
                    foreach (Order o in value.NewItems)
                    {
                        Orders.Add(o);
                    }
                    break;
                case DataChangedAction.Remove:
                    foreach (Order o in Orders.Where(order => value.OldItems.FirstOrDefault(or => or.Id == order.Id) != null).ToList())
                    {
                        Orders.Remove(o);
                    }
                    break;
                case DataChangedAction.Replace:
                    foreach (Order o in Orders.Where(order => value.OldItems.FirstOrDefault(or => or.Id == order.Id) != null).ToList())
                    {
                        Orders.Remove(o);
                    }
                    foreach (Order o in value.NewItems)
                    {
                        Orders.Add(o);
                    }
                    break;
                case DataChangedAction.Reset:
                    Orders.Clear();
                    break;
                case DataChangedAction.Update:
                    SyncContext.Post(_ => {
                        foreach (Order item in value.UpdatedItems)
                        {
                            int index = Orders.IndexOf(Orders.FirstOrDefault(o => o.Id == item.Id));
                            Orders.RemoveAt(index);
                            Orders.Insert(index, item);
                        }
                    }, null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void IObserver<DataChanged<Order>>.OnCompleted()
        {
            Console.WriteLine($"{nameof(Order)} subscription was completed.");
        }

        void IObserver<DataChanged<Order>>.OnError(Exception error)
        {
            Console.WriteLine($"An exception occurred during {nameof(Order)} subscription: {error}");
        }

        void IObserver<DataChanged<Product>>.OnCompleted()
        {
            Console.WriteLine($"{nameof(Product)} subscription was completed.");
        }

        void IObserver<DataChanged<Product>>.OnError(Exception error)
        {
            Console.WriteLine($"An exception occurred during {nameof(Product)} subscription: {error}");
        }

        void IObserver<DataChanged<Client>>.OnCompleted()
        {
            Console.WriteLine($"{nameof(Client)} subscription was completed.");
        }

        void IObserver<DataChanged<Client>>.OnError(Exception error)
        {
            Console.WriteLine($"An exception occurred during {nameof(Client)} subscription: {error}");
        }

        void IObserver<OrderSent>.OnCompleted()
        {
            Console.WriteLine($"{nameof(OrderSent)} subscription was completed.");
        }

        void IObserver<OrderSent>.OnError(Exception error)
        {
            Console.WriteLine($"An exception occurred during {nameof(OrderSent)} subscription: {error}");
        }
    }
}
