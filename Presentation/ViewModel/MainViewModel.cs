using Logic;
using Presentation.Model;
using System.Windows.Input;

namespace Presentation.ViewModel
{
    internal class MainViewModel : ViewModelBase
    {
        public MainViewModel(IDialogHost dialogHost, IDataRepository dataRepository)
        {
            DataRepository = dataRepository;
            DialogHost = dialogHost;
            DialogCreateClientViewModel = new DialogClientEditViewModel(dialogHost, dataRepository);
            CreateClient = new RelayCommand(ExecuteCreateClient);
        }

        private void ExecuteCreateClient()
        {
            DialogCreateClientViewModel.OpenDialog();
        }

        public ICommand CreateClient { get; }

        public IDataRepository DataRepository
        {
            get;
        }
        public IDialogHost DialogHost
        {
            get;
        }

        public DialogClientEditViewModel DialogCreateClientViewModel
        {
            get;
        }
    }
}
