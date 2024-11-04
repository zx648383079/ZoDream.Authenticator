using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Database
{
    public partial class Database
    {
        public IEnumerable<GroupItem> FetchGroup()
        {
            if (_builder is null)
            {
                return [];
            }
            return _builder.ReadGroup(_header);
        }

        public IEnumerable<EntryItem> FetchEntry()
        {
            if (_builder is null)
            {
                return [];
            }
            return _builder.ReadEntry(_header);
        }

    }
}
