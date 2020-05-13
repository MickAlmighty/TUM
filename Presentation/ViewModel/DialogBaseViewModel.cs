using Presentation.Model;

namespace Presentation.ViewModel
{
    /// <summary>
    ///     Base for MVVM dialogs
    /// </summary>
    public abstract class DialogBaseViewModel : ViewModelBase
    {
        protected DialogBaseViewModel(IDialogHost dialogHost)
        {
            DialogHost = dialogHost;
        }

        private IDialogHost DialogHost
        {
            get;
        }
        /// <summary>
        ///     Current dialog session.
        /// </summary>
        private IDialogSession DialogSession { get; set; }

        /// <summary>
        ///     Opens the dialog.
        /// </summary>
        protected void Open()
        {
            DialogHost.OpenDialog(this, DialogOpenedEventHandler);
        }

        /// <summary>
        ///     Opens the dialog with a dialogIdentifier.
        /// </summary>
        /// <param name="dialogIdentifier">Dialog identifier object to be used</param>
        protected void Open(object dialogIdentifier)
        {
            DialogHost.OpenDialog(this, dialogIdentifier, DialogOpenedEventHandler);
        }

        /// <summary>
        ///     Closes the dialog if current DialogSession exists and is running.
        /// </summary>
        public void CloseDialog()
        {
            if (DialogSession != null && !DialogSession.IsEnded)
            {
                DialogSession.Close();
                DialogSession = null;
            }
        }

        /// <summary>
        ///     Should be used only as a second argument of DialogHost.Show method.
        ///     Reads the session of an opening dialog in order to be able to close it later.
        /// </summary>
        /// <param name="source">Object that called the event</param>
        /// <param name="args">Event arguments containing DialogSession</param>
        private void DialogOpenedEventHandler(object source, DialogOpenedEventArgs args)
        {
            DialogSession = args.Session;
        }
    }
}
