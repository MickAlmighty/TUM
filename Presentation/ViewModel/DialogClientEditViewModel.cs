using Data;
using Logic;
using Presentation.Model;

namespace Presentation.ViewModel
{
    public class DialogClientEditViewModel : DialogDataEditViewModel<Client>
    {
        private string _Username, _FirstName, _LastName, _Street, _StreetNumber, _PhoneNumber;

        public DialogClientEditViewModel(IDialogHost dialogHost, IDataRepository dataRepository) : base(dialogHost, dataRepository) { }

        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                _Username = value;
                RaisePropertyChanged();
            }
        }

        public string FirstName
        {
            get
            {
                return _FirstName;
            }
            set
            {
                _FirstName = value;
                RaisePropertyChanged();
            }
        }

        public string LastName
        {
            get
            {
                return _LastName;
            }
            set
            {
                _LastName = value;
                RaisePropertyChanged();
            }
        }

        public string Street
        {
            get
            {
                return _Street;
            }
            set
            {
                _Street = value;
                RaisePropertyChanged();
            }
        }

        public string StreetNumber
        {
            get
            {
                return _StreetNumber;
            }
            set
            {
                _StreetNumber = value;
                RaisePropertyChanged();
            }
        }

        public string PhoneNumber
        {
            get
            {
                return _PhoneNumber;
            }
            set
            {
                _PhoneNumber = value;
                RaisePropertyChanged();
            }
        }

        protected override void ApplyCreate()
        {
            DataRepository.CreateClient(Username, FirstName, LastName, Street, uint.Parse(StreetNumber), PhoneNumber);
        }

        protected override void ApplyEdit()
        {
            DataRepository.Update(new Client(Username, FirstName, LastName, Street, uint.Parse(StreetNumber), PhoneNumber));
        }

        protected override void InjectProperties(Client toUpdate)
        {
            Username = toUpdate.Username;
            FirstName = toUpdate.FirstName;
            LastName = toUpdate.LastName;
            Street = toUpdate.Street;
            StreetNumber = toUpdate.StreetNumber.ToString();
            PhoneNumber = toUpdate.PhoneNumber;
        }

        protected override void ResetProperties()
        {
            Username = "";
            FirstName = "";
            LastName = "";
            Street = "";
            StreetNumber = "";
            PhoneNumber = "";
        }
    }
}
