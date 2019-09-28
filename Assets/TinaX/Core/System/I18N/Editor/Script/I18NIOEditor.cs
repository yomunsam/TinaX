using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TinaX;

namespace TinaXEditor.I18NKit
{
    /// <summary>
    /// 编辑器下直接IO读写，不做缓存（因为编辑器下内容可随时更改
    /// </summary>
    public static class I18NIOEditor
    {
        public static string GetStringByKey(string regionName, string key, string group = TinaX.I18NKit.I18NConst.DefaultGroupName)
        {
            var config = TinaX.Config.GetTinaXConfig<TinaX.I18NKit.I18NConfigModel>(TinaX.Conf.ConfigPath.i18n);//这个config在Unity里面好像有缓存，所以通常可以认为是没有IO开销的
            //不缓存，所以每次获取都会读取一次
            var regions = config.Regions.Where(r => r.region_name == regionName);
            if(regions.Count() <= 0)
            {
                return key;
            }
            else
            {
                var cur_region = regions.First();
                //先找Asset
                var asset_files = cur_region.language_asset_files.Where(f => f.GroupName == group);
                if(asset_files.Count() > 0)
                {
                    //找key
                    foreach(var a_item in asset_files)
                    {
                        if(a_item.ListFile != null && a_item.ListFile.Items != null)
                        {
                            var values = a_item.ListFile.Items.Where(kv => kv.key == key);
                            if(values.Count() > 0)
                            {
                                return values.First().value;
                            }
                        }
                    }
                }

                //找json文件
                var json_files = cur_region.language_json_files.Where(f => f.GroupName == group);
                if(json_files.Count() > 0)
                {
                    foreach(var json_file in json_files)
                    {
                        var json_txt = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(json_file.JsonFilePath);
                        var json_obj = JsonUtility.FromJson<TinaX.I18NKit.I18NJsonTpl>(json_txt.text);
                        if(json_obj != null && json_obj.data != null)
                        {
                            var values = json_obj.data.Where(kv => kv.key == key);
                            if(values.Count() > 0)
                            {
                                return values.First().value;
                            }
                        } 
                    }
                }

                ////找json base64文件
                var jsonb64_files = cur_region.language_json_files_base64.Where(f => f.GroupName == group);
                if (jsonb64_files.Count() > 0)
                {
                    foreach (var json_file in jsonb64_files)
                    {
                        var json_txt = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(json_file.JsonFilePath);
                        var json_obj = JsonUtility.FromJson<TinaX.I18NKit.I18NJsonTpl>(json_txt.text);
                        if (json_obj != null && json_obj.data != null)
                        {
                            var values = json_obj.data.Where(kv => kv.key == key);
                            if (values.Count() > 0)
                            {
                                return values.First().value.Base64ToStr();
                            }
                        }
                    }
                }

            }



            return key;
        }


        public static string[] GetRegionNames()
        {
            var config = TinaX.Config.GetTinaXConfig<TinaX.I18NKit.I18NConfigModel>(TinaX.Conf.ConfigPath.i18n);
            if(config == null)
            {
                return null;
            }
            else
            {
                List<string> regions = new List<string>();
                foreach(var item in config.Regions)
                {
                    regions.Add(item.region_name);
                }
                return regions.ToArray();
            }
        }
    }
}

