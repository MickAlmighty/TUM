using MaterialDesignThemes.Wpf;

namespace Presentation.Model
{
    internal class MDSession : IDialogSession
    {
        private DialogSession Session
        {
            get;
        }

        public MDSession(DialogSession session)
        {
            Session = session;
        }

        public bool IsEnded => Session.IsEnded;

        public object Content => Session.Content;

        public void Close()
        {
            Session.Close();
        }
    }
}