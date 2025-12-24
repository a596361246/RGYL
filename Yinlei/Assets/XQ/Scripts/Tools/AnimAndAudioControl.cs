using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayType
{
    PT_Audio,
    PT_AllGroupLoop,
    PT_SingleGroupLoop
}

[RequireComponent(typeof(Image))]
public class AnimAndAudioControl : MonoBehaviour
{
    public PlayType playType = PlayType.PT_Audio;

    private Image img;
    [Header("第一组序列帧(说话)")] 
    [SerializeField] private Sprite[] _firstGroupSprites;
    [Header("第二组序列帧(循环)")]
    [SerializeField] private Sprite[] _secondGroupSprites;
    /// <summary>
    /// 计时器
    /// </summary>
    private float m_timer = 0;
    /// <summary>
    /// 下标
    /// </summary>
    private int m_index = 0;
    /// <summary>
    /// 是否播放完毕
    /// </summary>
    private bool isOver = false;
    /// <summary>
    /// 切帧速度
    /// </summary>
    [Header("切帧速度")]
    public float _framingSpeed = 0.025f;

    /// <summary>
    /// 根据语音播放
    /// </summary>
    [Header("语音时长(等待语音播放完毕后切换为第二组序列帧)")]
    [SerializeField] private AudioClip _audioClip;
    private float _curAudioTimer = .0f;
    private bool _isOverAudio = false;

    /// <summary>
    /// 仅循环播放序列帧
    /// </summary>
    [Header("仅播放序列帧(仅循环播放序列帧)")]
    private bool _isFirstSprites = true;   //当前播放的序列组是否为第一组

    /// <summary>
    /// 仅播放一遍序列帧
    /// </summary>
    [Header("仅播放一遍序列帧(结束播放执行事件)")]
    public UnityEngine.UI.Button.ButtonClickedEvent _overAction;

    void Awake()
    {
        m_index = 0;
        img = transform.GetComponent<Image>();
    }
    private void OnEnable()
    {
        if (_firstGroupSprites.Length != 0)
            img.sprite = _firstGroupSprites[0];
        else
            _isFirstSprites = false;
    }

    void Update()
    {
        switch(playType)
        {
            case PlayType.PT_Audio:
                {
                    if (!isOver)
                    {
                        SequenceFramePlay();
                    }
                    break;
                }
            case PlayType.PT_AllGroupLoop:
                {
                    if (!isOver)
                    {
                        OnlySequenceFramePlay();
                    }
                    break;
                }
            case PlayType.PT_SingleGroupLoop:
                {
                    if (!isOver)
                    {
                        OnlySequenceFramePlayOnce();
                    }
                    break;
                }
        }

    }

    public void SequenceFramePlay()
    {
        m_timer += Time.deltaTime;
        if(m_timer >= _framingSpeed)
        {
            if (!_isOverAudio)
            {
                m_index++;
                if (m_index > _firstGroupSprites.Length - 1)
                {
                    m_index = 0;
                }

                m_timer = 0;
                img.sprite = _firstGroupSprites[m_index];
            }
            else
            {
                m_index++;
                if (m_index > _secondGroupSprites.Length - 1)
                {
                    m_index = 0;
                }

                m_timer = 0;
                img.sprite = _secondGroupSprites[m_index];
            }
        }

        if(!_isOverAudio)
        {
            _curAudioTimer += Time.deltaTime;
            if (_curAudioTimer >= _audioClip.length + 0.1f)
            {
                _isOverAudio = true;
            }
        }
    }

    public void OnlySequenceFramePlay()
    {
        m_timer += Time.deltaTime;
        if (m_timer >= _framingSpeed)
        {
            if (_isFirstSprites)
            {
                m_index++;
                if (m_index > _firstGroupSprites.Length - 1)
                {
                    if(_secondGroupSprites.Length != 0)
                    {
                        _isFirstSprites = false;
                    }
                    m_index = 0;
                }
                img.sprite = _firstGroupSprites[m_index];
            }
            else
            {
                m_index++;
                if (m_index > _secondGroupSprites.Length - 1)
                {
                    m_index = 0;
                }
                img.sprite = _secondGroupSprites[m_index];
            }

            m_timer = 0;
        }
    }

    public void OnlySequenceFramePlayOnce()
    {
        m_timer += Time.deltaTime;
        if (m_timer >= _framingSpeed)
        {
            if (_isFirstSprites)
            {
                m_index++;
                if (m_index > _firstGroupSprites.Length - 1)
                {
                    isOver = true;
                    _overAction?.Invoke();
                    return;
                }
            }

            m_timer = 0;
            img.sprite = _firstGroupSprites[m_index];
        }
    }

    private void OnDisable()
    {
        isOver = false;
        _isOverAudio = false;
        m_timer = 0;
        m_index = 0;
    }
}
