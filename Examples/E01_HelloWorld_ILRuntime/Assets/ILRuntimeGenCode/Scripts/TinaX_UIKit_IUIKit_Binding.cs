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
    unsafe class TinaX_UIKit_IUIKit_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(TinaX.UIKit.IUIKit);
            args = new Type[]{typeof(System.String), typeof(TinaX.XComponent.XBehaviour), typeof(System.Action<TinaX.UIKit.IUIEntity, TinaX.XException>), typeof(System.Object[])};
            method = type.GetMethod("OpenUIAsync", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OpenUIAsync_0);
            args = new Type[]{typeof(System.String), typeof(TinaX.XComponent.XBehaviour), typeof(System.Object[])};
            method = type.GetMethod("OpenUIAsync", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OpenUIAsync_1);
            args = new Type[]{typeof(System.String), typeof(TinaX.XComponent.XBehaviour), typeof(TinaX.UIKit.OpenUIParam), typeof(System.Object[])};
            method = type.GetMethod("OpenUIAsync", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OpenUIAsync_2);


        }


        static StackObject* OpenUIAsync_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 5);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object[] @args = (System.Object[])typeof(System.Object[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Action<TinaX.UIKit.IUIEntity, TinaX.XException> @callback = (System.Action<TinaX.UIKit.IUIEntity, TinaX.XException>)typeof(System.Action<TinaX.UIKit.IUIEntity, TinaX.XException>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            TinaX.XComponent.XBehaviour @behaviour = (TinaX.XComponent.XBehaviour)typeof(TinaX.XComponent.XBehaviour).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.String @UIName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            TinaX.UIKit.IUIKit instance_of_this_method = (TinaX.UIKit.IUIKit)typeof(TinaX.UIKit.IUIKit).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OpenUIAsync(@UIName, @behaviour, @callback, @args);

            return __ret;
        }

        static StackObject* OpenUIAsync_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object[] @args = (System.Object[])typeof(System.Object[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            TinaX.XComponent.XBehaviour @behaviour = (TinaX.XComponent.XBehaviour)typeof(TinaX.XComponent.XBehaviour).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.String @UIName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            TinaX.UIKit.IUIKit instance_of_this_method = (TinaX.UIKit.IUIKit)typeof(TinaX.UIKit.IUIKit).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.OpenUIAsync(@UIName, @behaviour, @args);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* OpenUIAsync_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 5);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object[] @args = (System.Object[])typeof(System.Object[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            TinaX.UIKit.OpenUIParam @openUIParam = (TinaX.UIKit.OpenUIParam)typeof(TinaX.UIKit.OpenUIParam).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            TinaX.XComponent.XBehaviour @behaviour = (TinaX.XComponent.XBehaviour)typeof(TinaX.XComponent.XBehaviour).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.String @UIName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            TinaX.UIKit.IUIKit instance_of_this_method = (TinaX.UIKit.IUIKit)typeof(TinaX.UIKit.IUIKit).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.OpenUIAsync(@UIName, @behaviour, @openUIParam, @args);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
