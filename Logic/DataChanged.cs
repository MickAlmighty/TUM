using System;
using System.Collections.Generic;

using Data;

namespace Logic
{
    public class DataChanged<DataType> where DataType : IUpdatable<DataType>
    {
        public DataChanged(DataChangedAction action, IList<DataType> changedItems)
        {
            Action = action;
            switch (action)
            {
                case DataChangedAction.Add:
                    NewItems = changedItems;
                    break;
                case DataChangedAction.Remove:
                    OldItems = changedItems;
                    break;
                case DataChangedAction.Update:
                    UpdatedItems = changedItems;
                    break;
                case DataChangedAction.Reset:
                    OldItems = changedItems;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action));
            }
        }
        public DataChanged(DataChangedAction action, IList<DataType> newItems, IList<DataType> oldItems)
        {
            if (action != DataChangedAction.Replace)
            {
                throw new ArgumentOutOfRangeException(nameof(action));
            }
            Action = action;
            NewItems = newItems;
            OldItems = oldItems;
        }
        public DataChangedAction Action { get; }
        public IList<DataType> NewItems { get; }
        public IList<DataType> UpdatedItems { get; }
        public IList<DataType> OldItems { get; }
    }
}
