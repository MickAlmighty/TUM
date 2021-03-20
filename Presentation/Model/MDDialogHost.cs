using MaterialDesignThemes.Wpf;
using System.ComponentModel;

namespace Presentation.Model
{
    public class MDDialogHost : IDialogHost
    {
        public void OpenDialog(INotifyPropertyChanged viewModel, DialogOpenedEventHandler dialogOpenedEventHandler)
        {
            DialogHost.Show(viewModel, (object source, MaterialDesignThemes.Wpf.DialogOpenedEventArgs args) =>
            {
                dialogOpenedEventHandler?.Invoke(source, new DialogOpenedEventArgs(new MDSession(args.Session)));
            });
        }

        public void OpenDialog(INotifyPropertyChanged viewModel, object dialogIdentifier, DialogOpenedEventHandler dialogOpenedEventHandler)
        {
            DialogHost.Show(viewModel, dialogIdentifier, (object source, MaterialDesignThemes.Wpf.DialogOpenedEventArgs args) =>
            {
                dialogOpenedEventHandler?.Invoke(source, new DialogOpenedEventArgs(new MDSession(args.Session)));
            });
        }
    }
}
