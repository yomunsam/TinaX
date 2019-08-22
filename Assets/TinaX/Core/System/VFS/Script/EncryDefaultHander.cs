namespace TinaX.VFSKit
{
    /// <summary>
    /// 默认加密方法
    /// </summary>
    public static class EncryDefaultHander
    {
        /// <summary>
        /// 获取Offset方式加密的offset 值
        /// </summary>
        /// <param name="assetBundleHashCode"></param>
        /// <param name="assetBundlePath">带后缀的</param>
        /// <returns></returns>
        public static ulong GetOffsetValue(UnityEngine.Hash128 assetBundleHashCode, string assetBundlePath)//瞎几把写的,效果是不同的hashCode获取到的offset值不一样
        {
            int emm = 5;
            string hashStr = assetBundleHashCode.ToString();
            if(hashStr.Length > 2)
            {
                emm = (int)hashStr[0] + (int)hashStr[1];
            }
            return (ulong)emm; 
        }

        /// <summary>
        /// 对文件做偏移
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static byte[] OffsetData(byte[] source, ulong offset)
        {
            byte[] target = new byte[source.Length + (int)offset];

            byte[] meow = System.Text.Encoding.UTF8.GetBytes(StringHelper.GenRandomStr((int)offset));
            meow.CopyTo(target, 0);
            source.CopyTo(target, meow.Length);

            return target;
        }
        
    }

}
