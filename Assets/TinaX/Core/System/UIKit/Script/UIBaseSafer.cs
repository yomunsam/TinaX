using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * UIBase继承这个方法，使得一些Unity的魔法方法无法直接被子类重载。
 * 尽可能确保UIBase中的魔法方法被正确调用
 * 
 * 
 */

namespace TinaX.UIKits
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIEntity))]
    public class UIBaseSafer : MonoBehaviour
    {
        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {

        }

        protected virtual void OnDestroy()
        {

        }



    }
}

