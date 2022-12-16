using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;


namespace Main.Control.Core.Infrastructure
{
    public class CacheBlock
    {
        #region Declarations and Properties
        //private const string CacheName = "UnitWiseCache";
        //public ICacheManager GetCacheManager = CacheFactory.GetCacheManager(CacheName);
        private int CacheExpiry;

        public ObjectCache GetCacheManager
        {
            get { return MemoryCache.Default; }
        }
        #endregion

        #region Constructor
        public CacheBlock()
        {
            int.TryParse(ConfigurationManager.AppSettings["CacheExpiryTime"].ToString(), out CacheExpiry);
        }
        #endregion

        #region Add Item
        /// <summary>
        /// Add the cache items
        /// </summary>
        /// <param name="key">key to add</param>
        /// <param name="value">value to add</param>
        public void Add(string key, object value)
        {
            ////Since Add method is void, not able to give the Add status
            //if (CacheExpiry != 0)
            //{
            //    GetCacheManager.Add(key, value, CacheItemPriority.Normal, null, new SlidingTime(TimeSpan.FromMinutes(CacheExpiry)));
            //}
            //else
            //{
            //    GetCacheManager.Add(key, value);
            //}
            CacheItem _item = new CacheItem(key, value);
            //Since Add method is void, not able to give the Add status
            if (CacheExpiry != 0)
            {
                CacheItemPolicy _policy = new CacheItemPolicy();
                _policy.Priority = CacheItemPriority.Default;
                _policy.SlidingExpiration.Add(TimeSpan.FromMinutes(CacheExpiry));
                GetCacheManager.Add(_item, _policy);
            }
            else
            {
                GetCacheManager.Add(_item, null);
            }
        }
        #endregion

        #region Check Contains
        /// <summary>
        /// To Check the entry in the Cache
        /// </summary>
        /// <param name="key">The key to find in Cache</param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return GetCacheManager.Contains(key);
        }
        #endregion

        #region Get Item
        /// <summary>
        /// To Get the item from Cache
        /// </summary>
        /// <param name="key">Key to get the item from cache</param>
        /// <returns></returns>
        public object Get(string key)
        {
            return GetCacheManager.Get(key);
        }
        #endregion

        #region Remove Item
        /// <summary>
        /// To Remove the item from Cache
        /// </summary>
        /// <param name="key">Key to delete the item from cache</param>
        /// <returns></returns>
        public void Remove(string key)
        {
            GetCacheManager.Remove(key);
        }
        #endregion
    }
}
