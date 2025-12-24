using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

namespace XQ
{
    /// <summary>
    /// 音频管理器 - 单例模式
    /// 支持从Resources和StreamingAssets加载音频
    /// 提供对象池优化性能
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;
        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("AudioManager");
                    _instance = go.AddComponent<AudioManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        [Header("音频设置")]
        [SerializeField] private int audioSourcePoolSize = 5; // 对象池大小
        [SerializeField] private float masterVolume = 1f; // 主音量
        [SerializeField] private float bgmVolume = 1f; // 背景音乐音量
        [SerializeField] private float sfxVolume = 1f; // 音效音量

        // BGM专用AudioSource
        private AudioSource bgmSource;

        // 音效AudioSource对象池
        private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
        private List<AudioSource> activeAudioSources = new List<AudioSource>();

        // 音频缓存
        private Dictionary<string, AudioClip> audioClipCache = new Dictionary<string, AudioClip>();

        // 当前播放的音频信息
        private Dictionary<string, AudioSource> namedAudioSources = new Dictionary<string, AudioSource>();

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAudioSources();
        }

        /// <summary>
        /// 初始化音频源对象池
        /// </summary>
        private void InitializeAudioSources()
        {
            // 创建BGM专用AudioSource
            GameObject bgmObject = new GameObject("BGM_AudioSource");
            bgmObject.transform.SetParent(transform);
            bgmSource = bgmObject.AddComponent<AudioSource>();
            bgmSource.playOnAwake = false;
            bgmSource.loop = true;

            // 创建音效对象池
            for (int i = 0; i < audioSourcePoolSize; i++)
            {
                CreateAudioSource();
            }
        }

        /// <summary>
        /// 创建新的AudioSource
        /// </summary>
        private AudioSource CreateAudioSource()
        {
            GameObject audioObject = new GameObject($"AudioSource_{audioSourcePool.Count}");
            audioObject.transform.SetParent(transform);
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSourcePool.Enqueue(audioSource);
            return audioSource;
        }

        /// <summary>
        /// 从对象池获取AudioSource
        /// </summary>
        private AudioSource GetAudioSource()
        {
            if (audioSourcePool.Count == 0)
            {
                return CreateAudioSource();
            }

            AudioSource source = audioSourcePool.Dequeue();
            activeAudioSources.Add(source);
            return source;
        }

        /// <summary>
        /// 回收AudioSource到对象池
        /// </summary>
        private void ReturnAudioSource(AudioSource source)
        {
            if (source == null) return;

            source.Stop();
            source.clip = null;
            source.loop = false;
            activeAudioSources.Remove(source);

            if (!audioSourcePool.Contains(source))
            {
                audioSourcePool.Enqueue(source);
            }
        }

        #region 播放音频

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="audioName">音频名称(不含扩展名)</param>
        /// <param name="fromStreamingAssets">是否从StreamingAssets加载</param>
        /// <param name="loop">是否循环</param>
        /// <param name="fadeInDuration">淡入时长</param>
        public void PlayBGM(string audioName, bool fromStreamingAssets = false, bool loop = true, float fadeInDuration = 0f)
        {
            StartCoroutine(PlayBGMCoroutine(audioName, fromStreamingAssets, loop, fadeInDuration));
        }

        private IEnumerator PlayBGMCoroutine(string audioName, bool fromStreamingAssets, bool loop, float fadeInDuration)
        {
            AudioClip clip = null;

            if (fromStreamingAssets)
            {
                yield return StartCoroutine(LoadAudioFromStreamingAssets(audioName, (loadedClip) => clip = loadedClip));
            }
            else
            {
                clip = LoadAudioFromResources(audioName);
            }

            if (clip == null)
            {
                Debug.LogError($"无法加载音频: {audioName}");
                yield break;
            }

            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.volume = fadeInDuration > 0 ? 0 : bgmVolume * masterVolume;
            bgmSource.Play();

            if (fadeInDuration > 0)
            {
                yield return StartCoroutine(FadeVolume(bgmSource, bgmVolume * masterVolume, fadeInDuration));
            }
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="audioName">音频名称</param>
        /// <param name="fromStreamingAssets">是否从StreamingAssets加载</param>
        /// <param name="volume">音量(0-1)</param>
        /// <param name="loop">是否循环</param>
        /// <param name="trackName">追踪名称，用于后续控制</param>
        public void PlaySFX(string audioName, bool fromStreamingAssets = false, float volume = 1f, bool loop = false, string trackName = null)
        {
            StartCoroutine(PlaySFXCoroutine(audioName, fromStreamingAssets, volume, loop, trackName));
        }

        private IEnumerator PlaySFXCoroutine(string audioName, bool fromStreamingAssets, float volume, bool loop, string trackName)
        {
            AudioClip clip = null;

            if (fromStreamingAssets)
            {
                yield return StartCoroutine(LoadAudioFromStreamingAssets(audioName, (loadedClip) => clip = loadedClip));
            }
            else
            {
                clip = LoadAudioFromResources(audioName);
            }

            if (clip == null)
            {
                Debug.LogError($"无法加载音频: {audioName}");
                yield break;
            }

            AudioSource source = GetAudioSource();
            source.clip = clip;
            source.volume = volume * sfxVolume * masterVolume;
            source.loop = loop;
            source.Play();

            // 如果有追踪名称，保存引用
            if (!string.IsNullOrEmpty(trackName))
            {
                if (namedAudioSources.ContainsKey(trackName))
                {
                    ReturnAudioSource(namedAudioSources[trackName]);
                }
                namedAudioSources[trackName] = source;
            }

            // 如果不循环，播放完毕后自动回收
            if (!loop)
            {
                yield return new WaitForSeconds(clip.length);
                ReturnAudioSource(source);
                if (!string.IsNullOrEmpty(trackName))
                {
                    namedAudioSources.Remove(trackName);
                }
            }
        }

        /// <summary>
        /// 播放一次性音效(简化版)
        /// </summary>
        public void PlayOneShot(string audioName, float volume = 1f, bool fromStreamingAssets = false)
        {
            PlaySFX(audioName, fromStreamingAssets, volume, false);
        }

        #endregion

        #region 音频加载

        /// <summary>
        /// 从Resources加载音频
        /// </summary>
        private AudioClip LoadAudioFromResources(string audioName)
        {
            // 先检查缓存
            if (audioClipCache.ContainsKey(audioName))
            {
                return audioClipCache[audioName];
            }

            // 尝试加载
            AudioClip clip = Resources.Load<AudioClip>(audioName);
            if (clip != null)
            {
                audioClipCache[audioName] = clip;
            }
            return clip;
        }

        /// <summary>
        /// 从StreamingAssets异步加载音频
        /// </summary>
        private IEnumerator LoadAudioFromStreamingAssets(string audioName, Action<AudioClip> callback)
        {
            // 先检查缓存
            if (audioClipCache.ContainsKey(audioName))
            {
                callback?.Invoke(audioClipCache[audioName]);
                yield break;
            }

            string path = System.IO.Path.Combine(Application.streamingAssetsPath, audioName);

            // 根据平台处理路径
#if UNITY_ANDROID && !UNITY_EDITOR
            path = System.IO.Path.Combine(Application.streamingAssetsPath, audioName);
#endif

            AudioType audioType = GetAudioType(audioName);

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, audioType))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    if (clip != null)
                    {
                        audioClipCache[audioName] = clip;
                        callback?.Invoke(clip);
                    }
                    else
                    {
                        Debug.LogError($"无法获取AudioClip: {audioName}");
                        callback?.Invoke(null);
                    }
                }
                else
                {
                    Debug.LogError($"加载音频失败: {path}, 错误: {www.error}");
                    callback?.Invoke(null);
                }
            }
        }

        /// <summary>
        /// 根据文件扩展名获取音频类型
        /// </summary>
        private AudioType GetAudioType(string fileName)
        {
            string extension = System.IO.Path.GetExtension(fileName).ToLower();
            switch (extension)
            {
                case ".mp3": return AudioType.MPEG;
                case ".wav": return AudioType.WAV;
                case ".ogg": return AudioType.OGGVORBIS;
                default: return AudioType.UNKNOWN;
            }
        }

        #endregion

        #region 音频控制

        /// <summary>
        /// 暂停BGM
        /// </summary>
        public void PauseBGM()
        {
            if (bgmSource != null && bgmSource.isPlaying)
            {
                bgmSource.Pause();
            }
        }

        /// <summary>
        /// 恢复BGM
        /// </summary>
        public void ResumeBGM()
        {
            if (bgmSource != null && !bgmSource.isPlaying)
            {
                bgmSource.UnPause();
            }
        }

        /// <summary>
        /// 停止BGM
        /// </summary>
        /// <param name="fadeOutDuration">淡出时长</param>
        public void StopBGM(float fadeOutDuration = 0f)
        {
            if (fadeOutDuration > 0)
            {
                StartCoroutine(StopBGMWithFade(fadeOutDuration));
            }
            else
            {
                bgmSource.Stop();
            }
        }

        private IEnumerator StopBGMWithFade(float duration)
        {
            yield return StartCoroutine(FadeVolume(bgmSource, 0, duration));
            bgmSource.Stop();
            bgmSource.volume = bgmVolume * masterVolume;
        }

        /// <summary>
        /// 暂停指定音效
        /// </summary>
        public void PauseSFX(string trackName)
        {
            if (namedAudioSources.ContainsKey(trackName))
            {
                namedAudioSources[trackName].Pause();
            }
        }

        /// <summary>
        /// 恢复指定音效
        /// </summary>
        public void ResumeSFX(string trackName)
        {
            if (namedAudioSources.ContainsKey(trackName))
            {
                namedAudioSources[trackName].UnPause();
            }
        }

        /// <summary>
        /// 停止指定音效
        /// </summary>
        public void StopSFX(string trackName)
        {
            if (namedAudioSources.ContainsKey(trackName))
            {
                ReturnAudioSource(namedAudioSources[trackName]);
                namedAudioSources.Remove(trackName);
            }
        }

        /// <summary>
        /// 停止所有音效
        /// </summary>
        public void StopAllSFX()
        {
            foreach (var source in activeAudioSources.ToArray())
            {
                ReturnAudioSource(source);
            }
            namedAudioSources.Clear();
        }

        /// <summary>
        /// 暂停所有音频
        /// </summary>
        public void PauseAll()
        {
            PauseBGM();
            foreach (var source in activeAudioSources)
            {
                source.Pause();
            }
        }

        /// <summary>
        /// 恢复所有音频
        /// </summary>
        public void ResumeAll()
        {
            ResumeBGM();
            foreach (var source in activeAudioSources)
            {
                source.UnPause();
            }
        }

        #endregion

        #region 音量控制

        /// <summary>
        /// 设置主音量
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            bgmSource.volume = bgmVolume * masterVolume;
            foreach (var source in activeAudioSources)
            {
                source.volume = sfxVolume * masterVolume;
            }
        }

        /// <summary>
        /// 设置BGM音量
        /// </summary>
        public void SetBGMVolume(float volume)
        {
            bgmVolume = Mathf.Clamp01(volume);
            bgmSource.volume = bgmVolume * masterVolume;
        }

        /// <summary>
        /// 设置音效音量
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            foreach (var source in activeAudioSources)
            {
                source.volume = sfxVolume * masterVolume;
            }
        }

        /// <summary>
        /// 设置指定音效音量
        /// </summary>
        public void SetTrackVolume(string trackName, float volume)
        {
            if (namedAudioSources.ContainsKey(trackName))
            {
                namedAudioSources[trackName].volume = Mathf.Clamp01(volume) * sfxVolume * masterVolume;
            }
        }

        /// <summary>
        /// 音量淡入淡出
        /// </summary>
        private IEnumerator FadeVolume(AudioSource source, float targetVolume, float duration)
        {
            float startVolume = source.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
                yield return null;
            }

            source.volume = targetVolume;
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 清理缓存
        /// </summary>
        public void ClearCache()
        {
            audioClipCache.Clear();
        }

        /// <summary>
        /// 是否正在播放BGM
        /// </summary>
        public bool IsBGMPlaying()
        {
            return bgmSource != null && bgmSource.isPlaying;
        }

        /// <summary>
        /// 获取当前BGM名称
        /// </summary>
        public string GetCurrentBGMName()
        {
            return bgmSource.clip != null ? bgmSource.clip.name : "";
        }

        #endregion

        private void OnDestroy()
        {
            StopAllCoroutines();
            ClearCache();
        }
    }
}
