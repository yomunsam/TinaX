using System;
using System.Collections.Generic;
using System.Reflection;
using TinaXEditor.XILRuntime;
using UnityEngine;

namespace Nekonya
{
    public class ILRuntimeGenCodeDefine : ICLRGenerateDefine
    {
        public void GenerateByAssemblies_InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            
        }

        public HashSet<FieldInfo> GetCLRBindingExcludeFields() => null;

        public HashSet<MethodBase> GetCLRBindingExcludeMethods() => null;

        public List<Type> GetCLRBindingTypes() => new List<Type>
        {
            // typeof(GameObject), ......
        };

        public List<Type> GetDelegateTypes() => new List<Type> { };

        public List<Type> GetValueTypeBinders() => new List<Type>
        {
            typeof(Vector3Int),
        };
    }
}

