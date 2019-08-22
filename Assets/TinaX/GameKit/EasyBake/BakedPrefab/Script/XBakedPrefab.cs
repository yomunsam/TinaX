//参考 https://github.com/Ayfel/PrefabLightmapping/blob/master/PrefabLightmapData.cs
//发现另一个相似的： https://www.xuanyusong.com/archives/3807 , https://www.cnblogs.com/roger634/p/9719739.html

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//#if ODIN_INSPECTOR
//using Sirenix.OdinInspector;
//#endif

namespace TinaXGameKit.EasyBake
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [AddComponentMenu("TinaX GameKit/EasyBake/xBakedPrefab")]
    public class XBakedPrefab : MonoBehaviour
    {
        [SerializeField]
        SRendererInfo[] mRendererInfos;

        [SerializeField]
        Texture2D[] mLightmaps;


        private void Awake()
        {
            if (mRendererInfos == null || mRendererInfos.Length == 0)
            {
                return;
            }

            var lightmaps = LightmapSettings.lightmaps;
            int[] offsetsIndexes = new int[mLightmaps.Length];
            var countTotal = lightmaps.Length;
            List<LightmapData> combinedLightmaps = new List<LightmapData>();

            for (int i = 0; i < mLightmaps.Length; i++)
            {
                var flag = false;
                for (int j = 0; j < lightmaps.Length; j++)
                {
                    if(mLightmaps[i] == lightmaps[j].lightmapColor)
                    {
                        flag = true;
                        offsetsIndexes[i] = j;
                    }
                }

                if (!flag)
                {
                    offsetsIndexes[i] = countTotal;
                    var newLightmapData = new LightmapData();
                    newLightmapData.lightmapColor = mLightmaps[i];
                    combinedLightmaps.Add(newLightmapData);

                    countTotal++;
                }

            }

            var combinedLightmaps2 = new LightmapData[countTotal];
            lightmaps.CopyTo(combinedLightmaps2, 0);
            combinedLightmaps
                .ToArray()
                .CopyTo(combinedLightmaps2, lightmaps.Length);
            LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
            

        }

        private void ApplyRendererInfo(SRendererInfo[] infos , int[] lightmapOffsetIndex)
        {
            for (int i = 0; i < infos.Length; i++)
            {
                var info = infos[i];
                info.renderer.lightmapIndex = lightmapOffsetIndex[info.lightmapIndex];
                info.renderer.lightmapScaleOffset = info.lightmapOffsetScale;
            }
        }


//#if UNITY_EDITOR

//#if ODIN_INSPECTOR
//        [Button("Bake Prefab",ButtonSizes.Large)]
//#endif
        public void GenerateLightmapInfo()
        {
            //if()
        }


//#endif

    }
}

