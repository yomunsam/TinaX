using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class TinaX_VFSKit_ISceneAsset_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(TinaX.VFSKit.ISceneAsset);
            args = new Type[]{typeof(UnityEngine.SceneManagement.LoadSceneMode)};
            method = type.GetMethod("OpenScene", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OpenScene_0);


        }


        static StackObject* OpenScene_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.SceneManagement.LoadSceneMode @loadMode = (UnityEngine.SceneManagement.LoadSceneMode)typeof(UnityEngine.SceneManagement.LoadSceneMode).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            TinaX.VFSKit.ISceneAsset instance_of_this_method = (TinaX.VFSKit.ISceneAsset)typeof(TinaX.VFSKit.ISceneAsset).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OpenScene(@loadMode);

            return __ret;
        }



    }
}
