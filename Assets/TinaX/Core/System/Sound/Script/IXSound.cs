using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.Sound
{
    /// <summary>
    /// 音频管理器接口
    /// </summary>
    public interface IXSound
    {
        /// <summary>
        /// 创建音轨，如已存在则返回已存在音轨
        /// </summary>
        /// <param name="name">音轨名</param>
        /// <returns>音轨对象</returns>
        ISoundTrack CreateSoundTrack(string name);


        /// <summary>
        /// 音轨是否已存在
        /// </summary>
        /// <param name="name">音轨名</param>
        /// <returns></returns>
        bool IsSoundTrackExist(string name);

        /// <summary>
        /// 获取音轨
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ISoundTrack GetSoundTrack(string name);
    }
}
