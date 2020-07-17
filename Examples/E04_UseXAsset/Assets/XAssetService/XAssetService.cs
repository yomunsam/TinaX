using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using TinaX;
using libx;
using Cysharp.Threading.Tasks;
using System;

namespace XAsset
{
    public class XAssetService : IXAssetService , TinaX.Services.IAssetService
    {
        private Dictionary<UnityEngine.Object, AssetRequest> m_Pool = new Dictionary<UnityEngine.Object, AssetRequest>();

        public async Task<XException> Start()
        {
            var init = Assets.Initialize();
            await init;
            return null;
        }

        public T Load<T>(string assetPath) where T : UnityEngine.Object
        {
            var req = Assets.LoadAsset(assetPath, typeof(T));
            RegisterPool(ref req);
            return req.asset as T;
        }

        public UnityEngine.Object Load(string assetPath, Type type)
        {
            var req = Assets.LoadAsset(assetPath, type);
            RegisterPool(ref req);

            return req.asset;
        }

        public async Task<T> LoadAsync<T>(string assetPath) where T : UnityEngine.Object
        {
            var req = Assets.LoadAssetAsync(assetPath, typeof(T));
            await req;
            RegisterPool(ref req);

            return req.asset as T;
        }

        public void LoadAsync(string assetPath, Type type, Action<UnityEngine.Object, XException> callback)
        {
            var req = Assets.LoadAssetAsync(assetPath, type);
            req.completed += assetReq =>
            {
                if (assetReq.error.IsNullOrEmpty())
                {
                    RegisterPool(ref assetReq);

                    callback?.Invoke(assetReq.asset, null);
                }
                else
                {
                    callback?.Invoke(null, new XException(assetReq.error));
                }
            };
        }

        public void Release(UnityEngine.Object asset)
        {
            lock (this)
            {
                if (m_Pool.TryGetValue(asset, out var req))
                {
                    req.Release();
                    if (req.refCount < 1)
                    {
                        m_Pool.Remove(asset);
                    }
                }
            }
        }

        
        private void RegisterPool(ref AssetRequest req)
        {
            lock (this)
            {
                if (!req.isDone || req.asset == null || !req.error.IsNullOrEmpty())
                    return;

                if (!m_Pool.ContainsKey(req.asset))
                {
                    m_Pool.Add(req.asset, req);
                }
            }
        }

    }
}

