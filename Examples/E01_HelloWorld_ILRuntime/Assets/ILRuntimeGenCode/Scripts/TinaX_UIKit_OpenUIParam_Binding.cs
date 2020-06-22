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
    unsafe class TinaX_UIKit_OpenUIParam_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(TinaX.UIKit.OpenUIParam);

            field = type.GetField("UseMask", flag);
            app.RegisterCLRFieldGetter(field, get_UseMask_0);
            app.RegisterCLRFieldSetter(field, set_UseMask_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_UseMask_0, AssignFromStack_UseMask_0);
            field = type.GetField("CloseByMask", flag);
            app.RegisterCLRFieldGetter(field, get_CloseByMask_1);
            app.RegisterCLRFieldSetter(field, set_CloseByMask_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_CloseByMask_1, AssignFromStack_CloseByMask_1);

            args = new Type[]{};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }



        static object get_UseMask_0(ref object o)
        {
            return ((TinaX.UIKit.OpenUIParam)o).UseMask;
        }

        static StackObject* CopyToStack_UseMask_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((TinaX.UIKit.OpenUIParam)o).UseMask;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_UseMask_0(ref object o, object v)
        {
            ((TinaX.UIKit.OpenUIParam)o).UseMask = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_UseMask_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @UseMask = ptr_of_this_method->Value == 1;
            ((TinaX.UIKit.OpenUIParam)o).UseMask = @UseMask;
            return ptr_of_this_method;
        }

        static object get_CloseByMask_1(ref object o)
        {
            return ((TinaX.UIKit.OpenUIParam)o).CloseByMask;
        }

        static StackObject* CopyToStack_CloseByMask_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((TinaX.UIKit.OpenUIParam)o).CloseByMask;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_CloseByMask_1(ref object o, object v)
        {
            ((TinaX.UIKit.OpenUIParam)o).CloseByMask = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_CloseByMask_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @CloseByMask = ptr_of_this_method->Value == 1;
            ((TinaX.UIKit.OpenUIParam)o).CloseByMask = @CloseByMask;
            return ptr_of_this_method;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = new TinaX.UIKit.OpenUIParam();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
