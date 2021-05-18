using Data;
using Logic;
using Presentation.Model;
using System;
using System.Linq;

namespace Presentation.ViewModel
{
    public class DialogProductEditViewModel : DialogDataEditViewModel<Product>
    {
        private uint _Id;
        private string _Name, _Price;
        private int _ProductTypeIndex;

        public DialogProductEditViewModel(IDialogHost dialogHost, IDataRepository dataRepository) : base(dialogHost, dataRepository) { }

        protected override async void ApplyCreate()
        {
            await DataRepository.CreateProduct(Name, double.Parse(Price), ProductTypes[ProductTypeIndex]);
        }

        protected override async void ApplyEdit()
        {
            await DataRepository.Update(new Product(_Id, Name, double.Parse(Price), ProductTypes[ProductTypeIndex]));
        }

        public static ProductType[] ProductTypes
        {
            get;
        } = Enum.GetValues(typeof(ProductType)).Cast<ProductType>().ToArray();

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                RaisePropertyChanged();
            }
        }

        public string Price
        {
            get
            {
                return _Price;
            }
            set
            {
                _Price = value;
                RaisePropertyChanged();
            }
        }

        public int ProductTypeIndex
        {
            get
            {
                return _ProductTypeIndex;
            }
            set
            {
                _ProductTypeIndex = value;
                RaisePropertyChanged();
            }
        }

        protected override void InjectProperties(Product toUpdate)
        {
            _Id = toUpdate.Id;
            Name = toUpdate.Name;
            Price = toUpdate.Price.ToString();
            ProductTypeIndex = (int)toUpdate.ProductType;
        }

        protected override void ResetProperties()
        {
            Name = "";
            Price = "";
            ProductTypeIndex = 0;
        }
    }
}
