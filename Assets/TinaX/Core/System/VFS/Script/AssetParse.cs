using System.Collections;
using System.Collections.Generic;
using CatLib;
using UnityEngine;
using TinaX.Conf;
using System;

namespace TinaX.VFS
{
    /// <summary>
    /// TinaX 资源解析
    /// </summary>
    public class AssetParse
    {
        //[Obsolete("Param 'VFSConfigModel' is obsolete , Please use param 'VFSConfigCache' instead. ")]
        public static S_FileInfo Parse(string file,VFSConfigModel conf = null)
        {
            var re_fileInfo = new S_FileInfo();
            re_fileInfo.file_path = file;
            var path_lower = file.ToLower();
            var asset_conf = conf;
            if (asset_conf == null)
            {
                asset_conf = Config.GetTinaXConfig<VFSConfigModel>(ConfigPath.vfs);
            }
            //忽略判定
            //是否属于白名单
            var white_list_flag = false;
            foreach (var item in asset_conf.Assets_system_whiteList)
            {
                string _t_path;
                if (item.EndsWith("/"))
                {
                    _t_path = item.ToLower();
                }
                else
                {
                    _t_path = item.ToLower() + "/";
                }
                //Debug.Log("判断路径:" + path_lower + "  |是否在白名单：" + _t_path); 
                if (path_lower.StartsWith(_t_path))
                {
                    white_list_flag = true;
                    break;
                }
            }
            if (!white_list_flag)
            {
                if (!IsIgnorePathWhileList(path_lower))
                {
                    //不在资源管理范围内
                    re_fileInfo.ab_name = "";
                    re_fileInfo.file_name_in_ab = "";
                    re_fileInfo.handle_tag = E_FileHandleTag.invalid;
                    re_fileInfo.invalidInfo = "不在虚拟文件系统的管理白名单内";
                    return re_fileInfo;
                }
            }

            //关键字忽略
            foreach (var item in GetPathIgnoreKeyword(asset_conf))
            {
                var _t_path = path_lower;
                if (_t_path.IndexOf('.') != -1)
                {
                    var last_index = _t_path.LastIndexOf("/");
                    _t_path = _t_path.Substring(0, last_index);
                }
                if (_t_path.Contains(item.ToLower()))
                {

                    //命中关键字
                    re_fileInfo.ab_name = "";
                    re_fileInfo.file_name_in_ab = "";
                    re_fileInfo.handle_tag = E_FileHandleTag.invalid;
                    re_fileInfo.invalidInfo = "被关键字忽略规则忽略";
                    return re_fileInfo;
                }
            }
            //后缀名忽略
            foreach (var item in GetPathIgnoreExt(asset_conf))
            {
                if (path_lower.EndsWith("." + item.ToLower()))
                {
                    //命中后缀名
                    re_fileInfo.ab_name = "";
                    re_fileInfo.file_name_in_ab = "";
                    re_fileInfo.handle_tag = E_FileHandleTag.invalid;
                    re_fileInfo.invalidInfo = "被后缀名忽略规则忽略";
                    return re_fileInfo;
                }
            }
            //路径忽略
            foreach (var item in GetPathIgnorePath(asset_conf))
            {
                string _t_path;
                if (item.EndsWith("/"))
                {
                    _t_path = item.ToLower();
                }
                else
                {
                    _t_path = item.ToLower() + "/";
                }
                if (path_lower.StartsWith(_t_path))
                {
                    //Debug.Log("命中");
                    if (!IsIgnorePathWhileList(path_lower))
                    {
                        //命中忽略路径
                        re_fileInfo.ab_name = "";
                        re_fileInfo.file_name_in_ab = "";
                        re_fileInfo.handle_tag = E_FileHandleTag.invalid;
                        re_fileInfo.invalidInfo = "被路径忽略规则忽略：" + _t_path;
                        return re_fileInfo;
                    }
                }
            }
            //文件夹特殊打包规则确定

            foreach (var item in asset_conf.Special_Package_Folder)
            {
                string _t_path;
                string _t_path_pure;
                if (item.FolderName.EndsWith("/"))
                {
                    _t_path = item.FolderName.ToLower();
                    _t_path_pure = _t_path.Substring(0, _t_path.Length - 1);
                }
                else
                {
                    _t_path_pure = item.FolderName.ToLower();
                    _t_path = _t_path_pure + "/";
                }
                if (path_lower.StartsWith(_t_path))
                {
                    re_fileInfo.handle_tag = E_FileHandleTag.special;
                    switch (item.PackType)
                    {
                        case E_Dir_PackType.sub_dir:
                            var subs_path = path_lower.Replace(_t_path, "");
                            var sub_index = subs_path.IndexOf('/');
                            if (sub_index == -1)
                            {
                                re_fileInfo.ab_name = _t_path_pure;
                                re_fileInfo.file_name_in_ab = path_lower;    //TODO
                            }
                            else
                            {
                                re_fileInfo.ab_name = _t_path + subs_path.Substring(0, sub_index);
                                re_fileInfo.file_name_in_ab = path_lower; //TODO
                            }
                            return re_fileInfo;

                        case E_Dir_PackType.whole:
                            re_fileInfo.ab_name = _t_path_pure;
                            re_fileInfo.file_name_in_ab = path_lower;
                            return re_fileInfo;
                    }
                }

            }

            re_fileInfo.ab_name = path_lower;
            re_fileInfo.file_name_in_ab = path_lower;
            re_fileInfo.handle_tag = E_FileHandleTag.single;

            return re_fileInfo;
        }

        public static S_FileInfo Parse(string file,VFSConfigCache confCache)
        {
            var re_fileInfo = new S_FileInfo();
            re_fileInfo.file_path = file;
            var path_lower = file.ToLower();
            
            //忽略判定
            //是否属于白名单
            var white_list_flag = false;
            foreach(var item in confCache.VFS_WhiteList)
            {
                string _t_path;
                if (item.EndsWith("/"))
                {
                    _t_path = item.ToLower();
                }
                else
                {
                    _t_path = item.ToLower() + "/";
                }
                //Debug.Log("判断路径:" + path_lower + "  |是否在白名单：" + _t_path); 
                if (path_lower.StartsWith(_t_path))
                {
                    white_list_flag = true;
                    break;
                }
            }
            if (!white_list_flag)
            {
                if (!IsIgnorePathWhileList(path_lower))
                {
                    //不在资源管理范围内
                    re_fileInfo.ab_name = "";
                    re_fileInfo.file_name_in_ab = "";
                    re_fileInfo.handle_tag = E_FileHandleTag.invalid;
                    re_fileInfo.invalidInfo = "不在虚拟文件系统的管理白名单内";
                    return re_fileInfo;
                }
            }

            //关键字忽略
            foreach (var item in confCache.VFS_IgnorePathKeyword)
            {
                var _t_path = path_lower;
                if (_t_path.IndexOf('.')!= -1)
                {
                    var last_index = _t_path.LastIndexOf("/");
                    _t_path = _t_path.Substring(0, last_index);
                }
                if (_t_path.Contains(item.ToLower()))
                {

                    //命中关键字
                    re_fileInfo.ab_name = "";
                    re_fileInfo.file_name_in_ab = "";
                    re_fileInfo.handle_tag = E_FileHandleTag.invalid;
                    re_fileInfo.invalidInfo = "被关键字忽略规则忽略";
                    return re_fileInfo;
                }
            }
            //后缀名忽略
            foreach(var item in confCache.VFS_IgnoreExt)
            {
                if(path_lower.EndsWith("." + item.ToLower()))
                {
                    //命中后缀名
                    re_fileInfo.ab_name = "";
                    re_fileInfo.file_name_in_ab = "";
                    re_fileInfo.handle_tag = E_FileHandleTag.invalid;
                    re_fileInfo.invalidInfo = "被后缀名忽略规则忽略";
                    return re_fileInfo;
                }
            }
            //路径忽略
            foreach(var item in confCache.VFS_IgnorePath)
            {
                string _t_path;
                if (item.EndsWith("/"))
                {
                    _t_path = item.ToLower();
                }
                else
                {
                    _t_path = item.ToLower() + "/";
                }
                if (path_lower.StartsWith(_t_path))
                {
                    //Debug.Log("命中");
                    if (!IsIgnorePathWhileList(path_lower))
                    {
                        //命中忽略路径
                        re_fileInfo.ab_name = "";
                        re_fileInfo.file_name_in_ab = "";
                        re_fileInfo.handle_tag = E_FileHandleTag.invalid;
                        re_fileInfo.invalidInfo = "被路径忽略规则忽略："+ _t_path;
                        return re_fileInfo;
                    }
                }
            }
            //文件夹特殊打包规则确定
            
            foreach(var item in confCache.VFS_SpecialFolder)
            {
                string _t_path;
                string _t_path_pure;
                if (item.FolderName.EndsWith("/"))
                {
                    _t_path = item.FolderName.ToLower();
                    _t_path_pure = _t_path.Substring(0, _t_path.Length - 1);
                }
                else
                {
                    _t_path_pure = item.FolderName.ToLower();
                    _t_path = _t_path_pure + "/";
                }
                if (path_lower.StartsWith(_t_path))
                {
                    re_fileInfo.handle_tag = E_FileHandleTag.special;
                    switch (item.PackType)
                    {
                        case E_Dir_PackType.sub_dir:
                            var subs_path = path_lower.Replace(_t_path, "");
                            var sub_index = subs_path.IndexOf('/');
                            if (sub_index == -1)
                            {
                                re_fileInfo.ab_name = _t_path_pure;
                                re_fileInfo.file_name_in_ab = path_lower;    //TODO
                            }
                            else
                            {
                                re_fileInfo.ab_name = _t_path + subs_path.Substring(0, sub_index);
                                re_fileInfo.file_name_in_ab = path_lower; //TODO
                            }
                            return re_fileInfo;

                        case E_Dir_PackType.whole:
                            re_fileInfo.ab_name = _t_path_pure;
                            re_fileInfo.file_name_in_ab = path_lower;
                            return re_fileInfo;
                    }
                }

            }
            
            re_fileInfo.ab_name = path_lower;
            re_fileInfo.file_name_in_ab = path_lower;
            re_fileInfo.handle_tag = E_FileHandleTag.single;

            return re_fileInfo;
        }



        /// <summary>
        /// 获取路径忽略关键字
        /// </summary>
        private static string[] GetPathIgnoreKeyword(VFSConfigModel conf)
        {
            return Arr.Merge<string>(
                conf.Ignore_Path_keyword,
                AssetRuleConst.Ignore_Path_keyword
                );
        }

        /// <summary>
        /// 获取路径忽略关键字
        /// </summary>
        private static string[] GetPathIgnoreKeyword()
        {
            return AssetRuleConst.Ignore_Path_keyword;
        }

        

        /// <summary>
        /// 获取路径忽略后缀名
        /// </summary>
        private static string[] GetPathIgnoreExt(VFSConfigModel conf)
        {
            return Arr.Merge<string>(
                conf.Ignore_Ext,
                AssetRuleConst.Ignore_Ext
                );
        }

        /// <summary>
        /// 获取路径忽略路径
        /// </summary>
        private static string[] GetPathIgnorePath(VFSConfigModel conf)
        {
            return Arr.Merge<string>(
                conf.Ignore_Path,
                AssetRuleConst.Ignore_Path
                );
        }

        private static bool IsIgnorePathWhileList(string path)
        {
            //Debug.Log("待检测：" + path);
            foreach(var item in AssetRuleConst.Ignore_Path_whileList)
            {
                string _t_path;
                if (item.EndsWith("/"))
                {
                    _t_path = item.ToLower();
                }
                else
                {
                    _t_path = item.ToLower() + "/";
                }
                if (path.StartsWith(_t_path))
                {
                    return true;
                }
            }
            return false;
        }
    }

}

