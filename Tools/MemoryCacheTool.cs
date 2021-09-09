using System;
using System.Runtime.Caching;

namespace NetLibrary.Tools
{
    /// <summary>
    /// 暫存記憶體工具
    /// </summary>
    public class MemoryCacheTool
    {
        /// <summary>
        /// 將資料存入暫存記憶體
        /// </summary>
        /// <typeparam name="T">資料格式</typeparam>
        /// <param name="CacheID">識別名稱</param>
        /// <param name="CacheData">資料</param>
        /// <param name="CacheMinutes">暫存時間(分)，預設為5分鐘</param>
        public void SetCacheData<T>(string CacheID, T CacheData, int CacheMinutes = 5)
        {
            ObjectCache _cache = MemoryCache.Default;
            _cache.Set(CacheID, CacheData, new DateTimeOffset(DateTime.Now.AddMinutes(CacheMinutes)));
        }

        /// <summary>
        /// 自暫存記憶體取出資料
        /// </summary>
        /// <typeparam name="T">資料格式</typeparam>
        /// <param name="CacheID">識別名稱</param>
        public T GetCacheData<T>(string CacheID)
        {
            ObjectCache _cache = MemoryCache.Default;
            T _ReturnObject = _cache[CacheID] == null ? default(T) : (T)_cache[CacheID];
            return _ReturnObject;
        }
    }
}
