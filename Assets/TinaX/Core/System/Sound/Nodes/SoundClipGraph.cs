using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace TinaX.BluePrint.Sound
{
    [CreateAssetMenu(menuName = "TinaX/音频/智能切片",fileName = "XSoundClip")]
    public class SmartClip : NodeGraph
    {
        

        private void OnEnable()
        {
            name = "音频智能对象";
        }
    }

}
