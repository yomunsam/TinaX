/*
 * 设计上，本class不直接暴露给开发者
 * 本class由XUIMgrGateway管理
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace TinaX.UIKit
{
    
    public class XUISafeAreaMgr
    {
        private UIKitConfig mUIConfig;
        private bool mIsEnable;

        private XUISafeAreaModel mModel;
        private XUISafeAreaItemModel mCurDeviceInfo;


        public XUISafeAreaMgr()
        {
            mUIConfig = TinaX.Config.GetTinaXConfig<UIKitConfig>(Conf.ConfigPath.uikit);
            if (mUIConfig == null)
            {
                mUIConfig = new UIKitConfig();
                mUIConfig.Enable_UISafeArea = false;
                mIsEnable = false;
            }



            mModel = LoadConfigFile();
            mCurDeviceInfo = mModel.CurDeviceInfo;

        }

        /// <summary>
        /// 启用UI安全区
        /// </summary>
        public XUISafeAreaMgr Enable()
        {
            if (mIsEnable)
            {
                return this;
            }

            mIsEnable = true;
            return this;
        }

        /// <summary>
        /// 停用UI安全区
        /// </summary>
        public void Disable()
        {
            if (!mIsEnable)
            {
                return;
            }

            mIsEnable = false;
        }


        public void GetOffsetInfo()
        {
            
        }


        #region IO

        private XUISafeAreaModel LoadConfigFile()
        {
            string config_json;
            if (mUIConfig.UISafeArea_LoadByVFS)
            {
                config_json = TinaX.AssetsMgr.I.LoadAsset<TextAsset>(mUIConfig.UI_SafeArea_Json)?.text;
            }
            else
            {
                config_json = Resources.Load<TextAsset>(mUIConfig.UI_SafeArea_Json)?.text;
            }
            if (config_json.IsNullOrEmpty())
            {
                return null;
            }

            return JsonConvert.DeserializeObject<XUISafeAreaModel>(config_json);

        }


        #endregion



        #region Data Read/Write




        #endregion



    }

}
