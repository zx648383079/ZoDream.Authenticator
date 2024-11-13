using System;
using System.Collections.Generic;
using System.Linq;

namespace ZoDream.Shared.Database
{
    public partial class Database
    {
        public IEnumerable<T> Fetch<T>()
            where T : IGroupEntity
        {
            foreach (var item in _groupItems)
            {
                var instance = Activator.CreateInstance<T>();
                if (instance is null)
                {
                    yield break;
                }
                instance.Id = item.Id;
                instance.ParentId = item.ParentId;
                _builder?.Seek(_header, item);
                instance.Name = _builder?.ReadString(item.NameLength)?? string.Empty;
                yield return instance;
            }
        }

        public void Insert(IGroupEntity data)
        {
            data.Id = _lastGroupId + 1;
            var pos = _header.GroupOffset;
            if (_groupItems.Count > 0)
            {
                var last = _groupItems.Last();
                pos += last.EntryOffset + last.EntryLength;
            }
            _builder.Seek(pos);
            var item = _builder.Write(_header, data);
            _groupItems.Add(item);
            _header.EntryOffset += item.EntryLength;
            _header.GroupCount++;
            _builder.Write(_header);
        }

        public void Update(IGroupEntity data)
        {
            var i = _groupItems.FindIndex(item => item.Id == data.Id);
            if (i < 0)
            {
                return;
            }
            _builder.Seek(_header.GroupOffset + _groupItems[i].EntryOffset);
            var oldLength = _groupItems[i].EntryLength;
            _groupItems[i] = _builder.Write(_header, data, oldLength);
            var offset = _groupItems[i].EntryLength -  oldLength;
            if (offset == 0)
            {
                return;
            }
            _header.EntryOffset += offset;
            for (var j = i + 1; j < _groupItems.Count; j++)
            {
                _groupItems[j].EntryOffset += offset;
            }
            _builder.Write(_header);
        }

        public void Delete(IGroupEntity data)
        {
            var i = _groupItems.FindIndex(item => item.Id == data.Id);
            if (i < 0)
            {
                return;
            }
            var offset = -_groupItems[i].EntryLength;
            _builder.AddSpace(_header.GroupOffset + _groupItems[i].EntryOffset, offset);
            _header.EntryOffset += offset;
            _groupItems.RemoveAt(i);
            for (var j = i; j < _groupItems.Count; j++)
            {
                _groupItems[j].EntryOffset += offset;
            }
            _builder.Write(_header);
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
            foreach (var item in _entryItems)
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
                _builder.Seek(_header, item);
                instance.Title = _builder.ReadString(item.PropertiesLength[0]);
                if (!string.IsNullOrWhiteSpace(keywords) && !instance.Title.Contains(keywords))
                {
                    continue;
                }
                if (item.HasAccount && instance is IAccountEntryEntity a)
                {
                    a.Account = _builder.ReadString(item.PropertiesLength[1]);
                }
                yield return instance;
            }
        }
        public T SingleEntry<T>(int id)
        {
            var entry = _entryItems.Find(item => item.Id == id);
            if (entry == null)
            {
                return default;
            }
            var instance = Activator.CreateInstance<T>();
            _builder.ReadEntry(_header, entry, instance);
            return instance;
        }
        public string ScalarEntry(int id, string column)
        {
            var entry = _entryItems.Find(item => item.Id == id);
            if (entry == null)
            {
                return string.Empty;
            }
            _builder.Seek(_header, entry);
            var names = TypeMapper.EntryPropertyNames(entry.Type);
            var i = Array.IndexOf(names, column);
            if (i < 0)
            {
                return string.Empty;
            }
            return _builder.ReadString(entry.PropertiesLength[i]);
        }

        public void Insert(object data)
        {
            TypeMapper.SetProperty(data, "Id", _lastEntryId + 1);
            var pos = _header.EntryRealOffset;
            if (_entryItems.Count > 0)
            {
                var last = _entryItems.Last();
                pos += last.EntryOffset + last.EntryLength;
            }
            _builder.Seek(pos);
            _entryItems.Add(_builder.Write(_header, data));
            _header.EntryCount++;
            _builder.Write(_header);
        }

        public void Update(int id, object data)
        {
            var i = _entryItems.FindIndex(item => item.Id == id);
            if (i < 0)
            {
                return;
            }
            _builder.Seek(_header, _entryItems[i], false);
            var oldLength = _entryItems[i].EntryLength;
            _entryItems[i] = _builder.Write(_header, data, oldLength);
            var offset = _entryItems[i].EntryLength - oldLength;
            if (offset == 0)
            {
                return;
            }
            for (var j = i + 1; j < _entryItems.Count; j++)
            {
                _entryItems[j].EntryOffset += offset;
            }
        }

        public void Delete(int id)
        {
            var i = _entryItems.FindIndex(item => item.Id == id);
            if (i < 0)
            {
                return;
            }
            var offset = -_entryItems[i].EntryLength;
            _builder.AddSpace(_header.EntryRealOffset + _entryItems[i].EntryOffset, offset);
            _entryItems.RemoveAt(i);
            _header.EntryCount--;
            for (var j = i; j < _entryItems.Count; j++)
            {
                _entryItems[j].EntryOffset += offset;
            }
            _builder.Write(_header);
        }

    }
}
