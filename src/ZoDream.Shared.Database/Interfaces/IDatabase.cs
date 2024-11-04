using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZoDream.Shared.Database
{
    public interface IDatabase: IDisposable
    {
        public void Create();
        public IEnumerable<EntryItem> FetchEntry();
        public IEnumerable<GroupItem> FetchGroup();
        public void Open();
    }
}
