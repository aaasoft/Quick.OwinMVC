using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Manager
{
    public abstract class AbstractManager<TManager, TItem>
            where TManager : class, new()
            where TItem : class
    {
        public static TManager Instance = new TManager();

        private Dictionary<string, TItem> itemDict = new Dictionary<string, TItem>();
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="item"></param>
        public virtual void Register(TItem item)
        {
            var key = item.GetType().FullName;
            itemDict[key] = item;
        }

        /// <summary>
        /// 反注册
        /// </summary>
        /// <param name="item"></param>
        public virtual void Unregister(TItem item)
        {
            var key = item.GetType().FullName;
            if (!itemDict.ContainsKey(key))
                return;
            itemDict.Remove(key);
        }


        /// <summary>
        /// 获取所有对象
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TItem> GetItems()
        {
            return itemDict.Values;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="classType"></param>
        /// <returns></returns>
        public virtual TItem GetItem(string classType)
        {
            if (string.IsNullOrEmpty(classType))
                return null;
            if (itemDict.ContainsKey(classType))
                return itemDict[classType];
            return null;
        }

        public virtual TItem GetItem<TItemClass>()
            where TItemClass : TItem
        {
            return GetItem(typeof(TItemClass).FullName);
        }
    }
}
