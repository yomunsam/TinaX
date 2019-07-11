using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TinaX;
using TinaX.UIKit;

namespace TinaXEditor.UIKit
{
    /// <summary>
    /// UIKit 编辑器菜单
    /// </summary>
    public class XUIKitMenus
    {
        [MenuItem("TinaX/UIKit/创建UI")]
        [MenuItem("Assets/Create/TinaX/UI")]
        static void Create_UI_Prefab()
        {
            //获取当前路径
            if(Selection.assetGUIDs.Length >1)
            {
                EditorUtility.DisplayDialog("定位错误","当前选中了多个项目，无法确定需要新建的位置。","好");
            }
            if(Selection.assetGUIDs.Length < 1)
            {
                EditorUtility.DisplayDialog("定位错误", "当前未选中任何位置", "好");
            }
            //var path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            //Debug.Log("path  " + path);
            string assetPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            string path = assetPath;
            if (System.IO.Path.HasExtension(assetPath))
            {
                path = System.IO.Path.GetDirectoryName(assetPath);
                path = path.Replace("\\", "/");
            }
            var file_path = path + "/UIEntity.prefab";
            if (File.Exists(System.IO.Path.Combine(System.IO.Path.GetFullPath(path), "UIEntity.prefab")))
            {
                var index = 1;
                var new_path = "UIEntity" + index.ToString() + ".prefab";
                while (File.Exists(System.IO.Path.Combine(System.IO.Path.GetFullPath(path), new_path)))
                {
                    index++;
                }
                file_path = path + "/UIEntity" + index.ToString() + ".prefab";
            }

            //创建UI
            var UIEntity_Go = new GameObject(StringHelper.GenRandomStr(10));
            UIEntity_Go.GetComponentOrAdd<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            UIEntity_Go.GetComponentOrAdd<UnityEngine.UI.GraphicRaycaster>();
            UIEntity_Go.GetComponentOrAdd<UIEntity>();

            //var rect_trans2 = UIEntity_Go.GetComponent<RectTransform>();
            //rect_trans2.anchoredPosition = Vector2.zero;
            //rect_trans2.anchorMax = Vector2.one;
            //rect_trans2.anchorMin = Vector2.zero;
            //rect_trans2.sizeDelta = Vector2.zero;
            //rect_trans2.localScale = Vector3.one;

            UIEntity_Go.SetLayerRecursive(5);
            PrefabUtility.SaveAsPrefabAsset(UIEntity_Go, file_path);
            GameObject.DestroyImmediate(UIEntity_Go);

        }

        //[MenuItem("TinaX/UIKit/创建UI工作空间")]
        //[MenuItem("Assets/Create/TinaX/UI工作空间")]
        //static void Create_UI_WorkSpace()
        //{
        //    //获取当前路径
        //    if (Selection.assetGUIDs.Length > 1)
        //    {
        //        EditorUtility.DisplayDialog("定位错误", "当前选中了多个项目，无法确定需要新建的位置。", "好");
        //    }
        //    if (Selection.assetGUIDs.Length < 1)
        //    {
        //        EditorUtility.DisplayDialog("定位错误", "当前未选中任何位置", "好");
        //    }
        //    //var path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
        //    //Debug.Log("path  " + path);
        //    string assetPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
        //    string path = assetPath;
        //    if (System.IO.Path.HasExtension(assetPath))
        //    {
        //        path = System.IO.Path.GetDirectoryName(assetPath);
        //        path = path.Replace("\\", "/");
        //    }
        //    var file_path = path + "/UIWorkSpace.unity";
        //    if (File.Exists(System.IO.Path.Combine(System.IO.Path.GetFullPath(path), "UIWorkSpace.unity")))
        //    {
        //        var index = 1;
        //        var new_path = "UIWorkSpace" + index.ToString() + ".unity";
        //        while (File.Exists(System.IO.Path.Combine(System.IO.Path.GetFullPath(path), new_path)))
        //        {
        //            index++;
        //        }
        //        file_path = path + "/UIWorkSpace" + index.ToString() + ".unity";
        //    }

        //    //创建Scene
        //    var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        //    EditorSceneManager.SetActiveScene(scene);
        //    //在Scene中创建内容
        //    Create_UI_WorkGameObject();

        //    EditorSceneManager.SaveScene(scene, file_path);
        //}

        //private static void Create_UI_WorkGameObject()
        //{
        //    var Go_EditRoot = GameObjectHelper.FindOrCreateGo("UIKit_Editor_Root");
        //    var Go_UICamera = new GameObject("UICamera");
        //    Go_UICamera.transform.SetParent(Go_EditRoot.transform);
        //    var UICamera = Go_UICamera.GetComponentOrAdd<Camera>();
        //    Go_UICamera.transform.position = new Vector3(500, 500, 500);
        //    UICamera.clearFlags = CameraClearFlags.Depth;
        //    UICamera.cullingMask = 1<< 5;
        //    UICamera.orthographic = true;
        //    UICamera.allowHDR = false;
        //    UICamera.allowMSAA = false;

        //    var Go_UIRoot = new GameObject("UIRoot").SetLayerRecursive(5);
        //    Go_UIRoot.transform.SetParent(Go_EditRoot.transform);
        //    Go_UIRoot.GetComponentOrAdd<UIRoot>();
        //    var Canvas = Go_UIRoot.GetComponentOrAdd<Canvas>();
        //    Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        //    Canvas.worldCamera = UICamera;
        //    var Canvas_Scaler = Go_UIRoot.GetComponentOrAdd<CanvasScaler>();
        //    var _GraphicRaycaster = Go_UIRoot.GetComponentOrAdd<GraphicRaycaster>();
        //    _GraphicRaycaster.ignoreReversedGraphics = true;

        //    var Go_EventSystem = GameObjectHelper.FindOrCreateGo("EventSystem");
        //    var _EventSysten = Go_EventSystem.GetComponentOrAdd<EventSystem>();
        //    _EventSysten.sendNavigationEvents = true;
        //    var SIM = Go_EventSystem.GetComponentOrAdd<StandaloneInputModule>();

        //}

        [MenuItem("Assets/TinaX/编辑UI",true)]
        static bool CanEditUI()
        {
            var go = Selection.activeGameObject;
            if (go != null)
            {
                if (go.GetComponent<UIEntity>() != null)
                {
                    return true;
                }
            }
            return false;
        }

        

        [MenuItem("Assets/TinaX/编辑UI")]
        static void EditUI()
        {
            var preview_scene = EditorSceneManager.NewPreviewScene();
            //EditorSceneManager.SetActiveScene(preview_scene);
            PrefabUtility.LoadPrefabContentsIntoPreviewScene(AssetDatabase.GetAssetPath(Selection.activeInstanceID), preview_scene);
            //EditorSceneManager.NewScene( NewSceneSetup.EmptyScene, NewSceneMode.Single);
            //PrefabUtility.InstantiatePrefab(Selection.activeGameObject);
        }
    }

}

