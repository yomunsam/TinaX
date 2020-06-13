using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Nekonya.Demo.Assets
{
    public class AssetsService : IAssets, TinaX.Services.IAssetService
    {
        public T Load<T>(string assetPath) where T : UnityEngine.Object
        {
            return Resources.Load<T>(assetPath);
        }

        public UnityEngine.Object Load(string assetPath, Type type)
            => Resources.Load(assetPath, type);

        public async Task<T> LoadAsync<T>(string assetPath) where T : UnityEngine.Object
        {
            var result =  await Resources.LoadAsync<T>(assetPath);
            return result as T;
        }

        public void LoadAsync(string assetPath, Type type, Action<UnityEngine.Object, XException> callback)
        {
            Resources.LoadAsync(assetPath, type)
                .ToUniTask()
                .ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(obj =>
                {
                    callback?.Invoke(obj, null);
                }, 
                e => {
                    throw e;
                });
        }

        public void Release(UnityEngine.Object asset)
        {
            if (!(asset is GameObject))
                Resources.UnloadAsset(asset);
        }
    }
}
