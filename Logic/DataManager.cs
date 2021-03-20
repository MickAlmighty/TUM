using Data;
using System;
using System.Collections.Generic;
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

        public HashSet<DataType> GetAll()
        {
            return new HashSet<DataType>(DataSet);
        }

        public event NotifyDataChangedEventHandler DataChanged;

        public void ReplaceData(HashSet<DataType> data)
        {
            HashSet<DataType> oldData = DataSet;
            DataSet = new HashSet<DataType>(data);
            DataChanged?.Invoke(this, new NotifyDataChangedEventArgs(NotifyDataChangedAction.Replace, DataSet.ToList(), oldData.ToList()));
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
            DataChanged?.Invoke(this, new NotifyDataChangedEventArgs(NotifyDataChangedAction.Add, new List<DataType> { data }));
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
            DataChanged?.Invoke(this, new NotifyDataChangedEventArgs(NotifyDataChangedAction.Update, new List<DataType> { data }));
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
            DataChanged?.Invoke(this, new NotifyDataChangedEventArgs(NotifyDataChangedAction.Remove, new List<DataType> { data }));
            return true;
        }

        public void Reset()
        {
            List<DataType> oldData = DataSet.ToList();
            DataSet.Clear();
            DataChanged?.Invoke(this, new NotifyDataChangedEventArgs(NotifyDataChangedAction.Reset, oldData));
        }
    }
}
