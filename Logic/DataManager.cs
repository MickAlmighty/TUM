using Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

namespace Logic
{
    public abstract class DataManager<DataType, KeyType> where DataType : IUpdatable<DataType>
    {
        private PropertyInfo IdProperty
        {
            get;
        }

        protected KeyType GetId(DataType data)
        {
            return (KeyType)IdProperty.GetValue(data);
        }

        protected ObservableCollection<DataType> DataSet
        {
            get;
        }

        protected DataManager()
        {
            IEnumerable<PropertyInfo> idProps = typeof(DataType).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(IdAttribute)));
            if (idProps.Count() != 1)
            {
                throw new ApplicationException($"Data type {typeof(DataType).Name} contains {idProps.Count()} Id properties instead of 1!");
            }
            PropertyInfo property = idProps.First();
            if (property.PropertyType != typeof(KeyType))
            {
                throw new ApplicationException($"Data type {typeof(DataType).Name}'s Id property {property.Name} is of type {property.PropertyType.Name}, expected {typeof(KeyType).Name}!");
            }
            IdProperty = property;
            DataSet = new ObservableCollection<DataType>();
        }

        protected DataManager(HashSet<DataType> data)
        {
            DataSet = new ObservableCollection<DataType>(data);
        }

        public HashSet<DataType> GetAll()
        {
            return new HashSet<DataType>(DataSet);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                DataSet.CollectionChanged += value;
            }
            remove
            {
                DataSet.CollectionChanged -= value;
            }
        }

        public bool Add(DataType data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(typeof(DataType).Name);
            }
            if (DataSet.FirstOrDefault(d => GetId(d).Equals(GetId(data))) != null)
            {
                return false;
            }
            DataSet.Add(data);
            return true;
        }

        public DataType Get(KeyType key)
        {
            return DataSet.FirstOrDefault(d => GetId(d).Equals(key));
        }

        public bool Update(DataType data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(typeof(DataType).Name);
            }
            DataType targetData = DataSet.FirstOrDefault(d => GetId(d).Equals(GetId(data)));
            if (targetData == null)
            {
                return false;
            }
            if (!ReferenceEquals(targetData, data))
            {
                targetData.Update(data);
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
            return true;
        }
    }
}
