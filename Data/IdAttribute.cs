using System;

namespace Data
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IdAttribute : Attribute
    {
        public IdAttribute() { }
    }
}
