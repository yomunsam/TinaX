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
        /// Destroy self, with delay time
        /// </summary>
        /// <param name="selfObj"></param>
        /// <param name="t">delay time </param>
        public static void DestroySelf(this GameObject selfObj,float t)
        {
            GameObject.Destroy(selfObj,t);
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
        /// 获取Component，如果不存在就新建
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type">Component type</param>
        /// <returns></returns>
        public static Component GetComponentOrAdd(this GameObject obj,System.Type type)
        {
            var c = obj.GetComponent(type);
            if(c == null)
            {
                c = obj.AddComponent(type);
            }
            return c;
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
        /// [链式]移除Component, 如果它存在的话
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type">Component type</param>
        public static GameObject RemoveComponentIfExists(this GameObject obj, System.Type type)
        {
            var t = obj.GetComponent(type);

            if (t != null)
            {
                Object.Destroy(t);
            }
            return obj;
        }

        /// <summary>
        /// [链式]移除Component, 如果它存在的话
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type">Component type</param>
        public static GameObject RemoveComponentIfExists(this GameObject obj, string type)
        {
            var t = obj.GetComponent(type);

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


        public static GameObject RemoveComponentsIfExists<T>(this GameObject obj,System.Type type)
        {
            var t = obj.GetComponents(type);

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
        public static GameObject FindOrCreateGameObject(this GameObject selfGo, string GameObjectName)
        {
            var trans = selfGo.transform.Find(GameObjectName);
            if (trans == null)
            {
                var go = new GameObject(GameObjectName).SetParent(selfGo);
                return go;
            }
            else
            {
                return trans.gameObject;
            }
        }

        /// <summary>
        /// 获取或创建GameObject
        /// </summary>
        /// <param name="selfGo"></param>
        public static GameObject FindOrCreateGo(this GameObject selfGo,string GameObjectName)
        {
            return selfGo.FindOrCreateGameObject(GameObjectName);
        }

        /// <summary>
        /// 获取或创建GameObject
        /// </summary>
        /// <param name="selfGo"></param>
        public static GameObject FindOrCreateGameObject(this GameObject selfGo, string GameObjectName, params System.Type[] Components)
        {
            var trans = selfGo.transform.Find(GameObjectName);
            if (trans == null)
            {
                var go = new GameObject(GameObjectName, Components).SetParent(selfGo);
                return go;
            }
            else
            {
                return trans.gameObject;
            }
        }

        public static GameObject CreateGameObject(this GameObject selfGo, string GameObjectName)
        {
            var go = new GameObject(GameObjectName).SetParent(selfGo);
            return go;
        }

        public static GameObject CreateGameObject(this GameObject selfGo,string GameObjectName,params System.Type[] Components)
        {
            var go = new GameObject(GameObjectName, Components).SetParent(selfGo);
            return go;
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
            if(selfGo.transform != null)
            {
                selfGo.transform.position = position;
            }
            return selfGo;
        }


        /// <summary>
        /// 设置本地坐标
        /// </summary>
        /// <param name="selfGo"></param>
        public static GameObject SetLocalPosition(this GameObject selfGo, Vector3 localPosition)
        {
            if (selfGo.transform != null)
            {
                selfGo.transform.localPosition = localPosition;

            }
            return selfGo;
        }

        public static GameObject SetLocalScale(this GameObject selfGo,Vector3 scaleValue)
        {
            if(selfGo.transform != null)
            {
                selfGo.transform.localScale = scaleValue;
            }
            return selfGo;
        }

        public static GameObject SetRotation(this GameObject selfGo, Quaternion value)
        {
            if (selfGo.transform != null)
            {
                selfGo.transform.rotation = value;
            }
            return selfGo;
        }

        public static GameObject SetLocalRotation(this GameObject selfGo, Quaternion value)
        {
            if (selfGo.transform != null)
            {
                selfGo.transform.localRotation = value;
            }
            return selfGo;
        }

        public static GameObject SetEulerAngles(this GameObject selfGo, Vector3 value)
        {
            if (selfGo.transform != null)
            {
                selfGo.transform.eulerAngles = value;
            }
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
        /// 检查组件是否存在
        /// </summary>
        /// <param name="gameObject">Game object</param>
        /// <param name="type">组件类型</param>
        /// <returns>True when component is attached.</returns>
        public static bool HasComponent(this GameObject gameObject,string type)
        {
            return gameObject.GetComponent(type) != null;
        }

        /// <summary>
        /// 检查组件是否存在
        /// </summary>
        /// <param name="gameObject">Game object</param>
        /// <param name="type">组件类型</param>
        /// <returns>True when component is attached.</returns>
        public static bool HasComponent(this GameObject gameObject, System.Type type)
        {
            return gameObject.GetComponent(type) != null;
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

