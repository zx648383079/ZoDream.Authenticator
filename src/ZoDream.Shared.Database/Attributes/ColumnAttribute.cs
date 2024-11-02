using System;

namespace ZoDream.Shared.Database
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ColumnAttribute(string name) : Attribute
    {
        public string Name => name;
    }
}
