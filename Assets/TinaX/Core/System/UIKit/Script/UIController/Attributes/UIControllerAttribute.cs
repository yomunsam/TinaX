using System;

namespace TinaX.UIKits
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class UIControllerAttribute : Attribute
    {
        public string UIName { get; set; }
        public string UIPath { get; set; }

        #region Updates

        /// <summary>
        /// Enable Update (every frame) | 是否启用Update方法（每帧触发）
        /// </summary>
        public bool EnableUpdate { get; set; } = false;
        public int UpdateOrder { get; set; } = 1;

        public bool EnableFixedUpdate { get; set; } = false;
        public int FixedUpdateOrder { get; set; } = 1;

        public bool EnableLateUpdate { get; set; } = false;
        public int LateUpdateOrder { get; set; } = 1;

        public bool PauseUpdatesWhenDisable { get; set; } = true;

        #endregion


        public UIControllerAttribute(string uiName)
        {
            UIName = uiName;
        }

        public UIControllerAttribute()
        {

        }

    }
}
