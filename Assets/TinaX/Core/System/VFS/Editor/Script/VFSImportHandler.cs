using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using TinaX;
using TinaX.VFSKit;

namespace TinaXEditor.VFSKit
{
    public class VFSImportHandler : AssetPostprocessor
    {

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var config = TinaX.Config.GetTinaXConfig<TinaX.VFSKit.VFSConfigModel>(TinaX.Conf.ConfigPath.vfs);
            if(config != null)
            {
                SpecialAssetHandle(importedAssets, config);
                SpecialAssetHandle(deletedAssets, config);
                SpecialAssetHandle(movedAssets, config);
            }
        }


        static void SpecialAssetHandle(string[] importedorMove, TinaX.VFSKit.VFSConfigModel config)
        {

            foreach (var item in importedorMove)
            {
                //Debug.Log("特殊文件处理：" + item);
                var path = item;
                var parseInfo = TinaX.VFSKit.AssetParseHelper.Parse(path,config);

                var DeleteFlag = false; //当这个资源被删除的话，这里应该改为True 
                #region 非字母和数字
                if (config.HandlerInputNotLettersOrNums)
                {
                    if (parseInfo.IsValid)
                    {
                        //处理，检查是否包含中文
                        if (item.IncludeChinese())
                        {
                            ///包含中文且被虚拟文件系统管理，看看处理方式
                            switch (config.NotLettersOrNumsHandleType)
                            {
                                case ImportNotLettersOrNumsHandleType.Delete:

                                    AssetDatabase.DeleteAsset(item);
                                    DeleteFlag = true;
                                    EditorUtility.DisplayDialog("导入资源错误", "导入的资源：" + item + "\n中包含中文文字，根据配置规则，该资源已被删除", "确定");
                                    Debug.LogError("导入资源包含中文命名，已被删除：" + item);
                                    AssetDatabase.Refresh();
                                    break;
                                case ImportNotLettersOrNumsHandleType.Warning:
                                    //Debug.Log("    忽略");
                                    break;
                                case ImportNotLettersOrNumsHandleType.Rename:
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




    }
}
