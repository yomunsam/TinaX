namespace TinaX
{
    /// <summary>
    /// TinaX事件定义
    /// </summary>
    public static class EventDefine
    {
        /// <summary>
        /// 场景切换,传递两个参数，当前场景和下一个场景
        /// </summary>
        public const string X_OnSceneChanged = "TinaX_OnActiveSceneChanged";

        /// <summary>
        /// I18N语言变更，传递一个参数为当前region名
        /// </summary>
        public const string X_OnI18NRegionChanged = "TinaX_OnI18NRegionChanged";
    }
}