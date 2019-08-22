//using System.Collections.Generic;
//using UnityEngine;
//namespace TinaX.Upgrade
//{
//    /// <summary>
//    /// 静态更新配置的Json文件模板
//    /// </summary>
//    public class StaticUpgradeConfigJsonTpl
//    {
//        public BasePackInfo[] versions;

//        private E_UpgradeHandleType mHandleType; //处理更新的方式
//        private int mMax_Version;   //最新版本号
//        private BasePackInfo mMax_Base_Pack_Info; //最新母包信息
        

//        /// <summary>
//        /// 检查是否需要更新
//        /// </summary>
//        /// <returns></returns>
//        public bool IsNeedUpgrade(int base_ver, int patch_ver)
//        {
//            //检查base_version
//            Debug.Log("检查是否需要更新，母包版本：" + base_ver + "  ,当前补丁包版本：" + patch_ver );
//            if (versions.Length < 1)
//            {
//                return false;
//            }

//            var flag = false;
//            foreach(var item in versions)
//            {
//                if(item.base_version == base_ver)
//                {
//                    //找到了这个版本
//                    flag = true;

//                    //检查更新属性
//                    if (item.upgrade_type == "cease")
//                    {
//                        //这个版本已经停用了,要更新
//                        //要下载母包，判断是客户端下载还是打开网页
//                        //先找到当前最新的包
//                        BasePackInfo last_pack_info = versions[0];
//                        int cur_max_version = versions[0].base_version;
//                        foreach(var emm in versions)
//                        {
//                            if(emm.base_version > cur_max_version)
//                            {
//                                cur_max_version = emm.base_version;
//                                last_pack_info = emm;
//                            }
//                        }

//                        mMax_Base_Pack_Info = last_pack_info;
//                        mMax_Version = cur_max_version;
//                        //看看最新版本的包的下载方式
//                        if (last_pack_info.download_type == "default")
//                        {
//                            mHandleType = E_UpgradeHandleType.download_base_pack;

//                        }
//                        else if(last_pack_info.download_type == "webpage")
//                        {
//                            mHandleType = E_UpgradeHandleType.webpage;
//                        }

//                        return true;
//                    }


//                    mHandleType = E_UpgradeHandleType.download;
//                    //检查补丁版本是否是最新的
//                    var flag2 = false; //如果找到更新的版本，这里置为true
//                    foreach(var sub in item.packages)
//                    {
//                        if(sub.version > patch_ver)
//                        {
//                            flag2 = true;
//                            return true;
//                        }
//                    }
//                    if (!flag2)
//                    {
//                        return false;
//                    }


//                    break;
//                }
//            }

//            if(flag == false)
//            {
//                //丫的就没找到这个版本信息，更新！
//                Debug.Log("没找到这个版本的信息");


//                //这时候让客户端去下载最新的母包好了
//                //要下载母包，判断是客户端下载还是打开网页
//                //先找到当前最新的包
//                BasePackInfo last_pack_info = versions[0];
//                int cur_max_version = versions[0].base_version;
//                foreach (var emm in versions)
//                {
//                    if (emm.base_version > cur_max_version)
//                    {
//                        cur_max_version = emm.base_version;
//                        last_pack_info = emm;
//                    }
//                }

//                mMax_Version = cur_max_version;
//                mMax_Base_Pack_Info = last_pack_info;
//                //看看最新版本的包的下载方式
//                if (last_pack_info.download_type == "default")
//                {
//                    mHandleType = E_UpgradeHandleType.download_base_pack;

//                }
//                else if (last_pack_info.download_type == "webpage")
//                {
//                    mHandleType = E_UpgradeHandleType.webpage;
//                }


//                return true;
//            }

//            return false;
//        }


//        /// <summary>
//        /// 获取所有要下载的补丁文件
//        /// </summary>
//        /// <returns></returns>
//        public PatchInfo[] GetDownloadList(int base_ver, int patch_ver)
//        {
            
//            List<PatchInfo> t_list = new List<PatchInfo>();


//            foreach (var item in versions)
//            {
//                if (item.base_version == base_ver)
//                {
//                    if (item.upgrade_type == "each")
//                    {
//                        //下载所有大于当前版本的补丁包
//                        foreach (var sub in item.packages)
//                        {
//                            if (sub.version > patch_ver)
//                            {
//                                t_list.Add(sub);
//                            }
//                        }
//                    }
//                    else
//                    {
//                        //下载最新版本的补丁包
//                        if(item.packages.Length > 0)
//                        {
//                            var max_patch_info = item.packages[0];
//                            foreach(var sub in item.packages)
//                            {
//                                if(sub.version > max_patch_info.version)
//                                {
//                                    max_patch_info = sub;
//                                }
//                            }
//                            return new PatchInfo[] { max_patch_info };


//                        }
//                        else
//                        {
//                            return new PatchInfo[] { };
//                        }
//                    }
                    

//                    break;
//                }
//            }

//            t_list.Sort((x, y) => -x.version.CompareTo(y.version));

//            return t_list.ToArray();
//        }

//        public E_UpgradeHandleType GetUpgradeHandleType()
//        {
//            return mHandleType;
//        }

//        /// <summary>
//        /// 获取最新的母包的网页Url
//        /// </summary>
//        /// <returns></returns>
//        public string GetWebPageUrl()
//        {
//            return mMax_Base_Pack_Info.webpage_url;
//        }

//        public string GetBasePackDownloadUrl()
//        {
//            return mMax_Base_Pack_Info.base_pack_url;
//        }







//        private void emm()
//        {
//            Debug.Log(mMax_Version);
            
//        }

//        /// <summary>
//        /// 母包信息
//        /// </summary>
//        [System.Serializable]
//        public struct BasePackInfo
//        {
//            public int base_version;
//            public string upgrade_type;
//            public string download_type;
//            public string webpage_url;
//            public string base_pack_url; //母包下载地址
//            public PatchInfo[] packages;
//        }

//        /// <summary>
//        /// 补丁包信息
//        /// </summary>
//        [System.Serializable]
//        public struct PatchInfo
//        {
//            public int version;
//            public string md5;
//            public string url;
//            public long size;
//        }
//    }
//}
