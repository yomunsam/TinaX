using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace TinaXGameKit.Drama.BP
{
    public abstract class DialogueBaseNode : Node
    {
        abstract public void Trigger(IPlayerForNode player);

        abstract public void DoNextWhitParam(IPlayerForNode player, System.Object param );

        abstract public void DoNext(IPlayerForNode player);

        /// <summary>
        /// 重新初始化（用于同一个对象多次调用时，重置状态）
        /// </summary>
        public virtual void ReInit()
        {

        }

        [System.Serializable]
        public class Connection
        {

        }

        [System.Serializable]
        public struct TextBindTpl
        {
            public string key;
            public string value;
        }

        [System.Serializable]
        public struct NumberBindTpl
        {
            public string key;
            public float value;
        }
    }
}
