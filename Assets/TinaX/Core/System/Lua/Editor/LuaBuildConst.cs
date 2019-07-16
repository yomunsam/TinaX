#if TinaX_CA_LuaRuntime_Enable

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XLua;

namespace TinaXEditor.Lua
{
    public static class LuaBuildConst
    {
        [LuaCallCSharp]
        //[ReflectionUse]
        public static List<Type> LuaCallCSharpNestedTypes
        {
            get
            {
                var types = new List<Type>();
                foreach (var type in LuaCallCSharpList)
                {
                    foreach (var nested_type in type.GetNestedTypes(BindingFlags.Public))
                    {
                        if ((!nested_type.IsAbstract && typeof(Delegate).IsAssignableFrom(nested_type))
                            || nested_type.IsGenericTypeDefinition)
                        {
                            continue;
                        }

                        types.Add(nested_type);
                    }
                }

                return types;
            }
        }

        [LuaCallCSharp]
        //[ReflectionUse]
        public static List<Type> LuaCallCSharpList = new List<Type>() {

            #region Unity

            //Unity
            typeof(UnityEngine.Application),
            typeof(GameObject),
            typeof(MonoBehaviour),
            typeof(Behaviour),
            typeof(Component),
            typeof(RectTransform),
            typeof(Transform),
            typeof(UnityEngine.UI.Text),
            typeof(UnityEngine.UI.Button),
            typeof(UnityEngine.UI.Image),
            typeof(UnityEngine.Events.UnityEvent),
            typeof(UnityEngine.Events.UnityEventBase),
            typeof(AudioClip),
            typeof(UnityEngine.Object),
            typeof(Application),
            typeof(Vector2),
            typeof(Vector3),


            #endregion


            //DotNet
            typeof(System.IO.Path),
            //typeof(System.IO.Directory),
            //typeof(System.IO.File),

            typeof(System.Object),
            typeof(List<int>),
            typeof(Action<string>),


            #region TinaX

            //TinaX
            typeof(TinaX.Setup),
            
            typeof(TinaX.XSound),
            typeof(TinaX.Sound.SoundTrack),
            typeof(TinaX.AssetsMgr),
            typeof(TinaX.VFS.XAssetsManager),

            typeof(TinaX.Lua.LuaBehaviour),




            typeof(TinaX.XLog),
            typeof(TinaX.XCore),
            typeof(TinaX.GameObjectExt),
            typeof(TinaX.StringExt),

            //TinaX System
            typeof(TinaX.Sound.XSoundMgr),
            typeof(TinaX.TimeMachine),
            typeof(TinaX.XTimeMachine),
            

            typeof(TinaX.XI18N),
            typeof(TinaX.I18N.XI18NMgr),

            //UIKit
            typeof(TinaX.UIKit.XButton),
            typeof(TinaX.UIKit.XImage),
            typeof(TinaX.UIKit.XText),
            typeof(TinaX.UIKit.UIKit),
            typeof(TinaX.UIKit.XUIManager),
            typeof(TinaX.UIKit.UIEntity),
            typeof(TinaX.UIKit.XUIMgrGateway),
            typeof(TinaX.UIKit.IUIEntity),
            typeof(TinaX.UIKit.IUIMgr),


            #endregion

            #region 第三方依赖

            ////thirdParty

            //typeof(SuperScrollView.LoopListView2),
            //typeof(SuperScrollView.LoopListViewItem2),


            //Unirx
            typeof(UniRx.Observable),


            #endregion
        };

        [CSharpCallLua]
        public static List<Type> CSharpCallLuaList = new List<Type>()
        {
            typeof(Action),
            typeof(Action<string>),
            typeof(Action<string, string>),
            typeof(Action<string, int>),
            typeof(Action<double>),
            typeof(Action<bool>),
            typeof(Action<Collider>),
            typeof(Action<Collision>),
            typeof(Action<UnityEngine.Object>),

            ////thirdparty
            //typeof(System.Func<SuperScrollView.LoopListView2, int, SuperScrollView.LoopListViewItem2>),
            //typeof(Action<SuperScrollView.LoopListViewItem2,object>),
            
        };



        [BlackList]
        public static List<List<string>> BlackList = new List<List<string>>()  {
            //Unity
            new List<string>(){ "UnityEngine.UI.Text", "OnRebuildRequested"},


            new List<string>(){ "TinaX.VFS.XAssetsManager", "Debug_GetABLoadedInfo"},

            new List<string>(){ "TinaX.Setup", "EditorFrameworkOutsideFolderPath"},
            new List<string>(){ "TinaX.Setup", "Framework_AssetSystem_Pack_Path"},
            new List<string>(){ "TinaX.Setup", "Framework_Build_Output_Path"},
            new List<string>(){ "TinaX.Setup", "Framework_VFS_Patch_Path"},
            new List<string>(){ "TinaX.Setup", "EditorFrameworkCacheFolder"},

            new List<string>(){ "TinaX.Lua.LuaBehaviour", "HotOverload"}

        };

    }

    
}

#endif