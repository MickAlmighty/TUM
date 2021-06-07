using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Data;

namespace Logic
{
    public static class Updater
    {
        public static void Update<T>(T to, T from)
        {
            if (to == null)
            {
                throw new ArgumentException("Target object cannot be null.");
            }
            if (from == null)
            {
                throw new ArgumentException("Source object cannot be null.");
            }
            Type type = typeof(T);
            if (Attribute.GetCustomAttribute(type, typeof(UpdatableAttribute)) == null)
            {
                throw new ArgumentException($"Data type {type.Name} is not marked with {nameof(UpdatableAttribute)}!");
            }
            List<PropertyInfo> props = type.GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(IdAttribute))).ToList();
            if (props.Count != 1)
            {
                throw new ArgumentException($"Data type {type.Name} contains {props.Count} Id properties instead of 1!");
            }
            PropertyInfo idProp = props.First();
            if (!idProp.GetValue(to).Equals(idProp.GetValue(from)))
            {
                throw new ArgumentException($"Provided {type.Name} instances have different IDs ({idProp.GetValue(to)} != {idProp.GetValue(from)})!");
            }
            props = type.GetProperties().Where(prop => !Attribute.IsDefined(prop, typeof(SkipUpdateAttribute))).ToList();
            props.Remove(idProp);
            foreach (PropertyInfo prop in props)
            {
                prop.SetValue(to, prop.GetValue(from));
            }
        }
    }
}
