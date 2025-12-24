using System;
using System.Collections;
using UnityEngine;

namespace XQ
{
    /// <summary>
    /// 倒计时器类 - 功能完善的计时器系统
    /// </summary>
    public class CountdownTimer
    {
        #region 属性和字段

        /// <summary>
        /// 计时器总时长（秒）
        /// </summary>
        public float Duration { get; private set; }

        /// <summary>
        /// 当前已经过的时间（秒）
        /// </summary>
        public float ElapsedTime { get; private set; }

        /// <summary>
        /// 剩余时间（秒）
        /// </summary>
        public float RemainingTime => Duration - ElapsedTime;

        /// <summary>
        /// 计时进度（0-1）
        /// </summary>
        public float Progress => Duration > 0 ? Mathf.Clamp01(ElapsedTime / Duration) : 1f;

        /// <summary>
        /// 是否正在运行
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// 是否暂停
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// 是否受TimeScale影响
        /// </summary>
        public bool UseUnscaledTime { get; set; }

        /// <summary>
        /// 是否正向计时（true:正向，false:倒计时）
        /// </summary>
        public bool IsForward { get; set; }

        /// <summary>
        /// 是否循环计时
        /// </summary>
        public bool IsLoop { get; set; }

        /// <summary>
        /// 完成后是否自动销毁
        /// </summary>
        public bool DestroyOnComplete { get; set; }

        /// <summary>
        /// 计时完成回调
        /// </summary>
        public event Action OnComplete;

        /// <summary>
        /// 每帧更新回调（参数：已用时间，剩余时间，进度）
        /// </summary>
        public event Action<float, float, float> OnUpdate;

        /// <summary>
        /// 暂停回调
        /// </summary>
        public event Action OnPause;

        /// <summary>
        /// 恢复回调
        /// </summary>
        public event Action OnResume;

        /// <summary>
        /// 重置回调
        /// </summary>
        public event Action OnReset;

        /// <summary>
        /// 销毁回调
        /// </summary>
        public event Action OnDestroy;

        private MonoBehaviour _coroutineRunner;
        private Coroutine _timerCoroutine;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建一个倒计时器
        /// </summary>
        /// <param name="duration">持续时间（秒）</param>
        /// <param name="onComplete">完成回调</param>
        /// <param name="useUnscaledTime">是否不受TimeScale影响</param>
        /// <param name="isForward">是否正向计时</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="destroyOnComplete">完成后是否销毁</param>
        public CountdownTimer(float duration, Action onComplete = null, bool useUnscaledTime = false,
            bool isForward = false, bool isLoop = false, bool destroyOnComplete = false)
        {
            Duration = duration;
            OnComplete = onComplete;
            UseUnscaledTime = useUnscaledTime;
            IsForward = isForward;
            IsLoop = isLoop;
            DestroyOnComplete = destroyOnComplete;
            ElapsedTime = 0f;
            IsRunning = false;
            IsCompleted = false;
            IsPaused = false;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 启动计时器
        /// </summary>
        /// <param name="runner">协程运行器（MonoBehaviour）</param>
        public void Start(MonoBehaviour runner)
        {
            if (runner == null)
            {
                Debug.LogError("[CountdownTimer] 协程运行器不能为空！");
                return;
            }

            if (IsRunning)
            {
                Debug.LogWarning("[CountdownTimer] 计时器已经在运行中！");
                return;
            }

            _coroutineRunner = runner;
            IsRunning = true;
            IsCompleted = false;
            IsPaused = false;
            _timerCoroutine = _coroutineRunner.StartCoroutine(TimerCoroutine());
        }

        /// <summary>
        /// 暂停计时器
        /// </summary>
        public void Pause()
        {
            if (!IsRunning || IsPaused)
            {
                return;
            }

            IsPaused = true;
            OnPause?.Invoke();
        }

        /// <summary>
        /// 恢复计时器
        /// </summary>
        public void Resume()
        {
            if (!IsRunning || !IsPaused)
            {
                return;
            }

            IsPaused = false;
            OnResume?.Invoke();
        }

        /// <summary>
        /// 停止计时器
        /// </summary>
        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            IsRunning = false;
            IsPaused = false;

            if (_timerCoroutine != null && _coroutineRunner != null)
            {
                _coroutineRunner.StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
        }

        /// <summary>
        /// 重置计时器
        /// </summary>
        /// <param name="restart">是否立即重新开始</param>
        public void Reset(bool restart = false)
        {
            Stop();
            ElapsedTime = 0f;
            IsCompleted = false;
            OnReset?.Invoke();

            if (restart && _coroutineRunner != null)
            {
                Start(_coroutineRunner);
            }
        }

        /// <summary>
        /// 重新开始计时
        /// </summary>
        public void Restart()
        {
            Reset(true);
        }

        /// <summary>
        /// 设置时间（秒）
        /// </summary>
        /// <param name="time">要设置的时间</param>
        public void SetTime(float time)
        {
            ElapsedTime = Mathf.Clamp(time, 0f, Duration);
        }

        /// <summary>
        /// 设置持续时间
        /// </summary>
        /// <param name="duration">新的持续时间</param>
        /// <param name="resetTimer">是否重置计时器</param>
        public void SetDuration(float duration, bool resetTimer = false)
        {
            Duration = duration;
            if (resetTimer)
            {
                Reset();
            }
        }

        /// <summary>
        /// 添加时间
        /// </summary>
        /// <param name="addTime">要添加的时间（秒）</param>
        public void AddTime(float addTime)
        {
            Duration += addTime;
        }

        /// <summary>
        /// 销毁计时器
        /// </summary>
        public void Destroy()
        {
            Stop();
            OnDestroy?.Invoke();

            // 清空所有事件
            OnComplete = null;
            OnUpdate = null;
            OnPause = null;
            OnResume = null;
            OnReset = null;
            OnDestroy = null;

            _coroutineRunner = null;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 计时器协程
        /// </summary>
        private IEnumerator TimerCoroutine()
        {
            while (IsRunning)
            {
                // 如果暂停，等待恢复
                if (IsPaused)
                {
                    yield return null;
                    continue;
                }

                // 获取增量时间
                float deltaTime = UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

                // 根据计时方向更新时间
                if (IsForward)
                {
                    ElapsedTime += deltaTime;
                }
                else
                {
                    ElapsedTime += deltaTime;
                }

                // 触发更新回调
                OnUpdate?.Invoke(ElapsedTime, RemainingTime, Progress);

                // 检查是否完成
                if (ElapsedTime >= Duration)
                {
                    ElapsedTime = Duration;
                    IsCompleted = true;

                    // 触发完成回调
                    OnComplete?.Invoke();

                    // 处理循环
                    if (IsLoop)
                    {
                        ElapsedTime = 0f;
                        IsCompleted = false;
                    }
                    else
                    {
                        IsRunning = false;

                        // 处理销毁
                        if (DestroyOnComplete)
                        {
                            Destroy();
                        }

                        yield break;
                    }
                }

                yield return null;
            }
        }

        #endregion
    }

    /// <summary>
    /// 倒计时管理器 - 用于创建和管理多个计时器
    /// </summary>
    public class CountdownTimerManager : MonoBehaviour
    {
        private static CountdownTimerManager _instance;

        /// <summary>
        /// 单例实例
        /// </summary>
        public static CountdownTimerManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("[CountdownTimerManager]");
                    _instance = go.AddComponent<CountdownTimerManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        /// <summary>
        /// 创建一个倒计时器并立即启动
        /// </summary>
        /// <param name="duration">持续时间（秒）</param>
        /// <param name="onComplete">完成回调</param>
        /// <param name="useUnscaledTime">是否不受TimeScale影响</param>
        /// <param name="isForward">是否正向计时</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="destroyOnComplete">完成后是否销毁</param>
        /// <returns>创建的计时器实例</returns>
        public static CountdownTimer CreateTimer(float duration, Action onComplete = null,
            bool useUnscaledTime = false, bool isForward = false, bool isLoop = false,
            bool destroyOnComplete = false)
        {
            CountdownTimer timer = new CountdownTimer(duration, onComplete, useUnscaledTime,
                isForward, isLoop, destroyOnComplete);
            timer.Start(Instance);
            return timer;
        }

        /// <summary>
        /// 创建一个倒计时器（不自动启动）
        /// </summary>
        /// <param name="duration">持续时间（秒）</param>
        /// <param name="onComplete">完成回调</param>
        /// <param name="useUnscaledTime">是否不受TimeScale影响</param>
        /// <param name="isForward">是否正向计时</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="destroyOnComplete">完成后是否销毁</param>
        /// <returns>创建的计时器实例</returns>
        public static CountdownTimer CreateTimerWithoutStart(float duration, Action onComplete = null,
            bool useUnscaledTime = false, bool isForward = false, bool isLoop = false,
            bool destroyOnComplete = false)
        {
            return new CountdownTimer(duration, onComplete, useUnscaledTime, isForward,
                isLoop, destroyOnComplete);
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
