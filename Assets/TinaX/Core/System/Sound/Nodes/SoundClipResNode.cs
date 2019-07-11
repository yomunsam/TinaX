using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace TinaX.BluePrint.Sound
{
    /// <summary>
    /// XSound音频切片-资源节点
    /// </summary>
    [CreateNodeMenu("Tina X/Sound/Main")]
    public class SoundClipResNode:Node
    {
        [Input] public AudioClip clip;
        [Input] public bool Loop;
        //[Input] public System.Type Effector;

        protected override void Init()
        {
            base.Init();
            name = "XSound Clip Config";
        }
    }
}

