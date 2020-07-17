using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinaX;
using UnityEngine;


namespace XAsset
{
    public interface IXAssetService
    {
        Task<XException> Start();
    }
}

