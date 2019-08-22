using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

namespace TinaX.VFSKit
{
    public static class AssetParseHelper
    {
        /// <summary>
        /// 解析加载路径
        /// </summary>
        /// <param name="load_path"></param>
        /// <param name="config"></param>
        /// <param name="simplify">简化解析，如果为true,在解析到第一个能判定IsValid为false的情况时就中断解析流程</param>
        /// <returns></returns>
        public static AssetParseInfo Parse(string load_path,VFSConfigModel config,bool simplify = true)
        {
            var parseInfo = new AssetParseInfo
            {
                LoadPath = load_path,
                LoadPathLower = load_path.ToLower()
            };

            //白名单判断
            if (!config.VFS_WhiteList.Any(item => parseInfo.LoadPathLower.StartsWith(item.ToLower())))
            {
                parseInfo.InWhiteList = false; //不在白名单
            }
            else
            {
                parseInfo.InWhiteList = true;
            }

            if (!parseInfo.InWhiteList && simplify)
            {
                return parseInfo;
            }

            //全局路径忽略
            var ignores = config.Ignore_Path.Where(item => parseInfo.LoadPathLower.StartsWith(item.ToLower()));
            if (ignores.Count() > 0)//在忽略名单中，判断一下是否在忽略名单的白名单中
            {

                var flag = false;   //如果命中忽略白名单，则为true;

                foreach (var v in ignores)
                {
                    if (config.Ignore_Path_WhiteList.Any(w => w.ToLower().StartsWith(v.ToLower()) && parseInfo.LoadPathLower.StartsWith(w.ToLower())))
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    parseInfo.Ignore_Path = false;
                }
                else
                {
                    parseInfo.Ignore_Path = true;
                }

            }
            else
            {
                parseInfo.Ignore_Path = false;
            }
            if (parseInfo.Ignore_Path && simplify)
                return parseInfo;

            //后缀名忽略
            if(config.Ignore_ExtName.Any(ext => Path.GetExtension(parseInfo.LoadPathLower) == ext))
            {
                //命中后缀名
                parseInfo.Ignore_ExtName = true;
            }
            else
            {
                parseInfo.Ignore_ExtName = false;
            }
            if (parseInfo.Ignore_ExtName && simplify)
                return parseInfo;

            //关键字忽略
            if(config.Ignore_Path_Keyword.Any(kw => parseInfo.LoadPathLower.IndexOf(kw.ToLower()) > 0))
            {
                parseInfo.Ignore_Keyword = true;
            }
            else
            {
                parseInfo.Ignore_Keyword = false;
            }
            if (parseInfo.Ignore_Keyword && simplify)
                return parseInfo;

            //路径项忽略
            parseInfo.Ignore_PathItem_Keyword = false;
            foreach (var item in config.Ignore_Path_Item_Keyword)
            {
                var path_arr = parseInfo.LoadPathLower.Split('/');
                if(path_arr.Any(i=> i == item.ToLower()))
                {
                    parseInfo.Ignore_PathItem_Keyword = true;
                    break;
                }
            }
            if (parseInfo.Ignore_PathItem_Keyword && simplify)
                return parseInfo;

            //加密路径判定
            if (parseInfo.IsValid)
            {
                var encry_rules = config.VFS_EncryFolder.Where(i => parseInfo.LoadPathLower.StartsWith(i.FolderPath.ToLower()));
                if (encry_rules.Count() > 0)
                {
                    //多条规则命中时，优先使用子级路径
                    var cur_rule = encry_rules.First();
                    foreach (var item in encry_rules)
                    {
                        if (item.FolderPath.Length > cur_rule.FolderPath.Length)
                        {
                            cur_rule = item;
                        }
                    }
                    parseInfo.EncryType = cur_rule.EncryType;
                    parseInfo.IsEncry = true;
                }
                else
                {
                    parseInfo.IsEncry = false;
                    //parseInfo.EncryType = default;
                }

            }


            //打包规则
            var pack_rules = config.FolderPackRule.Where(i => parseInfo.LoadPathLower.StartsWith(i.FolderPath.ToLower()));
            if (pack_rules.Count() > 0)
            {
                //命中规则，挑选最子级路径
                var cur_rule = pack_rules.First();
                foreach(var item in pack_rules)
                {
                    if(item.FolderPath.Length > cur_rule.FolderPath.Length)
                    {
                        cur_rule = item;
                    }
                }
                parseInfo.PackType = cur_rule.PackType;
                //命名处理
                if (parseInfo.IsValid)
                {
                    var pure_path_lower = cur_rule.FolderPath.ToLower().Substring(cur_rule.FolderPath.Length - 1); //预处理之后，所有的路径都是带"/"结尾的，这里把去掉

                    switch (parseInfo.PackType)
                    {
                        default:
                            //最细粒度
                            parseInfo.ABFileNameWithExtName = parseInfo.LoadPathLower + Setup.Framework_VFS_AssetBundle_Ext_Name;
                            parseInfo.ABFileName = parseInfo.LoadPathLower;
                            parseInfo.PathInAB = parseInfo.LoadPathLower;
                            break;
                        case FolderPackageType.sub_dir:
                            var subs_path = parseInfo.LoadPathLower.Substring(cur_rule.FolderPath.Length, parseInfo.LoadPathLower.Length - cur_rule.FolderPath.Length);
                            var sub_index = subs_path.IndexOf('/');
                            if(sub_index == -1)
                            {
                                parseInfo.ABFileName = pure_path_lower ;
                                parseInfo.ABFileNameWithExtName = pure_path_lower + Setup.Framework_VFS_AssetBundle_Ext_Name;
                                parseInfo.PathInAB = parseInfo.LoadPathLower;
                            }
                            else
                            {
                                parseInfo.ABFileName = $"{cur_rule.FolderPath.ToLower()}{subs_path.Substring(0, sub_index)}";
                                parseInfo.ABFileNameWithExtName = $"{cur_rule.FolderPath.ToLower()}{subs_path.Substring(0, sub_index)}" + Setup.Framework_VFS_AssetBundle_Ext_Name;
                                parseInfo.PathInAB = parseInfo.LoadPathLower;
                            }

                            break;
                        case FolderPackageType.whole:
                            parseInfo.ABFileName = pure_path_lower;
                            parseInfo.ABFileNameWithExtName = pure_path_lower + Setup.Framework_VFS_AssetBundle_Ext_Name;
                            parseInfo.PathInAB = parseInfo.LoadPathLower;
                            break;
                    }
                }
            }
            else
            {
                //没有命中规则，默认使用normal打包方式，即最细粒度 每个文件一个包
                parseInfo.PackType = FolderPackageType.normal;
                //命名
                if (parseInfo.IsValid)
                {
                    parseInfo.ABFileName = parseInfo.LoadPathLower;
                    parseInfo.ABFileNameWithExtName = parseInfo.LoadPathLower + Setup.Framework_VFS_AssetBundle_Ext_Name;
                    parseInfo.PathInAB = parseInfo.LoadPathLower;
                }
            }

            


            return parseInfo;
        }

        /// <summary>
        /// 根据AssetBundle的虚拟寻址路径，查询其加密方式
        /// </summary>
        /// <param name="abPath"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static EncryptionType ParseEncryTypeByAssetBundlePath(string abPath, VFSConfigModel config)
        {
            var path_lower = abPath.ToLower();
            var encry_rules = config.VFS_EncryFolder.Where(i => path_lower.StartsWith(i.FolderPath.ToLower()));
            if(encry_rules.Count() > 0)
            {
                var cur_rule = encry_rules.First();
                foreach(var item in encry_rules)
                {
                    if(item.FolderPath.Length > cur_rule.FolderPath.Length)
                    {
                        cur_rule = item;
                    }
                }
                return cur_rule.EncryType;
            }
            else
            {
                return EncryptionType.None;
            }
        }
    }

    /// <summary>
    /// 资产解析信息
    /// </summary>
    public struct AssetParseInfo
    {
        public string LoadPath { get; set; }//加载路径
        public string LoadPathLower { get; set; }

        /// <summary>
        /// Without ExtName
        /// </summary>
        public string ABFileName { get; set; } //真实的AssetBundle文件名

        public string ABFileNameWithExtName { get; set; } //带有后缀名的AssetBundle

        public string PathInAB { get; set; } //用来在AB包中加载具体Asset时用的名字

        public bool IsValid //是否有效路径
        {
            get
            {
                if (!InWhiteList) return false;
                if (Ignore) return false;

                return true;
            }
        }
        /// <summary>
        /// 是否在白名单中
        /// </summary>
        public bool InWhiteList { get; set; }

        public bool Ignore {
            get
            {
                if (Ignore_Path) return true;
                if (Ignore_ExtName) return true;
                if (Ignore_Keyword) return true;

                return false;
            }
        }

        public bool Ignore_Path { get; set; }
        public bool Ignore_ExtName { get; set; }
        public bool Ignore_Keyword { get; set; } //路径关键字忽略
        public bool Ignore_PathItem_Keyword { get; set; } //路径项忽略

        public bool IsEncry { get; set; }
        public EncryptionType EncryType { get; set; }

        public FolderPackageType PackType { get; set; } //打包类型

    }

}
