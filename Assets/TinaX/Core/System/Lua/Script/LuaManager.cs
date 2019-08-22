#if TinaX_CA_LuaRuntime_Enable

using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TinaX;
using XLua;

namespace TinaX.Lua
{
    public class LuaManager:ILuaMgr
    {
        public static LuaEnv LuaVM
        {
            get
            {
                return m_LuaVM;
            }
        }
        public static bool Inited
        {
            get;private set;
        }


        #region rumtime
        private static LuaEnv m_LuaVM;

        private static float lastGCTime = 0;
        private const float GCInterval = 1; //1 second
        private string mFileExt; //文件后缀名
        private string mTinaXFileExt = ".lua.txt"; //在TinaX内部的Lua文件的后缀名
        private string mTinaXFileRootPath;  //在TinaX内部的Lua文件的根目录

        private List<LoadFileHandler> mLoadFileHander = new List<LoadFileHandler>();

        private LuaEnv.CustomLoader m_loader;

        private LuaConfig mLuaConfig;

        private IVFS mVFSMgr;

        /// <summary>
        /// 是否启用了Lua
        /// </summary>
        private bool EnableLua
        {
            get
            {
                if(mLuaConfig == null)
                {
                    return false;
                }
                return mLuaConfig.EnableLua;
            }
        }

        #endregion

        public LuaManager(IVFS vfsMgr)
        {
            mVFSMgr = vfsMgr;
            //先获取配置，检查是否启用Lua
            mLuaConfig = Config.GetTinaXConfig<LuaConfig>(Conf.ConfigPath.lua);
            mTinaXFileRootPath = Setup.Framework_Path + "/Lua/";
            if (EnableLua)
            {
                //获取lua文件的后缀名
                switch (mLuaConfig.FileExten)
                {
                    default:
                        mFileExt = ".txt";
                        break;
                    case LuaFileExten.lua:
                        mFileExt = ".lua";
                        break;
                    case LuaFileExten.lua_txt:
                        mFileExt = ".lua.txt";
                        break;
                    case LuaFileExten.txt:
                        mFileExt = ".txt";
                        break;
                }
                //初始化lua运行环境
                m_LuaVM = new LuaEnv();
                //lua 文件加载方法定义
                m_loader = LoadLuaCodeFile;
                m_LuaVM.AddLoader(m_loader);

                TimeMachine.I.AddUpdate(XUpdate);

                //添加特殊load路径处理规则
                mLoadFileHander.Add(new LoadFileHandler()
                {
                    IsTrigger = (path) =>
                    {
                        return path.StartsWith(mTinaXFileRootPath);
                    },
                    Handler = (ref string fileName,out byte[] reData, out bool handled_filename) =>
                    {
                        fileName += mTinaXFileExt;
                        reData = default;
                        handled_filename = true;
                        return false; //如果不想中断的话，一定要为false
                    }
                });

                //把IDE调试相关代码移到这里
                IDEDebugHandler(); //改动：部分IDE调试工具支持编译后远程调试，所以这里把“只在编辑器下启动调试”的限制去掉了



                m_LuaVM.DoString(@"require('" + Setup.Framework_Lua_Init + "')", "init");

                Inited = true;
            }

            


            
        }

        public void Start()
        {

        }

        public LuaConfig GetConfig()
        {
            return mLuaConfig;
        }

        public void RunFile(string _path)
        {

            var file_path = _path;
            //这里的RunFile负责把path的后缀扒干净了，包括".lua.txt"这种两个点的后缀，后缀的添加规则在加载文件的地方，这里不管
            while (file_path.IndexOf('.') >= 0)
            {
                //包含后缀名
                file_path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file_path), System.IO.Path.GetFileNameWithoutExtension(file_path));
            }
            //为什么这里不用正则呢，因为实测发现效率有点低


            file_path = file_path.Replace("\\", "/");
            //m_LuaVM.DoString(@"require('" + file_path + "')", System.IO.Path.GetFileNameWithoutExtension(_path));
            m_LuaVM.DoString(@"require('" + file_path + "')");  //这里好像不应该加路径
            
        }


        /// <summary>
        /// 运行Lua function
        /// </summary>
        /// <param name="FuncPath">Global下的Lua function 的Path</param>
        public void RunLuaFunc(string FuncPath)
        {
            m_LuaVM?.Global.GetInPath<System.Action>(FuncPath)?.Invoke();
        }

        public R RunLuaFuncR<R>(string FuncPath)
        {
            Func<R> func = LuaVM.Global.GetInPath<Func<R>>(FuncPath);
            if (func != null)
            {
                return func();
            }
            else
            {
                return default;
            }

        }

        public void RunLuaFunc<T>(string FuncPath,T Param1)
        {
            m_LuaVM?.Global.GetInPath<Action<T>>(FuncPath)?.Invoke(Param1);
        }

        public R RunLuaFuncR<T,R>(string FuncPath, T Param1)
        {
            Func<T,R> func = m_LuaVM?.Global.GetInPath<Func<T, R>>(FuncPath);
            if(func != null)
            {
                return func(Param1);
            }
            else
            {
                return default(R);
            }
        }

        public void RunLuaFunc<T1,T2>(string FuncPath, T1 Param1 , T2 Param2)
        {
            m_LuaVM.Global.GetInPath<Action<T1, T2>>(FuncPath)
                ?.Invoke(Param1, Param2);
        }

        public R RunLuaFunc<T1,T2,R>(string FuncPath, T1 Param1, T2 Param2)
        {
            Func<T1, T2, R> func = m_LuaVM?.Global.GetInPath<Func<T1, T2, R>>(FuncPath);
            if (func != null)
            {
                return func(Param1, Param2);
            }
            else
            {
                return default;
            }
        }

        public void DoString(string LuaCodeStr,string chunkName = "chunk",LuaTable luaEnv = null)
        {
            LuaManager.m_LuaVM?.DoString(LuaCodeStr, chunkName, luaEnv);
        }


        private void XUpdate()
        {
            if (Time.time - lastGCTime > GCInterval)
            {
                m_LuaVM.Tick();
                lastGCTime = Time.time;
            }
        }



        private byte[] LoadLuaCodeFile(ref string fileName)
        {
            
            if(fileName.IndexOf('.') != -1)
            {
                fileName = fileName.Replace('.','/');
            }


            //上面那堆改造成酱子：

            var flag = false;
            bool handled_fileName = false;
            byte[] tempData = default;
            foreach(var item in mLoadFileHander)
            {
                if (item.IsTrigger(fileName))
                {
                    //命中规则
                    if(item.Handler(ref fileName,out tempData,out handled_fileName))
                    {
                        //有一个handler中断了处理
                        flag = true;
                        break;
                    }
                }
            }

            if (flag) 
            {
                //被中断
                return tempData;

            }

            //没有被特殊规则中断处理，继续往下执行

            if (!handled_fileName)
            {
                //附上后缀名
                fileName += mFileExt;
            }


            #region 注释掉的东西,大概率是不要了，但是先留着
            //#if UNITY_EDITOR
            //            //编辑器下特殊加载处理

            //            //编辑器下尝试加载LuaIDE - LuaDebug调试文件
            //            if (fileName == "LuaDebug.txt")
            //            {
            //                var luadebug_filepath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), fileName);
            //                //检查文件是否存在
            //                if (System.IO.File.Exists(luadebug_filepath))
            //                {
            //                    var luadebug_file = System.IO.File.ReadAllBytes(luadebug_filepath);
            //                    return luadebug_file;
            //                }
            //                else
            //                {
            //                    return null;
            //                }
            //            }

            //#endif
            #endregion


            
            var codeText = mVFSMgr.LoadAsset<TextAsset>(fileName);
            if (codeText == null)
            {
                XLog.PrintE("[TinaX][LuaManager]File Not Found :" + fileName);
                return null;
            }
            else
            {
                return codeText.bytes;
            }
            
        }



        /// <summary>
        /// IDE调试扩展
        /// </summary>
        private void IDEDebugHandler()
        {
            bool enable_debug = Debug.isDebugBuild;
#if UNITY_EDITOR
            enable_debug = true;
#endif
            if (enable_debug && mLuaConfig != null)
            {
                var xdebug_file_path = Setup.Framework_Lua_RootPath + "/" + LuaIDEDebugConst.Lua_IdeDebug_RootPath + "/xdebug";
                m_LuaVM.DoString(@"require('" + xdebug_file_path + "')", xdebug_file_path + mTinaXFileExt);

                //luaide
                if (mLuaConfig.Debug_LuaIDE_Enable)
                {
                    this.RunLuaFunc<string,int>("XCore.XDebug.StartLuaIDE",mLuaConfig.Debug_LuaIDE_Addr,mLuaConfig.Debug_LuaIDE_Port);
                }

#if UNITY_EDITOR
                //lua perfect
                //这个看起来好像只能再编辑器里面用，先包一层起来
                if (mLuaConfig.Debug_LuaPerfact_Enable)
                {
                    //先添加特定规则
                    mLoadFileHander.Add(new LoadFileHandler()
                    {
                        IsTrigger = (path) =>
                        {
                            return path.Equals("LuaDebuggee");
                        },
                        Handler = (ref string fileName, out byte[] reData, out bool handled_filename) =>
                        {
                            reData = default;
                            handled_filename = true;
                            return true;
                        }
                    });

                    this.RunLuaFunc<string, int>("XCore.XDebug.StartLuaPerfect", mLuaConfig.Debug_LuaPerfact_Addr, mLuaConfig.Debug_LuaPerfact_Port);

                }
#endif

            }

        }



        /// <summary>
        /// 处理 loadfile的特殊方法
        /// </summary>
        public struct LoadFileHandler
        {
            //public string Path;
            public Func<string, bool> IsTrigger;    //是否命中规则

            /// <summary>
            /// 特殊处理方法的定义
            /// </summary>
            /// <param name="fileName">引用 欲加载的文件名</param>
            /// <param name="retValue">return 为 true时的返回值</param>
            /// <param name="handledFileName">已经处理过后缀名了，不需要接受其他多余的处理</param>
            /// <returns>是否中断后续处理并直接返回retValue的值</returns>
            public delegate bool HanderFuncDg(ref string fileName,out byte[] retValue, out bool handledFileName);   //处理方法
            public HanderFuncDg Handler;
        }

    }

    /// <summary>
    /// Lua脚本文件的后缀名
    /// </summary>
    [System.Serializable]
    public enum LuaFileExten
    {
        [Header(".txt")]
        txt,
        [Header(".lua.txt")]
        lua_txt,
        [Header(".lua (不推荐)")]
        lua

    }
}



#endif