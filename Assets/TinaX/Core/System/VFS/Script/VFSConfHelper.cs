using CatLib;
using System.Collections.Generic;
using System.Linq;

namespace TinaX.VFSKit
{
    public static class VFSConfHelper
    {
        /// <summary>
        /// 传入用户设置，返回合并用户设置和TinaX内部设置的合并设置
        /// </summary>
        /// <param name="UserConfig"></param>
        /// <returns></returns>
        public static VFSConfigModel GetPerfectConfig(VFSConfigModel _UserConfig)
        {
            var UserConfig = (VFSConfigModel)_UserConfig.Clone();
            //白名单
            List<string> _white_list = new List<string>();
            //特殊规则
            List<FolderPackageRule> _folder_pack_rule = new List<FolderPackageRule>();
            //全局忽略路径
            List<string> _ignore_path = new List<string>();
            //针对忽略路径的白名单
            List<string> _ignore_path_whiteList = new List<string>();
            //后缀名忽略
            List<string> _ignore_extname = new List<string>();
            //忽略路径项
            List<string> _ignore_path_item = new List<string>();
            //文件加密设置
            List<FolderEncryDefine> _encry_config = new List<FolderEncryDefine>();

            #region 白名单
            //导入用户配置
            foreach (var item in UserConfig.VFS_WhiteList)
            {
                //格式统一
                _white_list.Add(PathProcessing(item));
            }
            //导入框架私有配置
            foreach(var item in VFSPrivateConfig.VFS_WhiteList)
            {
                var str = PathProcessing(item); ;
                if (!_white_list.Contains(str))
                {
                    _white_list.Add(str);
                }
            }

            #endregion

            #region 特殊打包规则
            //用户配置
            foreach(var item in UserConfig.FolderPackRule)
            {
                //路径规范化
                var _item = item;
                PathProcessingRef(ref _item.FolderPath);
                _folder_pack_rule.Add(_item); 
            }
            //框架私有配置
            foreach(var item in VFSPrivateConfig.VFS_FolderPackRule)
            {
                var _item = item;
                PathProcessingRef(ref _item.FolderPath);
                if(!_folder_pack_rule.Any(v => v.FolderPath == _item.FolderPath))
                {
                    _folder_pack_rule.Add(_item);
                }
            }
            //处理编辑器Only
            //在非编辑器下，从白名单中干掉EditorOnly规则的路径
            //注意，打包流程本身是在编辑器下的，所以打包流程中要注意白名单中的 编辑器Only
#if !UNITY_EDITOR
            var items = _folder_pack_rule.Where(item => item.PackType == FolderPackageType.EditorOnly);
            foreach(var item in items)
            {
                if (_white_list.Contains(item.FolderPath))
                {
                    _white_list.Remove(item.FolderPath);
                }
            }
#endif


            #endregion

            #region 全局忽略路径
            //用户定义
            foreach(var item in UserConfig.Ignore_Path)
            {
                _ignore_path.Add(PathProcessing(item));
            }
            //系统定义
            foreach(var item in VFSPrivateConfig.VFS_Ignore_Path)
            {
                var _path = PathProcessing(item);
                if (!_ignore_path.Contains(_path))
                {
                    _ignore_path.Add(_path);
                }
            }
            #endregion

            #region 忽略路径中的白名单
            foreach(var item in UserConfig.Ignore_Path_WhiteList)
            {
                _ignore_path_whiteList.Add(PathProcessing(item));
            }
            foreach(var item in VFSPrivateConfig.VFS_Ignore_Path_Whitelist)
            {
                var path = PathProcessing(item);
                if (!_ignore_path_whiteList.Contains(path))
                {
                    _ignore_path_whiteList.Add(path);
                }
            }

            #endregion

            #region 后缀名
            foreach(var item in Arr.Merge<string>(UserConfig.Ignore_ExtName,VFSPrivateConfig.Ignore_ExtName))
            {
                var extName = item;
                if (!extName.StartsWith("."))
                {
                    extName = "." + extName;
                }
                if (!_ignore_extname.Contains(extName))
                {
                    _ignore_extname.Add(extName);
                }
            }

            #endregion

            #region 忽略路径项目
            foreach(var item in Arr.Merge<string>(UserConfig.Ignore_Path_Item_Keyword, VFSPrivateConfig.Ignore_Path_Item))
            {
                if(!_ignore_path_item.Any(v => v.ToLower() == item.ToLower()))
                {
                    _ignore_path_item.Add(item);
                }
            }
            #endregion

            #region 加密设置
            //用户配置
            foreach (var item in UserConfig.VFS_EncryFolder)
            {
                //路径规范化
                var _item = item;
                PathProcessingRef(ref _item.FolderPath);
                _encry_config.Add(_item);
            }
            //框架私有配置
            foreach (var item in VFSPrivateConfig.VFS_EncryFolder)
            {
                var _item = item;
                PathProcessingRef(ref _item.FolderPath);
                if (!_encry_config.Any(v => v.FolderPath == _item.FolderPath))
                {
                    _encry_config.Add(_item);
                }
            }
            #endregion

            #region 统一赋值进去
            //白名单
            UserConfig.VFS_WhiteList = _white_list.ToArray();
            UserConfig.FolderPackRule = _folder_pack_rule.ToArray();
            UserConfig.Ignore_Path = _ignore_path.ToArray();
            UserConfig.Ignore_Path_WhiteList = _ignore_path_whiteList.ToArray();
            UserConfig.Ignore_ExtName = _ignore_extname.ToArray();
            UserConfig.Ignore_Path_Item_Keyword = _ignore_path_item.ToArray();
            UserConfig.VFS_EncryFolder = _encry_config.ToArray();
            #endregion

            #region 几个小私有方法
            void PathProcessingRef(ref string path)
            {
                //统一以"/"结尾
                if (!path.EndsWith("/"))
                {
                    path += "/";
                }
            }

            string PathProcessing(string path)
            {
                if (!path.EndsWith("/"))
                {
                    return path += "/";
                }
                else
                {
                    return path;
                }
            }

            #endregion

            return UserConfig;
        }

        public static VFSConfigModel GetPerfect(this VFSConfigModel self)
        {
            return VFSConfHelper.GetPerfectConfig(self);
        }

    }

}
