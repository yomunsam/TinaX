using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinaX.VFSKit
{
    /// <summary>
    /// AssetBundle Web下载计划
    /// </summary>
    public class AssetBundleDownloadPlan
    {
        /// <summary>
        /// AssetBundle 加载路径 full path
        /// </summary>
        public string LoadPath { get; private set; }

        public AssetBundleCreateRequest LoadRequest { get; private set; }


        public AssetBundleDownloadPlan(string path, AssetBundleCreateRequest request)
        {
            LoadPath = path;
            LoadRequest = request;
        }
    }

    public class AssetBundleDownloadPlanMgr
    {
        private Dictionary<string, AssetBundleLoadPlan> mDownloadPlan = new Dictionary<string, AssetBundleLoadPlan>();  //key: 绝对路径

        public bool ContainsPath(string path)
        {
            return mDownloadPlan.ContainsKey(path);
        }

        public AssetBundleLoadPlan Get(string path)
        {
            return mDownloadPlan[path];
        }

        public void Remove(string path)
        {
            if (mDownloadPlan.ContainsKey(path))
            {
                mDownloadPlan.Remove(path);
            }
        }

        public void Add(string path, AssetBundleLoadPlan plan)
        {
            if (!mDownloadPlan.ContainsKey(path))
            {
                mDownloadPlan.Add(path, plan);
            }
        }

    }
}


