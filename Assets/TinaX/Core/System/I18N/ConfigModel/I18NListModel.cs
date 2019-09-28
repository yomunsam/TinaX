using UnityEngine;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif


namespace TinaX.I18NKit
{
    [CreateAssetMenu(fileName = "i18nList",menuName = "TinaX/I18N List")]
    public class I18NListModel : ScriptableObject
    {
#if UNITY_EDITOR && ODIN_INSPECTOR
        [TableList(ShowIndexLabels =true)]
#endif
        public ListItem[] Items;

        

        [System.Serializable]
        public struct ListItem
        {
            [Header("key")]
            public string key;
            [Header("value")]
            public string value;
        }
    }
}

