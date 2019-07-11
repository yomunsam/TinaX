using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
//using System.Collections.Generic;

namespace TinaX.UIKit
{
    [CreateAssetMenu(fileName ="UIGroup",menuName = "TinaX/UI组")]
    public class UIGroupConf : ScriptableObject
    {
#if UNITY_EDITOR && ODIN_INSPECTOR
        [Header("UI登记组")]
        [InfoBox("注意确保登记中的所有UI的路径都是可被资源管理器访问的哦。")]
        [TableList]
#endif
        public UIGroupItem[] UIRegister;





        /// <summary>
        /// 注册信息登记，key:name value:path
        /// </summary>
        private Dictionary<string, string> m_dict_ui_register;

        /// <summary>
        /// 根据UI注册名获取UI路径
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetPathByName(string name)
        {
            if(m_dict_ui_register == null)
            {
                InitDict();
            }
            if (m_dict_ui_register.ContainsKey(name))
            {
                return m_dict_ui_register[name];
            }
            else
            {
                return "";
            }
        }

        public void InitDict()
        {
            m_dict_ui_register = new Dictionary<string, string>();
            foreach(var item in UIRegister)
            {
                if (item.Name != "")
                {
                    if (!m_dict_ui_register.ContainsKey(item.Name))
                    {
                        m_dict_ui_register.Add(item.Name, item.Path);
                    }
                    else
                    {
                        Debug.LogWarning("UIKit: UI组有重复的名称定义");
                    }
                }
                else
                {
                    Debug.LogWarning("UIKit: UI组有空的名称定义");
                }
                
            }
        }

    }

    [System.Serializable]
    public struct UIGroupItem
    {
#if UNITY_EDITOR
        [Header("UI名")]
#endif
        public string Name;
#if UNITY_EDITOR && ODIN_INSPECTOR
        [Header("UI路径")]
        [FilePath]
#endif
        public string Path;
    }
}

