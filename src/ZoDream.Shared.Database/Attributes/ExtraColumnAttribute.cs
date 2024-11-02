using System;

namespace ZoDream.Shared.Database
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtraColumnAttribute(int index) : Attribute
    {
        public int Index => index;
    }
}
