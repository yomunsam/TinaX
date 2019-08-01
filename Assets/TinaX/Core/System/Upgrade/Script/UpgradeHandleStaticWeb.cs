using UnityEngine;
using System.Collections;
using System;
using UniRx;
using UniRx.Async;
using UnityEngine.Networking;


namespace TinaX.Upgrade
{
    /// <summary>
    /// 静态Web 热更新管理器
    /// </summary>
    public class UpgradeHandleStaticWeb
    {


        private UpgradeMgr mUpgradeMgr;

        private StaticUpgradeConfigJsonTpl mUpgradeTpl; //静态更新数据模板
        


        public UpgradeHandleStaticWeb(UpgradeMgr _baseMgr)
        {
            mUpgradeMgr = _baseMgr;
        }


        /// <summary>
        /// 检查更新
        /// </summary>
        /// <param name="IndexUrls">更新Index组</param>
        /// <param name="ResultsCallback">检查结果回调</param>
        /// <param name="BaseVersion">母包版本</param>
        /// <param name="PatchVersion">补丁包版本</param>
        /// <param name="CheckUrlCallback">检查Url过程回调</param>
        public void CheckUpgrade(string[] IndexUrls,int BaseVersion,int PatchVersion, Action<EUpgradeCheckResults> ResultsCallback, Action<int> CheckUrlCallback = null)
        {
            int i = 0;
            

            void DoItOnce(string IndexUrl) // 用来递归的函数
            {
                if(CheckUrlCallback != null)
                {
                    CheckUrlCallback(i);
                }
                XLog.Print("检查更新,获取更新索引：" + IndexUrl);
                CheckupgradeSingle(IndexUrl, (isSuccess, JsonText, ErrType) => 
                {
                    if (isSuccess)
                    {
                        //成功
                        var jsonObj = JsonUtility.FromJson<StaticUpgradeConfigJsonTpl>(JsonText);
                        mUpgradeTpl = jsonObj;
                        //检查更新结果类型
                        if(mUpgradeTpl.IsNeedUpgrade(BaseVersion, PatchVersion))
                        {
                            //需要更新
                            ResultsCallback(EUpgradeCheckResults.upgrade);
                        }
                        else
                        {
                            //已经是最新的
                            ResultsCallback(EUpgradeCheckResults.newest);
                        }
                        return;
                    }
                    else
                    {
                        XLog.Print("检查更新失败：" + ErrType.ToString());
                        //失败
                        i++;
                        if (IndexUrls.Length - 1 >= i)
                        {
                            //递归
                            DoItOnce(IndexUrls[i]);
                        }
                        else
                        {
                            //全部检查过了，对外告知检查失败
                            //把最后一次检查的报错放出去
                            switch (ErrType)
                            {
                                case EGetIndexJsonErrorType.connect_error:
                                    ResultsCallback(EUpgradeCheckResults.connect_error);
                                    break;
                                case EGetIndexJsonErrorType.url_error:
                                    ResultsCallback(EUpgradeCheckResults.error);
                                    break;
                                default:
                                    ResultsCallback(EUpgradeCheckResults.error);
                                    break;
                            }
                            return;
                        }
                    }
                });
            }


            if(IndexUrls.Length -1 >= i)
            {
                DoItOnce(IndexUrls[i]);
            }
        }




        /// <summary>
        /// 对单条Url检查更新
        /// </summary>
        /// <param name="IndexUrl"></param>
        /// <param name="callback">检查回调， 参数1：检查是否成功 参数2:如果检查更新成功，返回收到的JSON 参数3：如果出错，返回出错类型</param>
        private void CheckupgradeSingle(string IndexUrl, Action<bool,string, EGetIndexJsonErrorType> callback)
        {
            
            if (IndexUrl.IsNullOrEmpty())
            {
                callback(false,"",EGetIndexJsonErrorType.url_error);
                return;
            }
            var task = GetIndexJson(IndexUrl,
                (isSuccess, JsonText) =>
                {
                    if (isSuccess)
                    {
                        callback(true, JsonText, EGetIndexJsonErrorType.none);
                    }
                    else
                    {
                        //更新失败
                        callback(false,JsonText, EGetIndexJsonErrorType.connect_error);
                    }
                });
        }




        /// <summary>
        /// 获取静态更新索引Json文件
        /// </summary>
        /// <param name="url">json url</param>
        /// <param name="callback">处理回调,参数1： 是否成功 参数2：成功获取的内容</param>
        /// <returns></returns>
        private async UniTask GetIndexJson(string url, Action<bool,string> callback)
        {
            var task = WebGetTextAsync(UnityWebRequest.Get(url));
            var json_data = await UniTask.WhenAll(task);
            
            if(json_data.Length >= 1)
            {
                if(callback != null)
                {
                    callback(true, json_data[0]);
                }
            }
            else
            {
                if(callback != null)
                {
                    callback(false, "");
                }
            }
        }

        /// <summary>
        /// GET: 文本
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        private async UniTask<string> WebGetTextAsync(UnityWebRequest req)
        {
            var op = await req.SendWebRequest();
            return op.downloadHandler.text;
        }



        /// <summary>
        /// 获取索引JSON错误
        /// </summary>
        private enum EGetIndexJsonErrorType
        {
            /// <summary>
            /// 没错
            /// </summary>
            none,
            /// <summary>
            /// url格式错误
            /// </summary>
            url_error,
            /// <summary>
            /// 连接服务器出错
            /// </summary>
            connect_error,
        }

    }
}
