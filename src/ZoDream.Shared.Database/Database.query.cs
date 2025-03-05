using System;
using System.Collections.Generic;
using System.Linq;

namespace ZoDream.Shared.Database
{
    public partial class Database
    {

        private IEnumerable<GroupRecord> GroupItems => _builder!.Items.Where(i => i is GroupRecord).Select(i => (GroupRecord)i);
        private IEnumerable<EntryRecord> EntryItems => _builder!.Items.Where(i => i is EntryRecord).Select(i => (EntryRecord)i);

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
            foreach (var item in GroupItems)
            {
                if (item.ParentId == id)
                {
                    items.Add(item.Id);
                }
            }
            return [.. items];
        }

        public IEnumerable<T> Fetch<T>()
            where T : IGroupEntity
        {
            foreach (var item in GroupItems)
            {
                var instance = Activator.CreateInstance<T>();
                if (instance is null)
                {
                    yield break;
                }
                instance.Id = item.Id;
                instance.ParentId = item.ParentId;
                _builder?.Read(item, (IGroupEntity)instance);
                yield return instance;
            }
        }

        public void Insert(IGroupEntity data)
        {
            _builder!.Save(data);
        }

        public void Update(IGroupEntity data)
        {
            _builder!.Save(data);
        }

        public void Delete(IGroupEntity data)
        {
            _builder!.Delete(data, 0);
        }

        public IEnumerable<T> Fetch<T>(Func<EntryType, T> createFn)
            where T : IEntryEntity
        {
            return Fetch(0, createFn);
        }
        public IEnumerable<T> Fetch<T>(int groupId, Func<EntryType, T> createFn) where T : IEntryEntity
        {
            return Fetch(0, string.Empty, createFn);
        }
        public IEnumerable<T> Fetch<T>(int groupId, string keywords, Func<EntryType, T> createFn)
          where T : IEntryEntity
        {
            var groups = GetGroupId(groupId);
            foreach (var item in EntryItems)
            {
                var instance = createFn.Invoke(item.Type);
                if (instance is null)
                {
                    continue;
                }
                if (groups.Length > 0 && !groups.Contains(item.GroupId))
                {
                    continue;
                }
                instance.Id = item.Id;
                instance.GroupId = item.GroupId;
                _builder!.Seek(item, true);
                instance.Title = _builder.ReadString(item, item.PropertiesLength[0]);
                if (!string.IsNullOrWhiteSpace(keywords) && !instance.Title.Contains(keywords))
                {
                    continue;
                }
                if (item.HasAccount && instance is IAccountEntryEntity a)
                {
                    a.Account = _builder.ReadString(item, item.PropertiesLength[1]);
                }
                yield return instance;
            }
        }
        public T? SingleEntry<T>(int id)
            where T : IEntryEntity
        {
            var entry = EntryItems.Where(item => item.Id == id).FirstOrDefault();
            if (entry == null)
            {
                return default;
            }
            var instance = Activator.CreateInstance<T>();
            _builder!.Read(entry, (IEntryEntity)instance);
            return instance;
        }
        public string ScalarEntry(int id, string column)
        {
            var entry = EntryItems.Where(item => item.Id == id).FirstOrDefault();
            if (entry == null)
            {
                return string.Empty;
            }
            var names = TypeMapper.EntryPropertyNames(entry.Type);
            var i = Array.IndexOf(names, column);
            if (i < 0)
            {
                return string.Empty;
            }
            var offset = 0;
            for (int j = 0; j < i; j++)
            {
                offset += entry.PropertiesLength[j];
            }
            _builder!.Seek(entry, offset);
            return _builder.ReadString(entry, entry.PropertiesLength[i]);
        }

        public void Insert(IEntryEntity data)
        {
            _builder!.Save(data);
        }

        public void Update(IEntryEntity data)
        {
            _builder!.Save(data);
        }

        public void Delete(params IEntryEntity[] items)
        {
            if (items.Length == 0)
            {
                return;
            }
            _builder!.Delete(items);
        }

    }
}
