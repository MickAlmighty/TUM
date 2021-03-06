using Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Logic
{
    public abstract class DataManager<DataType, KeyType> : IObservable<DataChanged<DataType>>
    {
        private HashSet<IObserver<DataChanged<DataType>>> Observers
        {
            get;
        } = new HashSet<IObserver<DataChanged<DataType>>>();

        private PropertyInfo IdProperty
        {
            get;
        }

        protected KeyType GetId(DataType data)
        {
            return (KeyType)IdProperty.GetValue(data);
        }

        protected HashSet<DataType> DataSet
        {
            get;
            set;
        }

        protected DataManager(HashSet<DataType> data) : this()
        {
            DataSet = new HashSet<DataType>(data);
            IdProperty = GetIdPropertyInfo();
        }

        protected DataManager()
        {
            DataSet = new HashSet<DataType>();
            IdProperty = GetIdPropertyInfo();
        }

        private PropertyInfo GetIdPropertyInfo()
        {
            IEnumerable<PropertyInfo> idProps = typeof(DataType).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(IdAttribute))).ToList();
            if (idProps.Count() != 1)
            {
                throw new ApplicationException($"Data type {typeof(DataType).Name} contains {idProps.Count()} Id properties instead of 1!");
            }
            PropertyInfo propertyInfo = idProps.First();
            if (propertyInfo.PropertyType != typeof(KeyType))
            {
                throw new ApplicationException($"Data type {typeof(DataType).Name}'s Id property {propertyInfo.Name} is of type {propertyInfo.PropertyType.Name}, expected {typeof(KeyType).Name}!");
            }
            return propertyInfo;
        }

        public HashSet<DataType> GetAll()
        {
            return new HashSet<DataType>(DataSet);
        }

        public void ReplaceData(HashSet<DataType> data)
        {
            HashSet<DataType> oldData = DataSet;
            DataSet = new HashSet<DataType>(data);
            foreach (IObserver<DataChanged<DataType>> observer in Observers.ToList())
            {
                observer.OnNext(new DataChanged<DataType>(DataChangedAction.Replace, DataSet.ToList(), oldData.ToList()));
            }
        }

        public KeyType Add(DataType data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(typeof(DataType).Name);
            }
            KeyType id = GetId(data);
            if (DataSet.FirstOrDefault(d => GetId(d).Equals(id)) != null)
            {
                throw new ArgumentException($"{typeof(DataType)} with ID {id} already exists!");
            }
            DataSet.Add(data);
            foreach (IObserver<DataChanged<DataType>> observer in Observers.ToList())
            {
                observer.OnNext(new DataChanged<DataType>(DataChangedAction.Add, new List<DataType> { data }));
            }
            return id;
        }

        public DataType Get(KeyType key)
        {
            return DataSet.FirstOrDefault(d => GetId(d).Equals(key));
        }

        public virtual bool Update(DataType data)
        {
            if (data == null)
            {
                return false;
                // throw new ArgumentNullException(typeof(DataType).Name);
            }
            KeyType id = GetId(data);
            DataType targetData = DataSet.FirstOrDefault(d => GetId(d).Equals(id));
            if (targetData == null)
            {
                return false;
                // throw new ArgumentException($"{typeof(DataType)} with ID {id} does not exist!");
            }
            if (!ReferenceEquals(targetData, data))
            {
                Updater.Update(targetData, data);
            }
            foreach (IObserver<DataChanged<DataType>> observer in Observers.ToList())
            {
                observer.OnNext(new DataChanged<DataType>(DataChangedAction.Update, new List<DataType> { data }));
            }
            return true;
        }

        public bool Remove(KeyType key)
        {
            DataType data = DataSet.FirstOrDefault(d => GetId(d).Equals(key));
            if (data == null)
            {
                return false;
            }
            DataSet.Remove(data);
            foreach (IObserver<DataChanged<DataType>> observer in Observers.ToList())
            {
                observer.OnNext(new DataChanged<DataType>(DataChangedAction.Remove, new List<DataType> { data }));
            }
            return true;
        }

        public void Reset()
        {
            List<DataType> oldData = DataSet.ToList();
            DataSet.Clear();
            foreach (IObserver<DataChanged<DataType>> observer in Observers.ToList())
            {
                observer.OnNext(new DataChanged<DataType>(DataChangedAction.Reset, oldData));
            }
        }

        public IDisposable Subscribe(IObserver<DataChanged<DataType>> observer)
        {
            Observers.Add(observer);
            return new Unsubscriber<DataChanged<DataType>>(Observers, observer);
        }
    }
}
