using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using TinaX;
using TinaX.UIKits;

namespace TinaXEditor.UIKit
{
    public class UIKitImportHandler : AssetPostprocessor
    {

        /// <summary>
        /// 图片类型特殊处理
        /// </summary>
        void OnPreprocessTexture()
        {
            var uikitConf = TinaX.Config.GetTinaXConfig<TinaX.UIKits.UIKitConfig>(TinaX.Conf.ConfigPath.uikit);
            string path = assetImporter.assetPath;


            //处理UI类资源的自动转sprite
            if (uikitConf != null)
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
