using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace XQ
{
    public class ResMgr : SingleBase<ResMgr>
    {
        /// <summary>
        /// 加载预设体
        /// </summary>
        /// <param name="prefabsName"></param>
        /// <returns></returns>
        public GameObject LoadPrefab(string prefabsName)
        {
            GameObject go = Resources.Load("Prefabs/" + prefabsName) as GameObject;
            return go;
        }

        /// <summary>
        /// 加载图片精灵
        /// </summary>
        /// <param name="sprName"></param>
        /// <returns></returns>
        public Sprite LoadImg(Sprite sprName)
        {
            Sprite spr = Resources.Load<Sprite>("Image/" + sprName);
            return spr;
        }

        /// <summary>
        /// 加载视频片段
        /// </summary>
        /// <param name="clipName"></param>
        /// <returns></returns>
        public VideoClip LoadVideoClip(string clipName)
        {
            VideoClip vc = Resources.Load<VideoClip>("Videos/" + clipName);
            return vc;
        }
        /// <summary>
        /// 加载音频片段
        /// </summary>
        /// <param name="clipName"></param>
        /// <returns></returns>
        public AudioClip LoadAudioClip(string clipName)
        {
            AudioClip clip = Resources.Load<AudioClip>("AudioClips/" + clipName);
            return clip;
        }
    }
}
