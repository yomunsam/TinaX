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

        private LuaEnv.CustomLoader m_loader;

        private LuaConfig mLuaConfig;

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

        public LuaManager()
        {
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
                m_LuaVM = new LuaEnv();
                m_loader = LoadLuaCodeFile;
                m_LuaVM.AddLoader(m_loader);
                TimeMachine.I.AddUpdate(XUpdate);

                m_LuaVM.DoString(@"require('" + Setup.Framework_Path + "/Lua/core/init" + "')", "init");

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
            Action func =  LuaVM.Global.GetInPath<System.Action>(FuncPath);
            if(func != null)
            {
                func();
            }
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
                return default(R);
            }
        }

        public void RunLuaFunc<T>(string FuncPath,T Param1)
        {
            Action<T> func = LuaVM.Global.GetInPath<Action<T>>(FuncPath);
            if(func != null)
            {
                func(Param1);
            }
        }

        public R RunLuaFuncR<T,R>(string FuncPath, T Param1)
        {
            Func<T,R> func = LuaVM.Global.GetInPath<Func<T, R>>(FuncPath);
            if(func != null)
            {
                return func(Param1);
            }
            else
            {
                return default(R);
            }
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
            if (fileName.StartsWith(mTinaXFileRootPath))
            {
                //TinaX内部文件
                fileName = fileName + mTinaXFileExt;
            }
            else
            {
                fileName = fileName + mFileExt;
            }
            

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

            var codeText = AssetsMgr.Instance.LoadAsset<TextAsset>(fileName);
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