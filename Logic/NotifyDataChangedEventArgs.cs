using System;
using System.Collections;

namespace Logic
{
    public class NotifyDataChangedEventArgs : EventArgs
    {
        public NotifyDataChangedEventArgs(NotifyDataChangedAction action, IList changedItems)
        {
            Action = action;
            switch (action)
            {
                case NotifyDataChangedAction.Add:
                    NewItems = changedItems;
                    break;
                case NotifyDataChangedAction.Remove:
                    OldItems = changedItems;
                    break;
                case NotifyDataChangedAction.Update:
                    UpdatedItems = changedItems;
                    break;
                case NotifyDataChangedAction.Reset:
                    OldItems = changedItems;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action));
            }
        }
        public NotifyDataChangedEventArgs(NotifyDataChangedAction action, IList newItems, IList oldItems)
        {
            if (action != NotifyDataChangedAction.Replace)
            {
                throw new ArgumentOutOfRangeException(nameof(action));
            }
            Action = action;
            NewItems = newItems;
            OldItems = oldItems;
        }
        public NotifyDataChangedAction Action { get; }
        public IList NewItems { get; }
        public IList UpdatedItems { get; }
        public IList OldItems { get; }
    }
}
