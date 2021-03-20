using System.ComponentModel;

namespace Presentation.Model
{
    public interface IDialogHost
    {
        void OpenDialog(INotifyPropertyChanged viewModel, DialogOpenedEventHandler dialogOpenedEventHandler);
        void OpenDialog(INotifyPropertyChanged viewModel, object dialogIdentifier, DialogOpenedEventHandler dialogOpenedEventHandler);
    }
}
