using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace TinaX.UIKits
{

    //UIKit UISafeArea 数据模型
    public class XUISafeAreaModel
    {

        public List<XUISafeAreaItemModel> data;

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
            foreach(var item in data)
            {
                if (item.regular)
                {
                    //正则匹配
                    if (Regex.IsMatch(myDeviceName, item.device_regular))
                    {
                        return item;
                    }
                }
                else
                {
                    if(item.device_name == myDeviceName)
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
        public string device_name { get; set; }
        /// <summary>
        /// 厂商名 品牌名
        /// </summary>
        public string brand { get; set; }
        /// <summary>
        /// 是否启用正则匹配设备名
        /// </summary>
        public bool regular { get; set; }

        /// <summary>
        /// 设备名正则表达式
        /// </summary>
        public string device_regular { get; set; }

        /// <summary>
        /// 是否为畸形屏幕
        /// </summary>
        public bool abnormal { get; set; }

        /// <summary>
        /// 设备顶部缩进
        /// </summary>
        public int top { get; set; }

        /// <summary>
        /// 设备底部缩进
        /// </summary>
        public int bottom { get; set; }

        /// <summary>
        /// 设备左边
        /// </summary>
        public int left { get; set; }

        /// <summary>
        /// 设备右边
        /// </summary>
        public int right { get; set; }

    }

}
