using System;

namespace Data
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SkipUpdateAttribute : Attribute
    {
    }
}