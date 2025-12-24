using UnityEngine;
using RenderHeads.Media.AVProVideo;
using System;
using System.Collections;

namespace XQ
{
    /// <summary>
    /// AVPro视频播放管理类
    /// 提供完整的视频播放控制、状态监听和事件回调功能
    /// </summary>
    public class AVProVideoManager : MonoBehaviour
    {
        #region 变量
        [Header("视频播放器引用")]
        [SerializeField] private MediaPlayer mediaPlayer;

        [Header("播放设置")]
        [SerializeField] private bool autoPlay = false;
        [SerializeField] private bool loop = false;
        [SerializeField] private float defaultVolume = 1f;
        //[SerializeField] private float fadeSpeed = 1f;

        // 视频状态
        private bool isPlaying = false;
        private bool isPaused = false;
        private bool isLoading = false;
        private float currentTime = 0f;
        private float duration = 0f;
        #endregion 

        #region 事件回调
        /// <summary>
        /// 开始播放
        /// </summary>
        public event Action OnVideoStarted;

        /// <summary>
        /// 视频暂停
        /// </summary>
        public event Action OnVideoPaused;

        /// <summary>
        /// 视频恢复播放
        /// </summary>
        public event Action OnVideoResumed;

        /// <summary>
        /// 视频停止
        /// </summary>
        public event Action OnVideoStopped;

        /// <summary>
        /// 视频播放完成
        /// </summary>
        public event Action OnVideoCompleted;

        /// <summary>
        /// 视频播放错误
        /// </summary>
        public event Action<string> OnVideoError;

        /// <summary>
        /// 视频播放中
        /// </summary>
        public event Action<float> OnVideoProgress; // 返回进度 0-1

        /// <summary>
        /// 视频加载完成
        /// </summary>
        public event Action OnVideoLoaded;

        /// <summary>
        /// 视频加载中
        /// </summary>
        public event Action OnVideoLoading;
        #endregion

        #region 属性
        public bool IsPlaying => isPlaying;
        public bool IsPaused => isPaused;
        public bool IsLoading => isLoading;
        public float CurrentTime => currentTime;
        public float Duration => duration;
        public float Progress => duration > 0 ? currentTime / duration : 0f;
        public MediaPlayer MediaPlayer => mediaPlayer;
        #endregion

        #region 私有方法
        private void Awake()
        {
            InitializeMediaPlayer();
        }

        private void Update()
        {
            if (mediaPlayer != null && isPlaying)
            {
                UpdateVideoStatus();
            }
        }

        /// <summary>
        /// 初始化媒体播放器
        /// </summary>
        private void InitializeMediaPlayer()
        {
            if (mediaPlayer == null)
            {
                mediaPlayer = GetComponent<MediaPlayer>();
                if (mediaPlayer == null)
                {
                    Debug.LogError("[AVProVideoManager] MediaPlayer组件未找到！");
                    return;
                }
            }

            // 注册事件
            mediaPlayer.Events.AddListener(OnMediaPlayerEvent);

            // 设置初始参数
            mediaPlayer.Loop = loop;
            mediaPlayer.AudioVolume = defaultVolume;
        }

        /// <summary>
        /// 更新视频状态
        /// </summary>
        private void UpdateVideoStatus()
        {
            if (mediaPlayer.Control != null)
            {
                currentTime = (float)mediaPlayer.Control.GetCurrentTime() / 1000f;
                duration = (float)mediaPlayer.Info.GetDuration() / 1000f;

                // 触发进度回调
                OnVideoProgress?.Invoke(Progress);

                // 检查是否播放完成
                if (!loop && currentTime >= duration - 0.1f && duration > 0)
                {
                    OnVideoCompleted?.Invoke();
                    Stop();
                }
            }
        }
        #endregion

        #region 视频加载
        /// <summary>
        /// 加载视频
        /// </summary>
        /// <param name="path">StreamingAssets下路径</param>
        /// <param name="pathType"></param>
        public void LoadVideo(string path, MediaPathType pathType = MediaPathType.RelativeToStreamingAssetsFolder)
        {
            if (mediaPlayer == null) return;

            isLoading = true;
            OnVideoLoading?.Invoke();

            mediaPlayer.OpenMedia(new MediaPath(path, pathType), autoPlay);
        }

        /// <summary>
        /// 从StreamingAssets加载视频
        /// </summary>
        /// <param name="videoName"></param>
        public void LoadVideoFromStreamingAssets(string videoName)
        {
            LoadVideo(videoName, MediaPathType.RelativeToStreamingAssetsFolder);
        }

        /// <summary>
        /// 从URL加载视频
        /// </summary>
        public void LoadVideoFromURL(string url)
        {
            LoadVideo(url, MediaPathType.AbsolutePathOrURL);
        }
        #endregion

        #region 播放控制
        /// <summary>
        /// 播放视频
        /// </summary>
        public void Play()
        {
            if (mediaPlayer == null || mediaPlayer.Control == null) return;

            if (isPaused)
            {
                Resume();
            }
            else
            {
                mediaPlayer.Control.Play();
                isPlaying = true;
                isPaused = false;
                OnVideoStarted?.Invoke();
            }
        }

        /// <summary>
        /// 暂停视频
        /// </summary>
        public void Pause()
        {
            if (mediaPlayer == null || mediaPlayer.Control == null || !isPlaying) return;

            mediaPlayer.Control.Pause();
            isPaused = true;
            OnVideoPaused?.Invoke();
        }

        /// <summary>
        /// 恢复播放
        /// </summary>
        public void Resume()
        {
            if (mediaPlayer == null || mediaPlayer.Control == null || !isPaused) return;

            mediaPlayer.Control.Play();
            isPaused = false;
            OnVideoResumed?.Invoke();
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        public void Stop()
        {
            if (mediaPlayer == null || mediaPlayer.Control == null) return;

            mediaPlayer.Control.Stop();
            isPlaying = false;
            isPaused = false;
            currentTime = 0f;
            OnVideoStopped?.Invoke();
        }

        /// <summary>
        /// 播放/暂停切换
        /// </summary>
        public void TogglePlayPause()
        {
            if (isPlaying && !isPaused)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        /// <summary>
        /// 重新播放
        /// </summary>
        public void Replay()
        {
            Seek(0f);
            Play();
        }
        #endregion

        #region 播放进度控制
        /// <summary>
        /// 跳转到指定时间（秒）
        /// </summary>
        public void Seek(float timeInSeconds)
        {
            if (mediaPlayer == null || mediaPlayer.Control == null) return;
            mediaPlayer.Control.Seek(timeInSeconds * 1000f);
        }

        /// <summary>
        /// 跳转到指定进度（0-1）
        /// </summary>
        public void SeekToProgress(float progress)
        {
            progress = Mathf.Clamp01(progress);
            Seek(duration * progress);
        }

        /// <summary>
        /// 快进指定秒数
        /// </summary>
        public void FastForward(float seconds)
        {
            Seek(currentTime + seconds);
        }

        /// <summary>
        /// 快退指定秒数
        /// </summary>
        public void Rewind(float seconds)
        {
            Seek(Mathf.Max(0, currentTime - seconds));
        }
        #endregion

        #region 音量控制
        /// <summary>
        /// 设置音量（0-1）
        /// </summary>
        public void SetVolume(float volume)
        {
            if (mediaPlayer == null) return;
            mediaPlayer.AudioVolume = Mathf.Clamp01(volume);
        }

        /// <summary>
        /// 静音/取消静音
        /// </summary>
        public void ToggleMute()
        {
            if (mediaPlayer == null) return;
            mediaPlayer.AudioMuted = !mediaPlayer.AudioMuted;
        }

        /// <summary>
        /// 设置静音状态
        /// </summary>
        public void SetMute(bool mute)
        {
            if (mediaPlayer == null) return;
            mediaPlayer.AudioMuted = mute;
        }

        /// <summary>
        /// 淡入音量
        /// </summary>
        public void FadeInVolume(float targetVolume = 1f, float duration = 1f)
        {
            StartCoroutine(FadeVolumeCoroutine(0f, targetVolume, duration));
        }

        /// <summary>
        /// 淡出音量
        /// </summary>
        public void FadeOutVolume(float duration = 1f)
        {
            StartCoroutine(FadeVolumeCoroutine(mediaPlayer.AudioVolume, 0f, duration));
        }

        private IEnumerator FadeVolumeCoroutine(float startVolume, float targetVolume, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
                SetVolume(volume);
                yield return null;
            }
            SetVolume(targetVolume);
        }
        #endregion

        #region 播放速度控制
        /// <summary>
        /// 设置播放速度
        /// </summary>
        public void SetPlaybackRate(float rate)
        {
            if (mediaPlayer == null || mediaPlayer.Control == null) return;
            mediaPlayer.Control.SetPlaybackRate(rate);
        }

        /// <summary>
        /// 恢复正常速度
        /// </summary>
        public void ResetPlaybackRate()
        {
            SetPlaybackRate(1f);
        }
        #endregion

        #region 循环控制
        /// <summary>
        /// 设置循环播放
        /// </summary>
        public void SetLoop(bool enable)
        {
            if (mediaPlayer == null) return;
            loop = enable;
            mediaPlayer.Loop = enable;
        }

        /// <summary>
        /// 切换循环状态
        /// </summary>
        public void ToggleLoop()
        {
            SetLoop(!loop);
        }
        #endregion

        #region 事件处理
        private void OnMediaPlayerEvent(MediaPlayer mp, MediaPlayerEvent.EventType eventType, ErrorCode errorCode)
        {
            switch (eventType)
            {
                case MediaPlayerEvent.EventType.ReadyToPlay:
                    isLoading = false;
                    duration = (float)mediaPlayer.Info.GetDuration() / 1000f;
                    OnVideoLoaded?.Invoke();
                    if (autoPlay)
                    {
                        Play();
                    }
                    break;

                case MediaPlayerEvent.EventType.Started:
                    isPlaying = true;
                    isPaused = false;
                    OnVideoStarted?.Invoke();
                    break;

                case MediaPlayerEvent.EventType.FirstFrameReady:
                    //Debug.Log("[AVProVideoManager] 第一帧已准备就绪");
                    break;

                case MediaPlayerEvent.EventType.FinishedPlaying:
                    OnVideoCompleted?.Invoke();
                    if (!loop)
                    {
                        Stop();
                    }
                    break;

                case MediaPlayerEvent.EventType.Error:
                    isLoading = false;
                    isPlaying = false;
                    string errorMessage = $"视频播放错误: {errorCode}";
                    Debug.LogError($"[AVProVideoManager] {errorMessage}");
                    OnVideoError?.Invoke(errorMessage);
                    break;
            }
        }
        #endregion

        #region 实用功能
        /// <summary>
        /// 获取视频信息
        /// </summary>
        public string GetVideoInfo()
        {
            if (mediaPlayer == null || mediaPlayer.Info == null) return "无视频信息";

            return $"分辨率: {mediaPlayer.Info.GetVideoWidth()}x{mediaPlayer.Info.GetVideoHeight()}\n" +
                   $"帧率: {mediaPlayer.Info.GetVideoFrameRate():F2} fps\n" +
                   $"时长: {duration:F2}秒\n" +
                   $"是否有音频: {mediaPlayer.Info.HasAudio()}\n" +
                   $"是否有视频: {mediaPlayer.Info.HasVideo()}";
        }

        /// <summary>
        /// 截取当前帧
        /// </summary>
        public Texture2D CaptureFrame()
        {
            if (mediaPlayer == null || mediaPlayer.TextureProducer == null) return null;

            Texture texture = mediaPlayer.TextureProducer.GetTexture();
            if (texture == null) return null;

            RenderTexture currentRT = RenderTexture.active;
            RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 0);
            Graphics.Blit(texture, renderTexture);
            RenderTexture.active = renderTexture;

            Texture2D snapshot = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
            snapshot.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            snapshot.Apply();

            RenderTexture.active = currentRT;
            RenderTexture.ReleaseTemporary(renderTexture);

            return snapshot;
        }
        #endregion

        #region 清理
        private void OnDestroy()
        {
            if (mediaPlayer != null)
            {
                mediaPlayer.Events.RemoveListener(OnMediaPlayerEvent);
                mediaPlayer.CloseMedia();
            }

            // 清空所有事件订阅
            OnVideoStarted = null;
            OnVideoPaused = null;
            OnVideoResumed = null;
            OnVideoStopped = null;
            OnVideoCompleted = null;
            OnVideoError = null;
            OnVideoProgress = null;
            OnVideoLoaded = null;
            OnVideoLoading = null;
        }
        #endregion
    }
}