using System;
using System.Collections.Generic;

namespace ZoDream.Shared.Database
{
    public interface IDatabase: IDisposable
    {
        public void Create();
        public IEnumerable<T> Fetch<T>(Func<EntryType, T> createFn)
            where T : IEntryEntity;
        public IEnumerable<T> Fetch<T>(int groupId, Func<EntryType, T> createFn) where T : IEntryEntity;
        public IEnumerable<T> Fetch<T>(int groupId, string keywords, Func<EntryType, T> createFn)
          where T : IEntryEntity;
        public IEnumerable<T> Fetch<T>()
            where T : IGroupEntity;
        /// <summary>
        /// 插入分组
        /// </summary>
        /// <param name="data"></param>
        public void Insert(IGroupEntity data);
        /// <summary>
        /// 更新分组
        /// </summary>
        /// <param name="data"></param>
        public void Update(IGroupEntity data);
        /// <summary>
        /// 删除分组
        /// </summary>
        /// <param name="data"></param>
        public void Delete(IGroupEntity data);

        public T SingleEntry<T>(int id);
        public string ScalarEntry(int id, string column);
        /// <summary>
        /// 插入Entry
        /// </summary>
        /// <param name="data"></param>
        public void Insert(object data);
        /// <summary>
        /// 更新Entry
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public void Update(int id, object data);
        /// <summary>
        /// 删除Entry
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id);
        public void Flush();
        public void Open();
        
    }
}
