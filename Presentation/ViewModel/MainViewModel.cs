using Data;

using Presentation.Model;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace Presentation.ViewModel
{
    internal class MainViewModel : ViewModelBase, ILoadingPresenter, IObserver<OrderSent>, IObserver<DataChanged<IClient>>, IObserver<DataChanged<IProduct>>, IObserver<DataChanged<IOrder>>
    {
        public MainViewModel(IDialogHost dialogHost, IDataRepository dataRepository, object dialogIdentifier0, object dialogIdentifier1)
        {
            DataRepository = dataRepository;
            DialogHost = dialogHost;
            DialogIdentifier0 = dialogIdentifier0;
            DialogIdentifier1 = dialogIdentifier1;
            DialogClientEditViewModel = new DialogClientEditViewModel(dialogHost, this, dataRepository);
            DialogOrderEditViewModel = new DialogOrderEditViewModel(dialogHost, this, dataRepository);
            DialogProductEditViewModel = new DialogProductEditViewModel(dialogHost, this, dataRepository);
            DialogOrderSentViewModel = new DialogInformationViewModel(dialogHost);
            Connect = new RelayCommand(ExecuteConnect);
            CreateClient = new RelayCommand(ExecuteCreateClient);
            EditClient = new RelayCommand<IClient>(ExecuteEditClient);
            RemoveClient = new RelayCommand<IClient>(ExecuteRemoveClient);
            CreateOrder = new RelayCommand(ExecuteCreateOrder);
            EditOrder = new RelayCommand<IOrder>(ExecuteEditOrder);
            RemoveOrder = new RelayCommand<IOrder>(ExecuteRemoveOrder);
            CreateProduct = new RelayCommand(ExecuteCreateProduct);
            EditProduct = new RelayCommand<IProduct>(ExecuteEditProduct);
            RemoveProduct = new RelayCommand<IProduct>(ExecuteRemoveProduct);
        }


        static MainViewModel()
        {
            SyncContext = SynchronizationContext.Current;
        }

        private async void ExecuteConnect()
        {
            StartLoading();
            if (!await DataRepository.OpenRepository())
            {
                throw new ApplicationException("Failed to open the data repository!");
            }
            Clients = new ObservableCollection<IClient>(await DataRepository.GetAllClients());
            Orders = new ObservableCollection<IOrder>(await DataRepository.GetAllOrders());
            Products = new ObservableCollection<IProduct>(await DataRepository.GetAllProducts());
            RaisePropertyChanged(nameof(Clients));
            RaisePropertyChanged(nameof(Orders));
            RaisePropertyChanged(nameof(Products));
            OrderSentUnsubscriber = DataRepository.Subscribe((IObserver<OrderSent>)this);
            ClientUnsubscriber = DataRepository.Subscribe((IObserver<DataChanged<IClient>>)this);
            ProductUnsubscriber = DataRepository.Subscribe((IObserver<DataChanged<IProduct>>)this);
            OrderUnsubscriber = DataRepository.Subscribe((IObserver<DataChanged<IOrder>>)this);
            StopLoading();
        }

        private void ExecuteCreateClient()
        {
            DialogClientEditViewModel.OpenDialog(DialogIdentifier0);
        }
        private void ExecuteEditClient(IClient client)
        {
            DialogClientEditViewModel.OpenDialog(client, DialogIdentifier0);
        }
        private async void ExecuteRemoveClient(IClient client)
        {
            StartLoading();
            await DataRepository.RemoveClient(client);
            StopLoading();
        }
        private void ExecuteCreateOrder()
        {
            if (Clients.Any() && Products.Any())
            {
                StartLoading();
                DialogOrderEditViewModel.OpenDialog(DialogIdentifier0);
                StopLoading();
            }
        }
        private void ExecuteEditOrder(IOrder order)
        {
            DialogOrderEditViewModel.OpenDialog(order, DialogIdentifier0);
        }
        private async void ExecuteRemoveOrder(IOrder order)
        {
            StartLoading();
            await DataRepository.RemoveOrder(order);
            StopLoading();
        }
        private void ExecuteCreateProduct()
        {
            DialogProductEditViewModel.OpenDialog(DialogIdentifier0);
        }
        private void ExecuteEditProduct(IProduct product)
        {
            DialogProductEditViewModel.OpenDialog(product, DialogIdentifier0);
        }
        private async void ExecuteRemoveProduct(IProduct product)
        {
            StartLoading();
            await DataRepository.RemoveProduct(product);
            StopLoading();
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

        public bool IsProcessing
        {
            get
            {
                return _IsProcessing;
            }
            set
            {
                _IsProcessing = value;
                RaisePropertyChanged();
            }
        }

        private bool _IsProcessing;

        private uint LoadingCounter
        {
            get;
            set;
        }

        public ObservableCollection<IClient> Clients
        {
            get;
            private set;
        }

        public ObservableCollection<IOrder> Orders
        {
            get;
            private set;
        }

        public ObservableCollection<IProduct> Products
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
            SyncContext.Post(d => {
                IOrder order = Orders.First(o => o.Id == value.Order.Id);
                order.DeliveryDate = value.Order.DeliveryDate;
                int index = Orders.IndexOf(order);
                Orders.RemoveAt(index);
                Orders.Insert(index, order);
                DialogOrderSentViewModel.OpenDialog(DialogIdentifier1, $"Order {value.Order.Id} of {value.Order.ClientUsername} was successfully delivered on {(value.Order.DeliveryDate.HasValue ? value.Order.DeliveryDate.Value.ToString() : "")}!");
            }, null);
        }

        public void OnNext(DataChanged<IClient> value)
        {
            SyncContext.Post(d => {
                switch (value.Action)
                {
                    case DataChangedAction.Add:
                        foreach (IClient c in value.NewItems)
                        {
                            Clients.Add(c);
                        }
                        break;
                    case DataChangedAction.Remove:
                        foreach (IClient c in Clients.Where(client => value.OldItems.FirstOrDefault(cl => cl.Username == client.Username) != null).ToList())
                        {
                            Clients.Remove(c);
                        }
                        break;
                    case DataChangedAction.Replace:
                        foreach (IClient c in Clients.Where(client => value.OldItems.FirstOrDefault(cl => cl.Username == client.Username) != null).ToList())
                        {
                            Clients.Remove(c);
                        }
                        foreach (IClient c in value.NewItems)
                        {
                            Clients.Add(c);
                        }
                        break;
                    case DataChangedAction.Reset:
                        Clients.Clear();
                        break;
                    case DataChangedAction.Update:
                        foreach (IClient item in value.UpdatedItems)
                        {
                            int index = Clients.IndexOf(Clients.FirstOrDefault(c => c.Username == item.Username));
                            Clients.RemoveAt(index);
                            Clients.Insert(index, item);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }, null);
        }

        public void OnNext(DataChanged<IProduct> value)
        {
            SyncContext.Post(d => {
                switch (value.Action)
                {
                    case DataChangedAction.Add:
                        foreach (IProduct p in value.NewItems)
                        {
                            Products.Add(p);
                        }
                        break;
                    case DataChangedAction.Remove:
                        foreach (IProduct p in Products.Where(product => value.OldItems.FirstOrDefault(pr => pr.Id == product.Id) != null).ToList())
                        {
                            Products.Remove(p);
                        }
                        break;
                    case DataChangedAction.Replace:
                        foreach (IProduct p in Products.Where(product => value.OldItems.FirstOrDefault(pr => pr.Id == product.Id) != null).ToList())
                        {
                            Products.Remove(p);
                        }
                        foreach (IProduct p in value.NewItems)
                        {
                            Products.Add(p);
                        }
                        break;
                    case DataChangedAction.Reset:
                        Products.Clear();
                        break;
                    case DataChangedAction.Update:
                        foreach (IProduct item in value.UpdatedItems)
                        {
                            int index = Products.IndexOf(Products.FirstOrDefault(p => p.Id == item.Id));
                            Products.RemoveAt(index);
                            Products.Insert(index, item);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }, null);
        }

        public void OnNext(DataChanged<IOrder> value)
        {
            SyncContext.Post(d => {
                switch (value.Action)
                {
                    case DataChangedAction.Add:
                        foreach (IOrder o in value.NewItems)
                        {
                            Orders.Add(o);
                        }
                        break;
                    case DataChangedAction.Remove:
                        foreach (IOrder o in Orders.Where(order => value.OldItems.FirstOrDefault(or => or.Id == order.Id) != null).ToList())
                        {
                            Orders.Remove(o);
                        }
                        break;
                    case DataChangedAction.Replace:
                        foreach (IOrder o in Orders.Where(order => value.OldItems.FirstOrDefault(or => or.Id == order.Id) != null).ToList())
                        {
                            Orders.Remove(o);
                        }
                        foreach (IOrder o in value.NewItems)
                        {
                            Orders.Add(o);
                        }
                        break;
                    case DataChangedAction.Reset:
                        Orders.Clear();
                        break;
                    case DataChangedAction.Update:
                        SyncContext.Post(_ => {
                            foreach (IOrder item in value.UpdatedItems)
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
            }, null);
        }

        void IObserver<DataChanged<IOrder>>.OnCompleted()
        {
            Console.WriteLine($"{nameof(IOrder)} subscription was completed.");
        }

        void IObserver<DataChanged<IOrder>>.OnError(Exception error)
        {
            Console.WriteLine($"An exception occurred during {nameof(IOrder)} subscription: {error}");
        }

        void IObserver<DataChanged<IProduct>>.OnCompleted()
        {
            Console.WriteLine($"{nameof(IProduct)} subscription was completed.");
        }

        void IObserver<DataChanged<IProduct>>.OnError(Exception error)
        {
            Console.WriteLine($"An exception occurred during {nameof(IProduct)} subscription: {error}");
        }

        void IObserver<DataChanged<IClient>>.OnCompleted()
        {
            Console.WriteLine($"{nameof(IClient)} subscription was completed.");
        }

        void IObserver<DataChanged<IClient>>.OnError(Exception error)
        {
            Console.WriteLine($"An exception occurred during {nameof(IClient)} subscription: {error}");
        }

        void IObserver<OrderSent>.OnCompleted()
        {
            Console.WriteLine($"{nameof(OrderSent)} subscription was completed.");
        }

        void IObserver<OrderSent>.OnError(Exception error)
        {
            Console.WriteLine($"An exception occurred during {nameof(OrderSent)} subscription: {error}");
        }

        public void StartLoading()
        {
            ++LoadingCounter;
            SyncContext.Post(o => IsProcessing = LoadingCounter > 0U, null);
        }

        public void StopLoading()
        {
            if (LoadingCounter > 0U)
            {
                --LoadingCounter;
            }
            SyncContext.Post(o => IsProcessing = LoadingCounter > 0U, null);
        }
    }
}
