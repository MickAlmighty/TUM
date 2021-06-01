using System;

namespace Data {
    public class OnRepositoryClosedEventHandlerArgs : EventArgs
    {
        public OnRepositoryClosedEventHandlerArgs(IDataRepository repository)
        {
            DataRepository = repository;
        }

        public IDataRepository DataRepository { get; }
    }
}