using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Resource
{
    public class StreamCacheManager
    {
        private Dictionary<string, StreamCache> dict = new Dictionary<string, StreamCache>();

        private string getDotId(string id)
        {
            return id.Replace("/", ".");
        }

        public bool HasCache(string id)
        {
            var dotId = getDotId(id);
            return dict.ContainsKey(dotId);
        }

        /// <summary>
        /// 缓存数量
        /// </summary>
        public int CacheCount => dict.Count;

        /// <summary>
        /// 获取全部的缓存大小 
        /// </summary>
        /// <returns></returns>
        public long GetTotalCacheSize() => dict.Sum(t => t.Value.ActualSize);

        public void AddCache(string id, StreamCache cache)
        {
            var dotId = getDotId(id);
            lock (dict)
                dict[dotId] = cache;
        }

        public void RemoveCache(string id)
        {
            var dotId = getDotId(id);
            lock (dict)
                if (dict.ContainsKey(dotId))
                    dict.Remove(dotId);
        }

        public StreamCache GetCache(string id)
        {
            var dotId = getDotId(id);
            if (dict.ContainsKey(dotId))
                return dict[dotId];
            return null;
        }
    }
}
