using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TinaX.UIKit
{
    //UIKit UISafeArea 数据模型

    [JsonObject]
    public class XUISafeAreaModel
    {
        [JsonProperty(PropertyName ="data")]
        public List<XUISafeAreaItemModel> Data;

        public XUISafeAreaItemModel CurDeviceInfo
        {
            get
            {
                if (!flag)
                {
                    mCurDeviceInfo = GetCurDeviceInfo();
                }
                return mCurDeviceInfo;
            }
        }


        private bool flag = false; //是否查询过设备信息的标记

        /// <summary>
        /// 当前设备信息
        /// </summary>
        private XUISafeAreaItemModel mCurDeviceInfo; 
        
        /// <summary>
        /// 获取当前设备信息
        /// </summary>
        /// <returns></returns>
        private XUISafeAreaItemModel GetCurDeviceInfo()
        {
            var myDeviceName = UnityEngine.SystemInfo.deviceModel;
            foreach(var item in Data)
            {
                if (item.EnableRegular)
                {
                    //正则匹配
                    if (Regex.IsMatch(myDeviceName, item.Device_RegularStr))
                    {
                        return item;
                    }
                }
                else
                {
                    if(item.Device_name == myDeviceName)
                    {
                        return item;
                    }
                }
            }
            return default(XUISafeAreaItemModel);
        }

    }

    
    public struct XUISafeAreaItemModel
    {
        /// <summary>
        /// 设备名
        /// </summary>
        [JsonProperty(PropertyName = "device_name")]
        public string Device_name { get; set; }
        /// <summary>
        /// 厂商名 品牌名
        /// </summary>
        [JsonProperty(PropertyName = "brand")]
        public string BrandName { get; set; }
        /// <summary>
        /// 是否启用正则匹配设备名
        /// </summary>
        [JsonProperty(PropertyName = "regular")]
        public bool EnableRegular { get; set; }

        /// <summary>
        /// 设备名正则表达式
        /// </summary>
        [JsonProperty(PropertyName = "device_regular")]
        public string Device_RegularStr { get; set; }

        /// <summary>
        /// 是否为畸形屏幕
        /// </summary>
        [JsonProperty(PropertyName = "abnormal")]
        public bool IsAbnormal { get; set; }

        /// <summary>
        /// 设备顶部缩进
        /// </summary>
        [JsonProperty(PropertyName = "top")]
        public int Top { get; set; }

        /// <summary>
        /// 设备底部缩进
        /// </summary>
        [JsonProperty(PropertyName = "bottom")]
        public int Bottom { get; set; }

        /// <summary>
        /// 设备左边
        /// </summary>
        [JsonProperty(PropertyName = "left")]
        public int Left { get; set; }

        /// <summary>
        /// 设备右边
        /// </summary>
        [JsonProperty(PropertyName = "right")]
        public int Right { get; set; }

    }

}
