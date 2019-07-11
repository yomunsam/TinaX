using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinaX.Sound
{
    public class XSoundMgr:IXSound
    {
        public XSoundMgr()
        {
            var TinaXRoot = GameObjectHelper.FindOrCreateGo(Setup.Framework_Base_GameObject);

            mXSoundRootGameObject = TinaXRoot.FindOrCreateGo(XSoundConst.Sound_Base_GameObject);
        }

        /// <summary>
        /// XSound根GameObject
        /// </summary>
        private GameObject mXSoundRootGameObject;

        /// <summary>
        /// 音轨池
        /// </summary>
        private Dictionary<string, SoundTrack> mDict_SoundTrack = new Dictionary<string, SoundTrack>();


        /// <summary>
        /// 创建音轨
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ISoundTrack CreateSoundTrack(string name)
        {
            if (mDict_SoundTrack.ContainsKey(name))
            {
                return mDict_SoundTrack[name];
            }
            else
            {
                var st = new SoundTrack(name,this);
                mDict_SoundTrack.Add(name, st);
                return st;
            }
        }

        /// <summary>
        /// 音轨是否已存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsSoundTrackExist(string name)
        {
            return mDict_SoundTrack.ContainsKey(name);
        }
        
        public ISoundTrack GetSoundTrack(string name)
        {
            if (!mDict_SoundTrack.ContainsKey(name))
            {
                return null;
            }
            return mDict_SoundTrack[name];
        }



        /// <summary>
        /// 获取到它的全局GameObject
        /// </summary>
        /// <returns></returns>
        public GameObject GetRootGameObject()
        {
            return mXSoundRootGameObject;
        }
    }
}


