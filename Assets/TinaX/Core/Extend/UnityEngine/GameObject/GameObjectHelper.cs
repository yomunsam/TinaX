using System;
using UnityEngine;

namespace TinaX
{
    public class GameObjectHelper
    {


        public static GameObject FindOrCreateGo(string name)
        {
            var go = GameObject.Find(name);
            if (go == null)
            {
                go = new GameObject(name);
            }
            return go;
        }

        
    }
}
