using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TinaX.VFSKit
{
    /// <summary>
    /// 已被加载的AssetBundle
    /// </summary>
    sealed class LoadedAssetBundle
    {


#pragma warning disable IDE1006 
        public AssetBundle mAssetBundle { get; private set; }
#pragma warning restore IDE1006 

        public Hash128 AssetBundleHashCode { get; private set; }
        //public bool IsFromStreamingAssetsPath { get; private set; }
        
        //public bool IsFromWeb { get; private set; }

        public string PureAssetBundlePath { get; private set; } //纯地址，以assets/xxx开头

        public string FullAssetBundlePath { get; private set; }

        public string[] Dependences { get; private set; }

        /// <summary>
        /// 引用计数
        /// </summary>
        public int UseNum { get { return mUseNum; } }

        private int mUseNum = 0;    //使用计数

        public LoadedAssetBundle(AssetBundle assetBundle,Hash128 abHash,string purePath,string fullPath,string[] dependences)
        {
            mAssetBundle = assetBundle;
            AssetBundleHashCode = abHash;
            PureAssetBundlePath = purePath;
            FullAssetBundlePath = fullPath;
            Dependences = dependences;
        }


        public void Register_Use()
        {
            mUseNum++;
        }

        public void Remove_Use()
        {
            mUseNum--;
        }

        /// <summary>
        /// 覆盖引用数量
        /// </summary>
        public void Cover_UseNum(int num)
        {
            mUseNum = num;
        }


        

    }


    sealed class LoadedAssetBundleMgr
    {
        private readonly Dictionary<string, LoadedAssetBundle> mLoadedAssetBundleCache = new Dictionary<string, LoadedAssetBundle>(); //key为ab包相对于Unity的路径（Assets/xxxx）

        public bool ContainsPath(string abPath)
        {
            return mLoadedAssetBundleCache.ContainsKey(abPath);
        }

        public LoadedAssetBundle GetDataByPath(string abPath)
        {
            return mLoadedAssetBundleCache[abPath];
        }

        public void RemoveByKey(string abPath)
        {
            mLoadedAssetBundleCache.Remove(abPath);
        }

        public void Add(string abPath,LoadedAssetBundle data)
        {
            if (!mLoadedAssetBundleCache.ContainsKey(abPath))
            {
                mLoadedAssetBundleCache.Add(abPath, data);
            }
        }

        public void GC()
        {
            List<string> Keys = new List<string>();
            foreach(var item in mLoadedAssetBundleCache)
            {
                if(item.Value.UseNum <= 0)
                {
                    item.Value.mAssetBundle.Unload(true);
                    Keys.Add(item.Key);
                }
            }

            foreach(var item in Keys)
            {
                if (mLoadedAssetBundleCache.ContainsKey(item))
                {
                    mLoadedAssetBundleCache.Remove(item);
                }
            }
        }

    }
}

