using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace TinaX.BluePrint.Sound
{


    #region Clips


    /// <summary>
    /// 随机
    /// </summary>
    [CreateNodeMenu("Tina X/Sound/Clip/随机")]
    public class RandomClip : Node
    {
        [Header("输入Clips")]
        [Input] public AudioClip[] clips;

        [Output] public AudioClip result;

        protected override void Init()
        {
            base.Init();
            name = "Random";
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "result")
            {
                var _clips = GetInputValue<AudioClip[]>("clips", this.clips);
                if (_clips.Length <= 0) { return null; }
                return _clips[UnityEngine.Random.Range(0, _clips.Length - 1)];
            }
            return null;
        }
    }

    /// <summary>
    /// 音频切片列表
    /// </summary>
    [CreateNodeMenu("Tina X/Sound/Clip/List")]
    public class ClipsList : Node
    {
        [Input] public AudioClip[] clips;
        [Input] public int outIndex=0;

        [Output] public AudioClip result;
        [Output] public int maxIndex;

        protected override void Init()
        {
            base.Init();
            name = "Clip List";
        }

        public override object GetValue(NodePort port)
        {
            var _clips = GetInputValue<AudioClip[]>("clips", this.clips);
            if (port.fieldName == "result")
            {
                
                var _index = GetInputValue<int>("outIndex", this.outIndex);
                if (_clips.Length <= 0) { return null; }
                if(_clips.Length < _index)
                {
                    return _clips[_clips.Length - 1];
                }
                else
                {
                    return _clips[_index];
                }

            }

            if (port.fieldName == "maxIndex")
            {

                return _clips.Length - 1;

            }
            return null;
        }
    }

    #endregion


    #region Math
    /// <summary>
    /// 音频切片列表
    /// </summary>
    [CreateNodeMenu("Tina X/Sound/Math/Random (int)")]
    public class GetRandomInt : Node
    {
        [Header("输入Clips")]
        [Input] public int start;
        [Input] public int end;

        [Output] public int value;

        protected override void Init()
        {
            base.Init();
            name = "Int Random";
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "value")
            {
                return UnityEngine.Random.Range(start, end);
            }
            return null;
        }
    }



    #endregion




}
