#if TinaX_CA_LuaRuntime_Enable

using XLua;

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

        /// <summary>
        /// 运行Lua Global Function，附带传入2个参数
        /// </summary>
        /// <typeparam name="T1">传入参数1的类型</typeparam>
        /// <typeparam name="T2">传入参数2的类型</typeparam>
        /// <param name="FuncPath">Global下的Lua function 的Path</param>
        /// <param name="Param1">参数1内容</param>
        /// <param name="Param2">参数2内容</param>
        void RunLuaFunc<T1, T2>(string FuncPath, T1 Param1, T2 Param2);

        /// <summary>
        /// 运行Lua Global Function，附带传入2个参数，并接收一个返回值
        /// </summary>
        /// <typeparam name="T1">传入参数1的类型</typeparam>
        /// <typeparam name="T2">传入参数2的类型</typeparam>
        /// <typeparam name="R">返回值的类型</typeparam>
        /// <param name="FuncPath">Global下的Lua function 的Path</param>
        /// <param name="Param1">参数1内容</param>
        /// <param name="Param2">参数2内容></param>
        /// <returns></returns>
        R RunLuaFunc<T1, T2, R>(string FuncPath, T1 Param1, T2 Param2);

        /// <summary>
        /// DoString
        /// </summary>
        /// <param name="LuaCodeStr">Lua 代码</param>
        /// <param name="chunkName"></param>
        /// <param name="luaEnv"></param>
        void DoString(string LuaCodeStr, string chunkName = "chunk", LuaTable luaEnv = null);

    }
}


#endif