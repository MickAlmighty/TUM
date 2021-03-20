namespace Presentation.Model
{
    public class DialogOpenedEventArgs
    {
        public DialogOpenedEventArgs(IDialogSession session)
        {
            Session = session;
        }
        public IDialogSession Session { get; }
    }
}