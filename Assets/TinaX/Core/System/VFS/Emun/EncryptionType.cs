using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TinaX.VFSKit
{
    [System.Serializable]
    public enum EncryptionType
    {
        None = 0,
        
        /// <summary>
        /// 偏移
        /// </summary>
        [Header("偏移")]
        Offset = 1,
        /// <summary>
        /// RSA 非对称加密
        /// </summary>
        [Header("RSA算法 非对称加密")]
        RSA = 2,
        /// <summary>
        /// DES
        /// </summary>
        [Header("DES")]
        DES = 3,
        /// <summary>
        /// AES128 ECB
        /// </summary>
        [Header("AES128 ECB")]
        AES128_ECB = 4,
        /// <summary>
        /// AES 256 ECB
        /// </summary>
        [Header("AES256 ECB")]
        AES256_ECB = 5,

    }

    /// <summary>
    /// 偏移加密方式 -> 偏移量计算
    /// </summary>
    [System.Serializable]
    public enum EncryOffsetType
    {
        [Header("默认方式")]
        Default = 0,    //默认方式采用AssetBundle的HashCode的特征值来计算
        [Header("固定值")]
        FixedValue = 1, //在设置中给定为某个值
        /// <summary>
        /// 填写一个自定义C#方法的路径，系统将尝试对其反射并作为方法使用。
        /// </summary>
        [Header("自定义方法")]
        CustomFunction = 2,
    }

}

