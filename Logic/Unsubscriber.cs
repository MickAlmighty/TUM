using System;
using System.Collections.Generic;

namespace Logic
{
    public class Unsubscriber<T> : IDisposable
    {
        private HashSet<IObserver<T>> Observers { get; }
        private IObserver<T> Observer { get; }

        public Unsubscriber(HashSet<IObserver<T>> observers, IObserver<T> observer)
        {
            Observers = observers;
            Observer = observer;
        }

        public void Dispose()
        {
            Observers.Remove(Observer);
            Observer.OnCompleted();
        }
    }
}
