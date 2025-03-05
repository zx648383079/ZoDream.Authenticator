using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        public T? SingleEntry<T>(int id) where T : IEntryEntity;
        public string ScalarEntry(int id, string column);
        /// <summary>
        /// 插入Entry
        /// </summary>
        /// <param name="data"></param>
        public void Insert(IEntryEntity data);
        /// <summary>
        /// 更新Entry
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public void Update(IEntryEntity data);
        /// <summary>
        /// 删除Entry
        /// </summary>
        public void Delete(params IEntryEntity[] items);
        public void Flush();
        public void Open();
        
    }
}
