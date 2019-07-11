using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TinaX;

namespace TinaXEditor
{
    /// <summary>
    /// TinaX资源变动监听与预处理
    /// </summary>
    public class AssetInputHandle : AssetPostprocessor
    {
        /// <summary>
        /// 需要监听变动的资源列表,Key:Path
        /// </summary>
        private static Dictionary<string, S_ResListenerInfo> mDict_AssetPathListenerList = new Dictionary<string, S_ResListenerInfo>();

        /// <summary>
        /// 资源监听信息
        /// </summary>
        public struct S_ResListenerInfo
        {
            public string path;
            /// <summary>
            /// 触发回调，传回
            /// </summary>
            public System.Action<string, ResChangeType> callback;

            /// <summary>
            /// 监听引用数量
            /// </summary>
            public int ListenNum;

            public void AddListener(System.Action<string, ResChangeType> _callback)
            {
                callback += _callback;
                ListenNum++;
            }

            public void RemobeListener(System.Action<string, ResChangeType> _callback)
            {
                callback -= _callback;
                ListenNum--;
            }
        }

        public enum ResChangeType
        {
            /// <summary>
            /// 新建或修改
            /// </summary>
            CreateOrModify,
            Remove
        }

        /// <summary>
        /// 添加资源监听
        /// </summary>
        public static void AddAssetListener(string path, System.Action<string, ResChangeType> callback)
        {
            //Debug.Log("添加了一个资源监听:" + path);
            if (mDict_AssetPathListenerList.ContainsKey(path))
            {
                mDict_AssetPathListenerList[path].AddListener(callback);
            }
            else
            {
                var info = new S_ResListenerInfo()
                {
                    path = path,
                    
                };
                info.AddListener(callback);
                mDict_AssetPathListenerList.Add(path,info);
            }
        }

        /// <summary>
        /// 移除资源监听
        /// </summary>
        public static void RemoveAssetListener(string path, System.Action<string, ResChangeType> callback)
        {
            if (mDict_AssetPathListenerList.ContainsKey(path))
            {
                mDict_AssetPathListenerList[path].RemobeListener(callback);
                if(mDict_AssetPathListenerList[path].ListenNum <= 0)
                {
                    mDict_AssetPathListenerList.Remove(path);
                }
            }
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var assetconfig = TinaX.Config.GetTinaXConfig<TinaX.VFS.VFSConfigModel>(TinaX.Conf.ConfigPath.vfs);
            foreach (var item in importedAssets)
            {
                
                #region 资源路径监听
                if (mDict_AssetPathListenerList.ContainsKey(item))
                {
                    mDict_AssetPathListenerList[item].callback(item, ResChangeType.CreateOrModify);
                }
                #endregion

                
            }
            if (assetconfig != null)
            {
                
                SpecialAssetHandle(importedAssets, assetconfig);
                SpecialAssetHandle(deletedAssets, assetconfig);
            }
            
            

        }

        /// <summary>
        /// 特殊文件导入处理
        /// </summary>
        /// <param name="importedorMove"></param>
        static void SpecialAssetHandle(string[] importedorMove,TinaX.VFS.VFSConfigModel assetconfig)
        {
            
            foreach (var item in importedorMove)
            {
                //Debug.Log("特殊文件处理：" + item);
                var path = item;
                var parseInfo = TinaX.VFS.AssetParse.Parse(path, assetconfig);

                var DeleteFlag = false; //当这个资源被删除的话，这里应该改为True 
                #region 特殊资源规则处理_中文
                if (assetconfig.EnableChineseFileInput)
                {
                    //处理，检查是否包含中文
                    if (item.IncludeChinese())
                    {
                        //Debug.Log("  包含中文");
                        //排除不应该被处理的文件
                        if (parseInfo.handle_tag != TinaX.VFS.E_FileHandleTag.invalid)
                        {
                            //包含中文且被虚拟文件系统管理，看看处理方式
                            switch (assetconfig.ChineseHandleType)
                            {
                                case TinaX.VFS.InputFileNameIncludeChineseHandleType.Delete:
                                    
                                    AssetDatabase.DeleteAsset(item);
                                    DeleteFlag = true;
                                    EditorUtility.DisplayDialog("导入资源错误", "导入的资源：" + item + "\n中包含中文文字，根据配置规则，该资源已被删除", "确定");
                                    Debug.LogError("导入资源包含中文命名，已被删除：" + item);
                                    AssetDatabase.Refresh();
                                    break;
                                case TinaX.VFS.InputFileNameIncludeChineseHandleType.Ignore:
                                    //Debug.Log("    忽略");
                                    break;
                                case TinaX.VFS.InputFileNameIncludeChineseHandleType.ReName:
                                    //改名
                                    var cur_timeStamp = System.Convert.ToInt64((System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds).ToString();
                                    var newName = "Rename_" + StringHelper.GenRandomStr(8) + cur_timeStamp + System.IO.Path.GetExtension(item);
                                    AssetDatabase.RenameAsset(path, newName);
                                    Debug.LogWarning("[TinaX][虚拟文件系统]导入的资源命名中包含中文:" + path + "    | 已改名为：" + newName);
                                    AssetDatabase.SaveAssets();
                                    AssetDatabase.Refresh();
                                    path = newName;
                                    break;
                            }
                        }
                        

                    }
                }

                #endregion

                if (!DeleteFlag)
                {

                }
            }

        }



        /// <summary>
        /// 图片类型特殊处理
        /// </summary>
        void OnPreprocessTexture()
        {
            var assetConf = TinaX.Config.GetTinaXConfig<TinaX.VFS.VFSConfigModel>(TinaX.Conf.ConfigPath.vfs);
            var uikitConf = TinaX.Config.GetTinaXConfig<TinaX.UIKit.UIKitConfig>(TinaX.Conf.ConfigPath.uikit);
            string path = assetImporter.assetPath;


            //处理UI类资源的自动转sprite
            if (assetConf != null && uikitConf != null)
            {
                if (assetConf.EnableTextureToSpriteInUIAssetsFolder)
                {
                    var ui_atlas_path = uikitConf.UI_Atlas_Path.ToLower();
                    if (!ui_atlas_path.EndsWith("/"))
                    {
                        ui_atlas_path = ui_atlas_path + "/";
                    }
                    if (path.ToLower().StartsWith(ui_atlas_path))
                    {
                        TextureImporter textureImporter = assetImporter as TextureImporter;
                        textureImporter.textureType = TextureImporterType.Sprite;

                        //图集处理
                        var strArr = path.Split('/');
                        var AtlasName = strArr[strArr.Length - 2];
                        textureImporter.spritePackingTag = AtlasName;
                    }

                    var ui_img_path = uikitConf.UI_Img_Path.ToLower();
                    if (!ui_img_path.EndsWith("/"))
                    {
                        ui_img_path = ui_img_path + "/";
                    }
                    if (path.ToLower().StartsWith(ui_img_path))
                    {
                        TextureImporter textureImporter = assetImporter as TextureImporter;
                        textureImporter.textureType = TextureImporterType.Sprite;
                    }
                    
                }
            }
            
        }

    }

}

