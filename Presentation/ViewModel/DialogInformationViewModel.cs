using Presentation.Model;
using System.Windows.Input;

namespace Presentation.ViewModel
{
    /// <summary>
    ///     Simple informating dialog implementation. Should be used with InformationControl.
    /// </summary>
    public class DialogInformationViewModel : DialogBaseViewModel
    {
        public DialogInformationViewModel(IDialogHost dialogHost) : base(dialogHost)
        {
            Close = new RelayCommand(base.CloseDialog);
        }

        public ICommand Close { get; }

        public string Message { get; set; }

        /// <summary>
        ///     Opens the dialog.
        /// </summary>
        /// <param name="message">Message to be shown in the dialog</param>
        public void OpenDialog(string message)
        {
            Message = message;
            Open();
        }

        /// <summary>
        ///     Opens the dialog with a dialogIdentifier.
        /// </summary>
        /// <param name="dialogIdentifier">Dialog identifier object to be used</param>
        /// <param name="message">Message to be shown in the dialog</param>
        public void OpenDialog(object dialogIdentifier, string message)
        {
            Message = message;
            Open(dialogIdentifier);
        }
    }
}
