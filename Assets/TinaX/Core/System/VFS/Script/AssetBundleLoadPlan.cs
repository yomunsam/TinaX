using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinaX.VFSKit
{
    /// <summary>
    /// AssetBundle加载计划
    /// </summary>
    public class AssetBundleLoadPlan
    {
        /// <summary>
        /// AssetBundle 加载路径 full path
        /// </summary>
        public string LoadPath { get; private set; }

        public AssetBundleCreateRequest LoadRequest { get; private set; }


        public AssetBundleLoadPlan(string path, AssetBundleCreateRequest request)
        {
            LoadPath = path;
            LoadRequest = request; 
        }
    }

    public class AssetBundleLoadPlanMgr
    {
        private readonly Dictionary<string, AssetBundleLoadPlan> mLoadPlan = new Dictionary<string, AssetBundleLoadPlan>();  //key: 绝对路径

        public bool ContainsPath(string path)
        {
            return mLoadPlan.ContainsKey(path);
        }

        public AssetBundleLoadPlan Get(string path)
        {
            return mLoadPlan[path];
        }

        public void Remove(string path)
        {
            if (mLoadPlan.ContainsKey(path))
            {
                mLoadPlan.Remove(path);
            }
        }

        public void Add(string path,AssetBundleLoadPlan plan)
        {
            if (!mLoadPlan.ContainsKey(path))
            {
                mLoadPlan.Add(path,plan);
            }
        }

    }
}


