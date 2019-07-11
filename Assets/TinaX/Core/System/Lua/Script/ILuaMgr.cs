#if TinaX_CA_LuaRuntime_Enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.Lua
{
    public interface ILuaMgr
    {
        void Start();

        LuaConfig GetConfig();

        void RunFile(string path);

        /// <summary>
        /// 运行Lua function
        /// </summary>
        /// <param name="FuncPath">Global下的Lua function 的Path</param>
        void RunLuaFunc(string FuncPath);

        /// <summary>
        /// 运行Lua Global Function，不传入任何参数，但获取一个返回值
        /// </summary>
        /// <typeparam name="R">返回值类型</typeparam>
        /// <param name="FuncPath">Global下的Lua function 的Path</param>
        /// <returns></returns>
        R RunLuaFuncR<R>(string FuncPath);

        /// <summary>
        /// 运行Lua Global Function，附带传入一个参数
        /// </summary>
        /// <typeparam name="T">附带参数的类型</typeparam>
        /// <param name="FuncPath">Global下的Lua function 的Path</param>
        /// <param name="Param1">参数内容</param>
        void RunLuaFunc<T>(string FuncPath, T Param1);


        /// <summary>
        /// 运行Lua Global Function，附带传入一个参数，并接收一个返回值
        /// </summary>
        /// <typeparam name="T">传入参数的类型</typeparam>
        /// <typeparam name="R">返回值的类型</typeparam>
        /// <param name="FuncPath">Global下的Lua function 的Path</param>
        /// <param name="Param1">参数内容</param>
        /// <returns></returns>
        R RunLuaFuncR<T, R>(string FuncPath, T Param1);
    }
}


#endif