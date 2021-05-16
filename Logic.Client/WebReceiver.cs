namespace Logic.Client
{
    interface WebReceiver
    {
        void OnMessage(string message);
        void OnClose();
        void OnError();
    }
}
