using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinaX
{
    public static class GameObjectExt
    {

        public static void DestroySelf(this GameObject selfObj)
        {
            GameObject.Destroy(selfObj);
        }

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="selfObj"></param>
        /// <returns></returns>
        public static GameObject Show(this GameObject selfObj)
        {
            selfObj.SetActive(true);
            return selfObj;
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        /// <param name="selfObj"></param>
        /// <returns></returns>
        public static GameObject Hide(this GameObject selfObj)
        {
            selfObj.SetActive(false);
            return selfObj;
        }

        /// <summary>
        /// 命名
        /// </summary>
        /// <param name="selfObj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject Name(this GameObject selfObj, string name)
        {
            selfObj.name = name;
            return selfObj;
        }


        /// <summary>
        /// 不要销毁
        /// </summary>
        /// <param name="selfObj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject DontDestroy(this GameObject selfObj)
        {
            GameObject.DontDestroyOnLoad(selfObj);
            return selfObj;
        }


        /// <summary>
        /// 获取Component，如果不存在就新建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetComponentOrAdd<T>(this GameObject obj) where T : Component
        {
            var t = obj.GetComponent<T>();

            if (t == null)
            {
                t = obj.AddComponent<T>();
            }

            return t;
        }

        /// <summary>
        /// [链式]移除Component, 如果它存在的话
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static GameObject RemoveComponentIfExists<T>(this GameObject obj) where T : Component
        {
            var t = obj.GetComponent<T>();

            if (t != null)
            {
                Object.Destroy(t);
            }
            return obj;
        }

        /// <summary>
        /// [链式]移除全部相同的Components, 如果它们存在的话
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static GameObject RemoveComponentsIfExists<T>(this GameObject obj) where T : Component
        {
            var t = obj.GetComponents<T>();

            for (var i = 0; i < t.Length; i++)
            {
                Object.Destroy(t[i]);
            }
            return obj;
        }


        /// <summary>
        /// [链式]设置一个GameObject和它的所有子物体的Layer
        /// </summary>
        /// <param name="o"></param>
        /// <param name="layer"></param>
        public static GameObject SetLayerRecursive(this GameObject o, int layer)
        {
            SetLayerInternal(o.transform, layer);
            return o;
        }

        private static void SetLayerInternal(Transform t, int layer)
        {
            t.gameObject.layer = layer;

            foreach (Transform o in t)
            {
                SetLayerInternal(o, layer);
            }
        }


        /// <summary>
        /// 在本帧直接销毁
        /// </summary>
        /// <param name="selfGo"></param>
        public static void DestroyNow(this GameObject selfGo)
        {
            GameObject.DestroyImmediate(selfGo);
        }


        /// <summary>
        /// 获取或创建GameObject
        /// </summary>
        /// <param name="selfGo"></param>
        public static GameObject FindOrCreateGo(this GameObject selfGo,string GameObjectName)
        {
            var trans = selfGo.transform.Find(GameObjectName);
            if (trans == null)
            {
                var go = new GameObject(GameObjectName);
                go.transform.SetParent(selfGo.transform);
                return go;
            }
            else
            {
                return trans.gameObject;
            }
        }

        /// <summary>
        /// 设置父级GameObject
        /// </summary>
        /// <param name="selfGo">返回自身</param>
        public static GameObject SetParent(this GameObject selfGo, GameObject parentGameObject)
        {
            selfGo.transform.SetParent(parentGameObject.transform);
            return selfGo;
        }


        /// <summary>
        /// 设置世界坐标
        /// </summary>
        /// <param name="selfGo"></param>
        public static GameObject SetPosition(this GameObject selfGo, Vector3 position)
        {
            selfGo.transform.position = position;
            return selfGo;
        }


        /// <summary>
        /// 设置本地坐标
        /// </summary>
        /// <param name="selfGo"></param>
        public static GameObject SetLocalPosition(this GameObject selfGo, Vector3 localPosition)
        {
            selfGo.transform.localPosition = localPosition;
            return selfGo;
        }


        /// <summary>
        /// 检查组件是否存在
        /// </summary>
        /// <param name="gameObject">Game object</param>
        /// <returns>True when component is attached.</returns>
        public static bool HasComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() != null;
        }

        /// <summary>
        /// 是否为空
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static bool IsNull(this GameObject go)
        {
            return go == null;
        }

    }
}

