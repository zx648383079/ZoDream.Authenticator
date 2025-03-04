using System;
using System.Collections.Generic;

namespace ZoDream.Shared.Database
{
    internal partial class FileBuilder : IComparer<GroupRecord>
    {
        public int Compare(GroupRecord? x, GroupRecord? y)
        {
            if (x is null)
            {
                return 1;
            }
            if (y is null)
            {
                return -1;
            }
            if (x.ParentId == y.ParentId)
            {
                return x.Id - y.Id;
            }
            return x.ParentId - y.ParentId;
        }


    }
}
