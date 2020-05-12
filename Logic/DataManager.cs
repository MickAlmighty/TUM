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
        private ObservableCollection<DataType> _DataSet;

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
            get
            {
                return _DataSet;
            }
            set
            {
                if (_DataSet != null)
                {
                    _DataSet.CollectionChanged -= DataSet_CollectionChanged;
                }
                if (value != null)
                {
                    value.CollectionChanged += DataSet_CollectionChanged;
                }
                _DataSet = value;
            }
        }

        protected DataManager(HashSet<DataType> data) : this()
        {
            DataSet = new ObservableCollection<DataType>(data);
            IdProperty = GetIdPropertyInfo();
        }

        protected DataManager()
        {
            DataSet = new ObservableCollection<DataType>();
            IdProperty = GetIdPropertyInfo();
        }

        private PropertyInfo GetIdPropertyInfo()
        {
            IEnumerable<PropertyInfo> idProps = typeof(DataType).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(IdAttribute)));
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

        private void DataSet_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(sender, e);
        }

        public HashSet<DataType> GetAll()
        {
            return new HashSet<DataType>(DataSet);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void ReplaceData(HashSet<DataType> data)
        {
            DataSet = new ObservableCollection<DataType>(data);
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
