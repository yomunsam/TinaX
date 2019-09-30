using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinaX.UIKits
{
    public class UIController : XBehaviour
    {
        protected GameObject gameObject
        {
            get
            {
                return entity.gameObject;
            }
        }

        public UIEntity entity { get; internal set; }

        public virtual void OnOpenUIMessage(object Param) { }

        public virtual void OnCloseUIMessage(object Param) { }


        protected void CloseMe()
        {
            entity?.Close();
        }


    }
}


