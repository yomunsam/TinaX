using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace TinaX.VFSKit
{
    /// <summary>
    /// Web 文件下载计划
    /// </summary>
    public class FileDownloadPlan
    {
        /// <summary>
        /// AssetBundle 加载路径 full path
        /// </summary>
        public System.Uri WebUri { get; private set; }

        public UnityWebRequestAsyncOperation AsyncOperation { get; private set; }


        public FileDownloadPlan(Uri uri, UnityWebRequestAsyncOperation async_operation)
        {
            WebUri = uri;
            AsyncOperation = async_operation;
        }
    }

    public class FileDownloadPlanMgr
    {
        private Dictionary<Uri, FileDownloadPlan> mDownloadPlan = new Dictionary<Uri, FileDownloadPlan>();  //key: Uri

        public bool ContainsUri(Uri uri)
        {
            return mDownloadPlan.ContainsKey(uri);
        }

        public FileDownloadPlan Get(Uri uri)
        {
            return mDownloadPlan[uri];
        }

        public void Remove(Uri uri)
        {
            if (mDownloadPlan.ContainsKey(uri))
            {
                mDownloadPlan.Remove(uri);
            }
        }

        public void Add(Uri uri, FileDownloadPlan plan)
        {
            if (!mDownloadPlan.ContainsKey(uri))
            {
                mDownloadPlan.Add(uri, plan);
            }
        }

    }
}


