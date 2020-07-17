using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using libx;
using TinaX;


namespace XAsset.Awaiter
{
    public struct AssetRequestAwaiter : ICriticalNotifyCompletion
    {
        AssetRequest m_AssetRequest;
        Action<AssetRequest> m_continuationAction;

        public bool IsCompleted => m_AssetRequest.isDone;

        public AssetRequestAwaiter(AssetRequest req)
        {
            m_AssetRequest = req;
            m_continuationAction = null;
        }

        public UnityEngine.Object GetResult()
        {
            if(m_continuationAction != null)
            {
                m_AssetRequest.completed -= m_continuationAction;
            }
            return m_AssetRequest.asset;
        }


        public void OnCompleted(Action continuation) //不严谨写法 | Not rigorous in writing
        {
            m_continuationAction = req => 
            {
                if (!req.error.IsNullOrEmpty())
                {
                    throw new XException($"[XAsset] Load asset error: {req.error}");
                }
                continuation?.Invoke();
            };
            m_AssetRequest.completed += m_continuationAction;
        }

        public void UnsafeOnCompleted(Action continuation) //不严谨写法 | Not rigorous in writing
        {
            m_continuationAction = req =>
            {
                if (!req.error.IsNullOrEmpty())
                {
                    throw new XException($"[XAsset] Load asset error: {req.error}");
                }
                continuation?.Invoke();
            };
            m_AssetRequest.completed += m_continuationAction;
        }
    }
}
