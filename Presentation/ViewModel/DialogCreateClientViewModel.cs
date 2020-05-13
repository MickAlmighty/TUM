using Logic;
using Presentation.Model;
using System.Windows.Input;

namespace Presentation.ViewModel
{
    public class DialogCreateClientViewModel : DialogBaseViewModel
    {
        private string _Username, _FirstName, _LastName, _Street, _StreetNumber, _PhoneNumber;

        public DialogCreateClientViewModel(IDialogHost dialogHost, IDataRepository dataRepository) : base(dialogHost)
        {
            Apply = new RelayCommand(ExecuteApply);
            Cancel = new RelayCommand(ExecuteCancel);
            DataRepository = dataRepository;
        }

        private void ExecuteApply()
        {
            DataRepository.CreateClient(Username, FirstName, LastName, Street, uint.Parse(StreetNumber), PhoneNumber);
            CloseDialog();
        }

        private void ExecuteCancel()
        {
            CloseDialog();
        }

        public ICommand Apply { get; }
        public ICommand Cancel { get; }
        public IDataRepository DataRepository { get; }

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

        public void OpenDialog()
        {
            ResetProperties();
            Open();
        }

        public void OpenDialog(object dialogIdentifier)
        {
            ResetProperties();
            Open(dialogIdentifier);
        }

        public void ResetProperties()
        {

        }

    }
}
