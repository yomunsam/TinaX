using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UniRx.Async;

namespace TinaX
{
    public static class XWeb
    {
        /// <summary>
        /// Download file from web and write to local path ,if it not exists . 如果给定的本地路径不存在文件，则从web下载 
        /// </summary>
        /// <param name="web_uri"></param>
        /// <param name="local_path"></param>
        /// <returns>local path 返回本地路径</returns>
        public static async UniTask<string> DownloadFileIfNotExist(Uri web_uri, string local_path)
        {
            if (File.Exists(local_path))
            {
                return local_path;
            }
            else
            {
                //不存在，从web下载到本地
                var req = UnityWebRequest.Get(web_uri);
                var op = req.SendWebRequest();
                await Task.Delay(1);
                var result = await op;
                var data = result.downloadHandler.data;
                if (!File.Exists(local_path))
                {
                    File.WriteAllBytes(local_path, data);
                }
                return local_path;
            }
        }


        public static async UniTask<string> GetText(Uri web_uri)
        {
            var req_op = UnityWebRequest
                .Get(web_uri)
                .SendWebRequest();
            await Task.Delay(1);
            var result = await req_op;

            //状态
            if (result.isHttpError)
            {
                throw new TinaX.Exceptions.XWebException("GET Text HTTP Error, uri:" + web_uri.ToString() + "  err:" + result.error, (System.Net.HttpStatusCode)result.responseCode);
            }else if (result.isNetworkError)
            {
                throw new TinaX.Exceptions.XWebException("GET Text Network Error, uri:" + web_uri.ToString() + "  err:" + result.error,Exceptions.XWebException.ErrorType.NetworkError, (System.Net.HttpStatusCode)result.responseCode);
            }

            return result.downloadHandler.text;
        }

    }
}
