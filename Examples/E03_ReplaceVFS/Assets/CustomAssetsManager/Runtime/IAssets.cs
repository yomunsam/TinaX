using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX;

namespace Nekonya.Demo.Assets
{
    public interface IAssets
    {
        T Load<T>(string assetPath) where T : UnityEngine.Object;
        UnityEngine.Object Load(string assetPath, Type type);
        Task<T> LoadAsync<T>(string assetPath) where T : UnityEngine.Object;
        void LoadAsync(string assetPath, Type type, Action<UnityEngine.Object, XException> callback);
        void Release(UnityEngine.Object asset);
    }
}
