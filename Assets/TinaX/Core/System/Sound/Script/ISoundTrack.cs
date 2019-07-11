using System;
using static TinaX.Sound.SoundTrack;

namespace TinaX.Sound
{
    /// <summary>
    /// 音轨 用户接口
    /// </summary>
    public interface ISoundTrack
    {
        /// <summary>
        /// 注册事件：当某个音频切片播放结束（单切片循环播放时不会触发）
        /// </summary>
        /// <param name="callback">事件回调，接收参数1：本次播放结束的切片名，接收参数2：即将播放的切片名，可能为空</param>
        /// <returns>事件注册句柄ID</returns>
        int Register_OnClipPlayEnd(Action<string, string> callback);

        /// <summary>
        /// 取消注册事件：当某个音频切片播放结束
        /// </summary>
        /// <param name="id">注册事件时得到的ID</param>
        void Remove_OnClipPlayEnd(int id);

        /// <summary>
        /// 注册事件：当音频列表播放结束（仅在列表播放不循环的模式下有效）
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        int Register_OnListPlayEnd(Action callback);

        /// <summary>
        /// 取消注册事件：当音频列表播放结束
        /// </summary>
        /// <param name="id"></param>
        void Remove_OnListPlayEnd(int id);

        /// <summary>
        /// 添加普通音频片段
        /// </summary>
        /// <param name="clip">AudioClip对象</param>
        /// <param name="name">片段名称</param>
        /// <returns></returns>
        ISoundTrack AddSoundClip(UnityEngine.AudioClip clip, string name);

        /// <summary>
        /// 移除音频切片
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ISoundTrack RemoveSoundClip(string name);

        /// <summary>
        /// 音频片段是否存在
        /// </summary>
        /// <param name="name">音频片段名</param>
        /// <returns></returns>
        bool IsSoundClipExist(string name);

        /// <summary>
        /// 通过Index设置播放模式
        /// </summary>
        /// <param name="modeId">播放模式ID: 1.单片段播放 2.单片段循环 3.列表播放 4.列表循环</param>
        /// <returns></returns>
        ISoundTrack SetPlayModeById(int modeId);


        /// <summary>
        /// 设置播放模式
        /// </summary>
        /// <param name="playMode">播放模式</param>
        /// <returns></returns>
        ISoundTrack SetPlayMode(E_PlayMode playMode);

        /// <summary>
        /// 添加到播放队列
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ISoundTrack AddToPlayList(string name);


        /// <summary>
        /// 播放！
        /// </summary>
        /// <returns></returns>
        ISoundTrack Play();

        /// <summary>
        /// 播放指定音频片段
        /// </summary>
        /// <param name="name">ClipName</param>
        /// <returns></returns>
        ISoundTrack Play(string name);
    }
}
