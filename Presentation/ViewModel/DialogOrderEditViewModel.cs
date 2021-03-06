using Data;
using Presentation.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Presentation.ViewModel
{
    public class DialogOrderEditViewModel : DialogDataEditViewModel<IOrder>
    {
        public class ProductQuantityViewModel : ViewModelBase
        {
            private uint _Quantity;

            public ProductQuantityViewModel(IProduct product, uint quantity)
            {
                Product = product;
                Quantity = quantity;

            }

            public IProduct Product
            {
                get;
            }

            public uint Quantity
            {
                get
                {
                    return _Quantity;
                }
                set
                {
                    _Quantity = value;
                    RaisePropertyChanged();
                }
            }
        }

        private uint _Id;
        private string[] _ClientUsernames;
        private int _ClientUsernameIndex, _ProductIndex;
        private DateTime _OrderDate, _DeliveryDate;
        private bool _Delivered;

        public DialogOrderEditViewModel(IDialogHost dialogHost, ILoadingPresenter loadingPresenter, IDataRepository dataRepository)
            : base(dialogHost, loadingPresenter, dataRepository)
        {
            AddProduct = new RelayCommand<IProduct>(ExecuteAddProduct, CanAddProduct);
            IncrementQuantity = new RelayCommand<ProductQuantityViewModel>(ExecuteIncrementQuantity);
            DecrementQuantity = new RelayCommand<ProductQuantityViewModel>(ExecuteDecrementQuantity);
        }

        public ICommand AddProduct { get; }
        public ICommand IncrementQuantity { get; }
        public ICommand DecrementQuantity { get; }

        private void ExecuteAddProduct(IProduct product)
        {
            ProductQuantities.Add(new ProductQuantityViewModel(product, 1U));
        }

        private bool CanAddProduct(IProduct product)
        {
            return ProductQuantities.FirstOrDefault(pq => pq.Product == product) == null;
        }

        private void ExecuteIncrementQuantity(ProductQuantityViewModel pq)
        {
            ++pq.Quantity;
        }

        private void ExecuteDecrementQuantity(ProductQuantityViewModel pq)
        {
            if (pq.Quantity == 1U)
            {
                ProductQuantities.Remove(pq);
            }
            else
            {
                --pq.Quantity;
            }
        }

        public int ClientUsernameIndex
        {
            get
            {
                return _ClientUsernameIndex;
            }
            set
            {
                _ClientUsernameIndex = value;
                RaisePropertyChanged();
            }
        }

        public bool Delivered
        {
            get
            {
                return _Delivered;
            }
            set
            {
                _Delivered = value;
                RaisePropertyChanged();
            }
        }

        public DateTime OrderDate
        {
            get
            {
                return _OrderDate;
            }
            set
            {
                _OrderDate = value;
                RaisePropertyChanged();
            }
        }

        public DateTime DeliveryDate
        {
            get
            {
                return _DeliveryDate;
            }
            set
            {
                _DeliveryDate = value;
                RaisePropertyChanged();
            }
        }

        public string[] ClientUsernames
        {
            get
            {
                return _ClientUsernames;
            }
            set
            {
                _ClientUsernames = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<ProductQuantityViewModel> ProductQuantities
        {
            get;
        } = new ObservableCollection<ProductQuantityViewModel>();

        public ObservableCollection<IProduct> Products
        {
            get;
        } = new ObservableCollection<IProduct>();

        public int ProductIndex
        {
            get
            {
                return _ProductIndex;
            }
            set
            {
                _ProductIndex = value;
                RaisePropertyChanged();
            }
        }

        protected override bool CanApply()
        {
            // TODO: add ProductIdQuantityMap GUI configuration and adjust CanApply here
            return ProductQuantities.Any() && ClientUsernameIndex != -1 && (!Delivered || DeliveryDate >= OrderDate);
        }

        protected override async void ApplyCreate()
        {
            LoadingPresenter.StartLoading();
            await DataRepository.CreateOrder(ClientUsernames[ClientUsernameIndex], OrderDate, GetProductIdQuantityMap(), Delivered ? DeliveryDate : (DateTime?)null);
            LoadingPresenter.StopLoading();
        }

        protected override async void ApplyEdit()
        {
            LoadingPresenter.StartLoading();
            await DataRepository.UpdateOrder(_Id, ClientUsernames[ClientUsernameIndex], OrderDate, GetProductIdQuantityMap(), GetPrice(), Delivered ? DeliveryDate : (DateTime?)null);
            LoadingPresenter.StopLoading();
        }

        protected override async void InjectProperties(IOrder toUpdate)
        {
            await UpdateDataSets();
            _Id = toUpdate.Id;
            ProductQuantities.Clear();
            foreach (KeyValuePair<uint, uint> pair in toUpdate.ProductIdQuantityMap)
            {
                IProduct product = Products.First(p => p.Id == pair.Key);
                ProductQuantities.Add(new ProductQuantityViewModel(product, pair.Value));
            }
            OrderDate = toUpdate.OrderDate;
            if (toUpdate.DeliveryDate.HasValue)
            {
                DeliveryDate = toUpdate.DeliveryDate.Value;
                Delivered = true;
            }
            else
            {
                DeliveryDate = DateTime.Now;
                Delivered = false;
            }
            ClientUsernameIndex = -1;
            for (int i = 0; i < ClientUsernames.Length; ++i)
            {
                if (ClientUsernames[i] == toUpdate.ClientUsername)
                {
                    ClientUsernameIndex = i;
                    break;
                }
            }
        }

        private double GetPrice()
        {
            double price = 0.0;
            foreach (ProductQuantityViewModel pq in ProductQuantities)
            {
                price += pq.Product.Price * pq.Quantity;
            }
            return price;
        }

        private Dictionary<uint, uint> GetProductIdQuantityMap()
        {
            Dictionary<uint, uint> result = new Dictionary<uint, uint>(ProductQuantities.Count);
            foreach (ProductQuantityViewModel pq in ProductQuantities)
            {
                result.Add(pq.Product.Id, pq.Quantity);
            }
            return result;
        }

        protected override async void ResetProperties()
        {
            await UpdateDataSets();
            OrderDate = DateTime.Now;
            DeliveryDate = DateTime.Now;
            Delivered = false;
            ClientUsernameIndex = 0;
            ProductQuantities.Clear();
        }

        private async Task UpdateDataSets()
        {
            ClientUsernames = (await DataRepository.GetAllClients()).Select(c => c.Username).ToArray();
            Products.Clear();
            foreach (IProduct product in (await DataRepository.GetAllProducts()))
            {
                Products.Add(product);
            }
            ProductIndex = 0;
        }
    }
}
