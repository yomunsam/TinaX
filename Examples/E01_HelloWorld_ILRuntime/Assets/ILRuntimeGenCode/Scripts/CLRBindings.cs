using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {

        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector3> s_UnityEngine_Vector3_Binding_Binder = null;
        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector2> s_UnityEngine_Vector2_Binding_Binder = null;
        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Quaternion> s_UnityEngine_Quaternion_Binding_Binder = null;
        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Color> s_UnityEngine_Color_Binding_Binder = null;

        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            UnityEngine_Debug_Binding.Register(app);
            TinaX_XCore_Binding.Register(app);
            TinaX_IXCore_Binding.Register(app);
            System_Array_Binding.Register(app);
            TinaX_Container_IServiceContainer_Binding.Register(app);
            TinaX_UIKit_IUIKit_Binding.Register(app);
            TinaX_VFSKit_IVFS_Binding.Register(app);
            System_Exception_Binding.Register(app);
            System_String_Binding.Register(app);
            TinaX_VFSKit_ISceneAsset_Binding.Register(app);
            TinaX_UIKit_DataBinding_BindableProperty_1_Int32_Binding.Register(app);
            UnityEngine_UI_Button_Binding.Register(app);
            UnityEngine_Events_UnityEvent_Binding.Register(app);
            TinaX_I18N_II18N_Binding.Register(app);
            UnityEngine_UI_Text_Binding.Register(app);
            System_Runtime_CompilerServices_AsyncVoidMethodBuilder_Binding.Register(app);
            System_Threading_Tasks_Task_1_IAsset_Binding.Register(app);
            System_Runtime_CompilerServices_TaskAwaiter_1_IAsset_Binding.Register(app);
            TinaX_VFSKit_IAsset_Binding.Register(app);
            UnityEngine_TextAsset_Binding.Register(app);
            System_IDisposable_Binding.Register(app);
            TinaX_UIKit_OpenUIParam_Binding.Register(app);
            System_Object_Binding.Register(app);
            System_Threading_Tasks_Task_1_IUIEntity_Binding.Register(app);
            System_Runtime_CompilerServices_TaskAwaiter_1_IUIEntity_Binding.Register(app);

            ILRuntime.CLR.TypeSystem.CLRType __clrType = null;
            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Vector3));
            s_UnityEngine_Vector3_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector3>;
            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Vector2));
            s_UnityEngine_Vector2_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector2>;
            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Quaternion));
            s_UnityEngine_Quaternion_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Quaternion>;
            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Color));
            s_UnityEngine_Color_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Color>;
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            s_UnityEngine_Vector3_Binding_Binder = null;
            s_UnityEngine_Vector2_Binding_Binder = null;
            s_UnityEngine_Quaternion_Binding_Binder = null;
            s_UnityEngine_Color_Binding_Binder = null;
        }
    }
}
