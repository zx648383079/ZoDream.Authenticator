using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Database
{
    public partial class Database
    {


        /**
* [4] signature
* [1] version
* [32] md5_encrypt_md5_options
* [4] groupOffset
* [1] groupCount
* [4] entryOffset
* [2] entryCount
* [4] entryDataOffset
* 
* [?] cipher iv
* 
* group
* * [1] parentIndex
* * [1] nameLength
* * [nameLength] name

* 
* entry
* * [1] entryType
* * [1] groupIndex
* * [1] dataCount
* * [1] titleLength
* * [1]? accountLength
* * * [?] dataLength
* * [titleLength] title
* * [accountLength]? account
* * * [dataLength] data
* * * [1] dataType [?] data
*/
        private List<GroupRecord> _groupItems = [];
        private List<EntryRecord> _entryItems = [];
        /// <summary>
        /// 分组id 变更记录
        /// </summary>
        private Dictionary<int, int> _groupMaps = [];
        private int _lastGroupId => _groupItems.Count > 0 ? _groupItems.Last().Id : (GroupRecord.BeginIndex - 1);
        private int _lastEntryId => _entryItems.Count > 0 ? _entryItems.Last().Id : 0;

        private int[] GetGroupId(int id)
        {
            if (id < 1)
            {
                return [];
            }
            if (id >= GroupRecord.BeginIndex)
            {
                return [id];
            }
            var items = new List<int>
            {
                id
            };
            foreach (var item in _groupItems)
            {
                if (item.ParentId == id)
                {
                    items.Add(item.Id);
                }
            }
            return [..items];
        }
    }
}
