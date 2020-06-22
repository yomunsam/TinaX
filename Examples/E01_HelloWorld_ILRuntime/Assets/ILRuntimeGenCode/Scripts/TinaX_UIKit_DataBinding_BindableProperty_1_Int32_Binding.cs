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
    unsafe class TinaX_UIKit_DataBinding_BindableProperty_1_Int32_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(TinaX.UIKit.DataBinding.BindableProperty<System.Int32>);
            args = new Type[]{typeof(TinaX.UIKit.DataBinding.BindableProperty<System.Int32>.ValueChangedDalegate)};
            method = type.GetMethod("OnValueChanged", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnValueChanged_0);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_Value", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_Value_1);
            args = new Type[]{};
            method = type.GetMethod("get_Value", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Value_2);

            args = new Type[]{typeof(System.Int32)};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }


        static StackObject* OnValueChanged_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            TinaX.UIKit.DataBinding.BindableProperty<System.Int32>.ValueChangedDalegate @callback = (TinaX.UIKit.DataBinding.BindableProperty<System.Int32>.ValueChangedDalegate)typeof(TinaX.UIKit.DataBinding.BindableProperty<System.Int32>.ValueChangedDalegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            TinaX.UIKit.DataBinding.BindableProperty<System.Int32> instance_of_this_method = (TinaX.UIKit.DataBinding.BindableProperty<System.Int32>)typeof(TinaX.UIKit.DataBinding.BindableProperty<System.Int32>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnValueChanged(@callback);

            return __ret;
        }

        static StackObject* set_Value_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            TinaX.UIKit.DataBinding.BindableProperty<System.Int32> instance_of_this_method = (TinaX.UIKit.DataBinding.BindableProperty<System.Int32>)typeof(TinaX.UIKit.DataBinding.BindableProperty<System.Int32>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Value = value;

            return __ret;
        }

        static StackObject* get_Value_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            TinaX.UIKit.DataBinding.BindableProperty<System.Int32> instance_of_this_method = (TinaX.UIKit.DataBinding.BindableProperty<System.Int32>)typeof(TinaX.UIKit.DataBinding.BindableProperty<System.Int32>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Value;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @defaultValue = ptr_of_this_method->Value;


            var result_of_this_method = new TinaX.UIKit.DataBinding.BindableProperty<System.Int32>(@defaultValue);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
