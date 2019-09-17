using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using CatLib;
using TinaX.Conf;
using TinaX.VFSKit;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace TinaX
{
    public class XCore
    {
        #region Instance

        private static XCore _instance;
        public static XCore I
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new XCore();
                }
                return _instance;
            }
        }
        public static XCore Instance
        {
            get { return I; }
        }
        
        #endregion

        #region Info

        /// <summary>
        /// framework version name
        /// </summary>
        public string version_name
        {
            get
            {
                return FrameworkInfo.FrameworkVersionName;
            }
        }

        /// <summary>
        /// framework version code
        /// </summary>
        public int version_code
        {
            get
            {
                return FrameworkInfo.FrameworkVersionCode;
            }
        }

        /// <summary>
        /// 框架的沙箱存储路径
        /// </summary>
        public string LocalStorage_TinaX
        {
            get
            {
                return UnityEngine.Application.persistentDataPath + "/" + Setup.Framework_LocalStorage_TinaX;
            }
        }

        /// <summary>
        /// App的沙箱存储路径（提供给框架使用开发者）
        /// </summary>
        public string LocalStorage_App
        {
            get
            {
                return UnityEngine.Application.persistentDataPath + "/" + Setup.Framework_LocalStorage_App;
            }
        }

        /// <summary>
        /// 框架的基础GameObject
        /// </summary>
        public GameObject BaseGameObject
        {
            get
            {
                return mBaseGameObject;
            }
        }

        

        #endregion

        #region Runtime
        private bool m_inited = false;
        private CatLib.Application m_catlib_app;
        private MainConfig mMainConfig;
        private GameObject mBaseGameObject;
        private List<IXBootstrap> mXBootstrapClassList;
        #endregion

        #region Action

        /// <summary>
        /// 框架软重启时触发的action，如果业务逻辑有需要的，在这时候顺带着处理下吧
        /// </summary>
        [Obsolete("please use interface \"TinaX.IXBootstrap\" instead it.")]
        public static Action OnFrameworkRestart; 

        #endregion

        public async Task<XCore> Init(MainConfig mainConfig)
        {
            if (m_inited) { return this; }
            m_inited = true;

            XLog.Print("[TinaX Framework] TinaX6 - v." + version_name + "    | Nekonya Studio | Corala.Space Project | Powerd by yomunsam - www.yomunchan.moe");

            mMainConfig = mainConfig;

            //生成一个全局的GameObject
            mBaseGameObject = GameObjectHelper.FindOrCreateGo(Setup.Framework_Base_GameObject)
                .DontDestroy()
                .SetPosition(new Vector3(-1000, -1000, -1000));

            //初始化配置与变量


            //启动引导系统
            m_catlib_app = new CatLib.Application();
            m_catlib_app.OnFindType((t) => Type.GetType(t));
            m_catlib_app.Bootstrap(new XBootstrap());
            m_catlib_app.Init();

            //TinaX 6.5 主动启动引导
            var _b_type = typeof(IXBootstrap);
            mXBootstrapClassList = new List<IXBootstrap>();
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(_b_type)))
                .ToArray();

            foreach(var type in types)
            {
                mXBootstrapClassList.Add((IXBootstrap)Activator.CreateInstance(type));
            }
                
            //Invoke 启动主动引导
            foreach(var item in mXBootstrapClassList)
            {
                item.OnInit();
            }

            //管理器等初始化工作
            await InitSystemsCtor();

            foreach(var item in mXBootstrapClassList)
            {
                item.OnStart();
            }

            //检查和处理自动更新
            HandleAutoUpgrade(() =>
            {
                //因为更新操作是异步的，所以接下来要执行的东西都在这个回调里

                StartupApp();
            });

            
            


            return this;
        }

        /// <summary>
        /// 软重启App
        /// </summary>
        /// <returns></returns>
        public XCore RestartAppInApp()
        {
            Debug.Log("[TinaX] Framework开始软重启");

            _instance = null;
            m_inited = false;

            #region 处理其他的资源释放之类的


            #endregion

            App.Terminate(); //停用CatLib


            XStart.RestartFramework();
#pragma warning disable 0618
            OnFrameworkRestart?.Invoke();
#pragma warning restore 0618
            foreach(var item in mXBootstrapClassList)
            {
                item.OnAppRestart();
            }

            return this;
        }

        /// <summary>
        /// App Version
        /// </summary>
        public int AppVersion => mMainConfig.Version_Code;



        private async Task InitSystemsCtor()
        {
            await VFS.I.CtorAsync();
            XI18N.Instance.Start();
#if TinaX_CA_LuaRuntime_Enable
            LuaScript.I.Start();
#endif
        }

        private void StartupApp()
        {

#if TinaX_CA_LuaRuntime_Enable
            var luaConfig = Config.GetTinaXConfig<Lua.LuaConfig>(Conf.ConfigPath.lua);
            if (luaConfig != null)
            {
                if (!luaConfig.LuaScriptStartup.IsNullOrEmpty() && luaConfig.EnableLua)
                {
                    LuaScript.I.RunFile(luaConfig.LuaScriptStartup);
                }
            }
#endif

            if (mMainConfig.Startup_Scene != null && mMainConfig.Startup_Scene != "")
            {
                if (SceneMgr.Instance.GetActiveSceneName() != mMainConfig.Startup_Scene)
                {
                    SceneMgr.Instance.OpenScene(mMainConfig.Startup_Scene);
                }
            }

            Debug.Log("app startup finish:" + System.DateTime.UtcNow.ToLongTimeString());
        }

        /// <summary>
        /// 处理自动更新
        /// </summary>
        /// <param name="OnFinish"></param>
        private void HandleAutoUpgrade(Action OnFinish)
        {

            var mUpgradeConfig = TinaX.Config.GetTinaXConfig<TinaX.Upgrade.UpgradeConfig>(TinaX.Conf.ConfigPath.upgrade);
            if (mUpgradeConfig == null)
            {
                OnFinish();
                return;
            }

            if (!mUpgradeConfig.Auto_Upgrade)
            {
                //不需要框架层处理自动更新
                OnFinish();
                return;
            }


#if UNITY_EDITOR
            //编辑器模式下，检查下要不要检查更新：如果使用AssetBundle包模拟加载，则检查更新，否则检查个锤子
            //编辑器模式下，需要判断，是从哪儿加载资源
            if (!VFSKit.VFSKit.IsUseAssetBundleInEdtor())
            {
                //直接使用编辑器加载策略
                //这种情况下，不用检查更新
                OnFinish();
                XLog.Print("Framework 热更新：在"+XLog.GetColorString_Blue("编辑器模式") +"下且未采用资源包方式加载资源，故忽略热更新检查。");
                return;
            }

#endif

            //TinaX.Upgrade.AutoUpgradeUI_Mgr.Start((res)=> {


            //    OnFinish();
            //});

        }

    }
}
