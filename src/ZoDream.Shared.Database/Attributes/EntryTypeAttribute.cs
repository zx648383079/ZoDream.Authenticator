using System;

namespace ZoDream.Shared.Database
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EntryAttribute(EntryType type) : Attribute
    {
        public EntryType Type => type;
    }
}
