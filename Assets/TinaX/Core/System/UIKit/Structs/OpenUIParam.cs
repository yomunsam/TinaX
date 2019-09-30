namespace TinaX.UIKits
{
    public struct OpenUIParam
    {
        public string UIName { get; set; }
        public string UIPath { get; set; }
        public object Param { get; set; }
        public bool UseMask { get; set; }
        public bool CloseByMask { get; set; }
    }
}
