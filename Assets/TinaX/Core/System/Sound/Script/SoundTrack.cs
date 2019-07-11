using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TinaX;
using UniRx;

namespace TinaX.Sound
{
    /// <summary>
    /// 音轨
    /// </summary>
    public class SoundTrack : ISoundTrack
    {
        public SoundTrack(string name, XSoundMgr mgr)
        {
            mSoundTrackName = name;
            mBaseMgr = mgr;
            TrackGameObject = mgr.GetRootGameObject().FindOrCreateGo("sTrack_" + name);
            MainAudioSource = TrackGameObject.GetComponentOrAdd<AudioSource>();
            MainAudioSource.playOnAwake = false;
        }


        private AudioSource MainAudioSource;
        private GameObject TrackGameObject;
        private string mSoundTrackName;
        private XSoundMgr mBaseMgr;

        /// <summary>
        /// 当前音轨播放模式
        /// </summary>
        private E_PlayMode mPlayMode = E_PlayMode.single;
        /// <summary>
        /// 当前音轨切换模式
        /// </summary>
        private E_SwitchMode mSwitchMode = E_SwitchMode.wait;

        /// <summary>
        /// 当前播放
        /// </summary>
        private SmartAudioClip mCur_PlayingClip;

        private IDisposable mCurPlayerTimer;

        /// <summary>
        /// 存储该列表中所有切片的对象池
        /// </summary>
        private Dictionary<string, SmartAudioClip> mClip_Pool = new Dictionary<string, SmartAudioClip>();

        /// <summary>
        /// 播放列表
        /// </summary>
        private List<SmartAudioClip> mPlayList = new List<SmartAudioClip>();

        /// <summary>
        /// 缓存播放列表
        /// </summary>
        private List<SmartAudioClip> mCachePlayList = new List<SmartAudioClip>(); //实际播放顺序是读的这个队列

        /// <summary>
        /// 当某个切片播放结束的回调，参数1为播放结束的切片名，参数2为即将播放的切片名，可能为空字符串（（单切片循环播放时不会触发））
        /// </summary>
        private Dictionary<int, Action<string, string>> OnClipPlayEnd = new Dictionary<int, Action<string, string>>();  //字典，为每个回调注册分配一个ID，字典为ID对Action的对应关系

        /// <summary>
        /// 当队列播放结束（单曲循环、单曲播放、队列循环时都不会相应这个Action）
        /// </summary>
        private Dictionary<int, Action> OnListPlayEnd = new Dictionary<int, Action>(); //同上，ID和Action的key/value的存储

        #region 事件注册相关API

        /// <summary>
        /// 是否是“某个切片播放结束的回调”的队列中的可用的空闲ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool IsFreeID_ForClipPlayEnd(int id)
        {
            return OnClipPlayEnd.ContainsKey(id);
        }

        /// <summary>
        /// 获取“某个切片播放结束的回调”的队列中一个可用的空闲ID
        /// </summary>
        /// <returns></returns>
        private int GetFreeID_ForClipPlayEnd()
        {
            int id = UnityEngine.Random.Range(99, 999);
            while (!IsFreeID_ForClipPlayEnd(id))
            {
                id++;
            }
            return id;
        }

        /// <summary>
        /// 是否是“队列播放完成的回调”的队列中的可用的空闲ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool IsFreeID_ForListPlayEnd(int id)
        {
            return OnListPlayEnd.ContainsKey(id);
        }

        /// <summary>
        /// 获取“队列播放完成的回调”的队列中一个可用的空闲ID
        /// </summary>
        /// <returns></returns>
        private int GetFreeID_ForListPlayEnd()
        {
            int id = UnityEngine.Random.Range(99, 999);
            while (!IsFreeID_ForListPlayEnd(id))
            {
                id++;
            }
            return id;
        }

        //注册事件：当某个音频切片播放结束（单切片循环播放时不会触发）
        public int Register_OnClipPlayEnd(Action<string, string> callback)
        {
            var id = GetFreeID_ForClipPlayEnd();
            OnClipPlayEnd.Add(id, callback);
            return id;
        }

        //注销事件：当某个音频切片播放结束
        public void Remove_OnClipPlayEnd(int id)
        {
            if (OnClipPlayEnd.ContainsKey(id))
            {
                OnClipPlayEnd.Remove(id);
            }
        }

        //注册事件：当播放列表播放结束
        public int Register_OnListPlayEnd(Action callback)
        {
            var id = GetFreeID_ForListPlayEnd();
            OnListPlayEnd.Add(id, callback);
            return id;
        }

        public void Remove_OnListPlayEnd(int id)
        {
            if (OnListPlayEnd.ContainsKey(id))
            {
                OnListPlayEnd.Remove(id);
            }
        }

        #endregion


        #region 切片管理API

        /// <summary>
        /// 添加音频片段
        /// </summary>
        public ISoundTrack AddSoundClip(UnityEngine.AudioClip clip, string name)
        {
            if(clip == null)
            {
                XLog.PrintW("[TinaX] 音轨 " + mSoundTrackName + " 添加音频片段失败：传入的音频片段为空");
                return this;
            }
            if (!mClip_Pool.ContainsKey(name))
            {
                mClip_Pool.Add(name, new SmartAudioClip() {
                    Name = name,
                    OriginClip = clip,
                });
            }

            return this;
        }


        public ISoundTrack RemoveSoundClip(string name)
        {
            if (mClip_Pool.ContainsKey(name))
            {
                mClip_Pool.Remove(name);
            }
            return this;
        }

        /// <summary>
        /// 音频片段是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsSoundClipExist(string name)
        {
            return mClip_Pool.ContainsKey(name);
        }

        public ISoundTrack SetPlayModeById(int modeId)
        {
            switch (modeId)
            {
                default:
                    return this;
                case 1:
                    return SetPlayMode(E_PlayMode.single);
                    
                case 2:
                    return SetPlayMode(E_PlayMode.single_loop);
                    
                case 3:
                    return SetPlayMode(E_PlayMode.list);
                    
                case 4:
                    return SetPlayMode(E_PlayMode.list_loop);
            }
        }

        public ISoundTrack SetPlayMode(E_PlayMode playMode)
        {
            mPlayMode = playMode;
            return this;
        }

        /// <summary>
        /// 添加到播放队列
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ISoundTrack AddToPlayList(string name)
        {
            if (mClip_Pool.ContainsKey(name))
            {
                mPlayList.Add(mClip_Pool[name]);
            }
            return this;
        }

        #endregion

        #region 播放操作

        /// <summary>
        /// 播放音轨
        /// </summary>
        /// <returns></returns>
        public ISoundTrack Play()
        {
            //编排CachePlayList
            mCachePlayList.Clear();
            if(mPlayList.Count >= 1)
            {
                mCachePlayList.Add(mPlayList[0]);   //默认情况下，只在Cache中存放即将播放的内容
            }
            

            //交给私有方法播放
            DoPlay();

            return this;
        }

        /// <summary>
        /// 播放指定音频片段
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ISoundTrack Play(string name)
        {
            //检查name是否存在
            if (!mClip_Pool.ContainsKey(name))
            {
                XLog.PrintW("[TinaX] 音轨 :" + mSoundTrackName + " ，播放指定音频片段 " + name + " 不存在");
                return this;
            }
            //当前是否有正在播放
            if (mCur_PlayingClip != null)
            {
                //有正在播放内容，看看切换策略吧
                if(mSwitchMode == E_SwitchMode.cover)
                {
                    //结束当前播放并开始下一个
                    //Todo 切换特效（淡入淡出 等，以后再写）

                    //修改Cache队列
                    mCachePlayList.Clear();
                    mCachePlayList.Add(mClip_Pool[name]);
                    DoPlay();


                }else if(mSwitchMode == E_SwitchMode.wait)
                {
                    //等待当前播放结束并播放下一个
                    //编排进Cache队列之后不管了
                    mCachePlayList.Add(mClip_Pool[name]);
                }
            }
            else
            {
                //直接编排队列
                mCachePlayList.Clear();
                mCachePlayList.Add(mClip_Pool[name]);
                DoPlay();
            }
            return this;
        }


        #endregion


        #region Private_音轨操作

        /// <summary>
        /// 内部播放方法，编排好CachePlayList之后调用这里
        /// </summary>
        private void DoPlay()
        {
            //当前是否有播放
            if (mCur_PlayingClip != null)
            {
                MainAudioSource.Stop();
                mCur_PlayingClip = null;
                if(mCurPlayerTimer != null)
                {
                    mCurPlayerTimer.Dispose();
                    mCurPlayerTimer = null;
                }
            }
            if(mCachePlayList.Count <= 0)
            {
                XLog.PrintW("[TinaX] 音轨 " + mSoundTrackName + " 播放失败：音频缓存队列为空");
                return;
            }
            mCur_PlayingClip = mCachePlayList[0];
            MainAudioSource.clip = mCur_PlayingClip.OriginClip;
            var length = mCur_PlayingClip.OriginClip.length;
            //设置计时器
            mCurPlayerTimer = Observable.Timer(TimeSpan.FromSeconds(length)).Subscribe(_=> {
                Timer_OnClipPlayEnd();
            });

            MainAudioSource.Play();
            mCachePlayList.RemoveAt(0);
        }

        //某个Clip播放结束的计时器
        private void Timer_OnClipPlayEnd()
        {
            Debug.Log("播放器：播放Clip计时器触发");
            //某个clip播放结束了，我们看看接下来要干嘛
            //首先看看播放模式
            if (mPlayMode == E_PlayMode.single)
            {
                //那就播放结束了呗，处理下回调
                foreach(var item in OnClipPlayEnd)
                {
                    item.Value(mCur_PlayingClip.Name,"");
                }
                //然后，清空当前状态
                mCurPlayerTimer = null;
                mCur_PlayingClip = null;
            }
            if (mPlayMode == E_PlayMode.single_loop)
            {
                //循环啊，那就再放一遍
                mCachePlayList.Add(mCur_PlayingClip);
                DoPlay();
            }
            if(mPlayMode == E_PlayMode.list || mPlayMode == E_PlayMode.list_loop)
            {
                //列表循环，播放下一个,编排Cache
                int index = 0;
                for(int i = 0; i < mPlayList.Count; i++)
                {
                    if(mPlayList[i].Name == mCur_PlayingClip.Name)
                    {
                        index = i;
                        break;
                    }
                }
                index++;
                if(index >= mPlayList.Count)
                {
                    index = 0;
                    if(mPlayMode == E_PlayMode.list)
                    {
                        foreach(var item in OnListPlayEnd)
                        {
                            item.Value();
                        }
                    }
                }
                mCachePlayList.Add(mPlayList[index]);
                DoPlay();

            }


            mCurPlayerTimer = null;
        }



        #endregion



        /// <summary>
        /// 播放模式
        /// </summary>
        public enum E_PlayMode
        {
            /// <summary>
            /// 播放单个片段
            /// </summary>
            single,
            /// <summary>
            /// 循环播放单个片段
            /// </summary>
            single_loop,
            /// <summary>
            /// 播放当前列表
            /// </summary>
            list,
            /// <summary>
            /// 循环播放当前列表
            /// </summary>
            list_loop,
        }

        /// <summary>
        /// 音轨中切换音频切片的方式
        /// </summary>
        public enum E_SwitchMode
        {
            /// <summary>
            /// 等待当前音频切片播放完毕后播放下一个
            /// </summary>
            wait,
            /// <summary>
            /// 如果当前正在播放音频切片，直接结束并播放待切换的切片
            /// </summary>
            cover,
        }

    }
}
