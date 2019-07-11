using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TinaX
{
    /// <summary>
    /// 配置读取类
    /// </summary>
    public class Config
    {

        /// <summary>
        /// 获取TinaX配置文件
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="file">文件名</param>
        public static T GetTinaXConfig<T>(string file) where T:ScriptableObject
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                //运行模式
                return Config_Cache.I.GetTinaXConfig<T>(file);
            }
            else
            {
                //直接读取

                return Resources.Load<T>(Setup.Framework_Config_Path + "/" + System.IO.Path.GetFileNameWithoutExtension(file));
            }

#else
            return Config_Cache.I.GetTinaXConfig<T>(file);
#endif
        }

#if UNITY_EDITOR

        /// <summary>
        /// 确保父路径
        /// </summary>
        private static void CheckDir()
        {
            var path_unity = "Assets/Resources/" + Setup.Framework_Config_Path + "/";
            path_unity = path_unity.Replace("/", "\\");
            var system_path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), path_unity);
            if (!System.IO.Directory.Exists(system_path))
            {
                System.IO.Directory.CreateDirectory(system_path);
            }
        }


        /// <summary>
        /// [Editor Only]如果不存在则创建配置
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="file_name">文件名</param>
        public static T CreateIfNotExist<T>(string file_name) where T : ScriptableObject
        {
            //AssetDatabase.SaveAssets();
            AssetDatabase.ReleaseCachedFileHandles();
            AssetDatabase.Refresh();

            var file = Resources.Load<T>(Setup.Framework_Config_Path + "/" + System.IO.Path.GetFileNameWithoutExtension(file_name));
            if (file == null)
            {
                var conf = ScriptableObject.CreateInstance<T>();
                //试着创建一下父路径
                CheckDir();

                AssetDatabase.CreateAsset(conf, "Assets/Resources/" + Setup.Framework_Config_Path + "/" + file_name);
                return conf;
            }
            else
            {
                return file;
            }
        }

#endif


    }

    /// <summary>
    /// 在运行时，将读取的配置缓存在内存中
    /// </summary>
    public class Config_Cache
    {
        #region Instance
        private static Config_Cache _instance;
        public static Config_Cache I
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Config_Cache();
                }
                return _instance;
            }
        }

        #endregion

        private Dictionary<string, ScriptableObject> m_config_cache = new Dictionary<string, ScriptableObject>();


        public T GetTinaXConfig<T>(string file_name) where T: ScriptableObject
        {
            //检查是否存在
            if (m_config_cache.ContainsKey(file_name))
            {
                return (T)m_config_cache[file_name];
            }
            else
            {
                //加载配置文件
                //Debug.Log("加载配置文件：" + Setup.Framework_Config_Path + "/" + System.IO.Path.GetFileNameWithoutExtension(file_name));
                var config = Resources.Load<T>(Setup.Framework_Config_Path + "/" + System.IO.Path.GetFileNameWithoutExtension(file_name));
                m_config_cache.Add(file_name, config);
                return config;
            }
        }


    }

}

