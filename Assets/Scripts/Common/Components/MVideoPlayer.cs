using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class MVideoPlayer : MonoBehaviour
{
    public enum MPlayType
    {
        None = 0,
        Normal = 1,
        EasyIn = 2,
        EasyOut = 3,
        EasyInOut = 4,
    }

#if UNITY_EDITOR
    public bool logInfo = false;
#endif

    private Dictionary<long, List<VideoPlayer.EventHandler>> _videoPlayerEventHandlers = new Dictionary<long, List<VideoPlayer.EventHandler>>();
    private Dictionary<long, List<System.Action>> _eventHandlers = new Dictionary<long, List<System.Action>>();
    private Dictionary<long, List<System.Action<MVideoPlayer>>> _mVideoPlayerEventHandlers = new Dictionary<long, List<System.Action<MVideoPlayer>>>();
    private Dictionary<long, System.Action> _internalEventHandlers = new Dictionary<long, System.Action>();
    private List<long> eventeds = new List<long>();
    private Dictionary<long, VideoPlayer.EventHandler> _videoPlayerRemoveEventHandlers = new Dictionary<long, VideoPlayer.EventHandler>();
    private Dictionary<long, System.Action> _eventRemoveHandlers = new Dictionary<long, System.Action>();
    private Dictionary<long, System.Action<MVideoPlayer>> _mVideoPlayerRemoveEventHandlers = new Dictionary<long, System.Action<MVideoPlayer>>();
    public VideoPlayer m_videoPlayer;
    public VideoClip m_videoClip;
    private Renderer m_renderer;
    public RawImage m_RawImage;
    public float alphaTime = 1;
    private VideoPlayer.EventHandler _eventHandler;
    private const string _COLOR = "_Color";
    private MPlayType _mPlayType = MPlayType.Normal;
    private bool _setAlphaComplete = true;
    private bool _isPlaying = false;//用于处理主动停止播放

    private void Awake()
    {
        if (m_videoPlayer == null)
            m_videoPlayer = this.GetComponent<VideoPlayer>();
        m_videoClip = m_videoPlayer.clip;
    }

    // Use this for initialization
    void Start()
    {

    }

    public VideoClip clip
    {
        set
        {
            m_videoPlayer.clip = value;
            m_videoClip = value;
        }
        get
        {
            if (m_videoClip == null)
                m_videoClip = m_videoPlayer.clip;
            return m_videoClip;
        }
    }

    public VideoRenderMode renderMode
    {
        set
        {
            m_videoPlayer.renderMode = value;
        }
        get
        {
            return m_videoPlayer.renderMode;
        }
    }

    public new Renderer renderer
    {
        get
        {
            if (!m_renderer)
                m_renderer = m_videoPlayer.targetMaterialRenderer;
            return m_renderer;
        }
        set
        {
            m_renderer = value;
            m_videoPlayer.targetMaterialRenderer = value;
        }
    }

    public RawImage rawImage
    {
        get
        {
            if (!m_RawImage)
                m_RawImage = GetComponent<RawImage>();
            return m_RawImage;
        }
        set
        {
            m_RawImage = value;
        }
    }

    private float _alpha;
    public float alpha
    {
        get
        {
            return _alpha;
        }
        set
        {
            _alpha = value;
            switch (renderMode)
            {
                case VideoRenderMode.CameraFarPlane:
                    break;
                case VideoRenderMode.CameraNearPlane:
                    break;
                case VideoRenderMode.RenderTexture:
                    if (rawImage)
                    {
                        Color col = rawImage.color;
                        col.a = value;
                        rawImage.color = col;
                        setAlphaComplete = false;
                    }
                    break;
                case VideoRenderMode.MaterialOverride:
                    if (renderer)
                    {
                        Color col = renderer.material.color;
                        col.a = value;
                        renderer.material.color = col;
                        setAlphaComplete = false;
                    }
                    break;
                case VideoRenderMode.APIOnly:
                    break;
            }

            setAlphaComplete = true;
        }
    }

    private bool setAlphaComplete
    {
        set
        {
            _setAlphaComplete = value;
        }
        get
        {
            switch (renderMode)
            {
                case VideoRenderMode.CameraFarPlane:
                    break;
                case VideoRenderMode.CameraNearPlane:
                    break;
                case VideoRenderMode.RenderTexture:
                    if (rawImage)
                        _setAlphaComplete = Mathf.Abs(rawImage.color.a - _alpha) < 0.001f;
                    break;
                case VideoRenderMode.MaterialOverride:
                    if (renderer)
                        _setAlphaComplete = Mathf.Abs(renderer.material.color.a - _alpha) < 0.001f;
                    break;
                case VideoRenderMode.APIOnly:
                    break;
            }
            return _setAlphaComplete;
        }
    }

    public float playbackSpeed
    {
        get { return m_videoPlayer.playbackSpeed; }
        set { m_videoPlayer.playbackSpeed = value; }
    }

    public long frame
    {
        get { return m_videoPlayer.frame; }
        set { m_videoPlayer.frame = value; }
    }

    public ulong frameCount
    {
        get { return m_videoPlayer.frameCount; }
    }

    public double time
    {
        get
        {
            return m_videoPlayer.time;
        }
        set
        {
            m_videoPlayer.time = value;
        }
    }

    /// <summary>
    /// 获取视频长度{get;}
    /// </summary>
    public double length
    {
        get
        {
            return clip.length;
        }
    }

    public bool isPlaying
    {
        get
        {
            return m_videoPlayer.isPlaying;
        }
    }

    public bool isLooping
    {
        get { return m_videoPlayer.isLooping; }
        set { m_videoPlayer.isLooping = value; }
    }

    /// <summary>
    /// 注册视频播放目标时间回调
    /// </summary>
    /// <param name="time">目标时间</param>
    /// <param name="eventHandler">回调</param>
    public void RegisterEvent(float time, VideoPlayer.EventHandler eventHandler)
    {
        if (eventHandler == null) return;
        long targetFrame = (long)(time * m_videoClip.frameRate);
        RegisterEvent(targetFrame, eventHandler);
        Debug.LogFormat("already RegisterEvent:time:{0},target frame:{1}", time, targetFrame);
    }

    /// <summary>
    /// 注册视频播放目标帧数回调
    /// </summary>
    /// <param name="time">目标帧数</param>
    /// <param name="eventHandler">回调</param>
    public void RegisterEvent(long targetFrame, VideoPlayer.EventHandler eventHandler)
    {
        List<VideoPlayer.EventHandler> list = null;
        if (!_videoPlayerEventHandlers.TryGetValue(targetFrame, out list))
            list = new List<VideoPlayer.EventHandler>();
        list.Add(eventHandler);
        _videoPlayerEventHandlers[targetFrame] = list;
    }

    /// <summary>
    /// 注册视频播放目标时间回调
    /// </summary>
    /// <param name="time">目标时间</param>
    /// <param name="eventHandler">无参数回调</param>
    public void RegisterEvent(float time, System.Action action)
    {
        if (action == null) return;
        long targetFrame = (long)(time * m_videoClip.frameRate);
        RegisterEvent(targetFrame, action);
        Debug.LogFormat("already RegisterEvent:time:{0},target frame:{1}", time, targetFrame);
    }

    /// <summary>
    /// 注册视频播放目标时间回调
    /// </summary>
    /// <param name="targetFrame">目标帧数</param>
    /// <param name="action">无参数回调</param>
    public void RegisterEvent(long targetFrame, System.Action action)
    {
        List<System.Action> list = null;
        if (!_eventHandlers.TryGetValue(targetFrame, out list))
            list = new List<System.Action>();
        list.Add(action);
        _eventHandlers[targetFrame] = list;
    }

    /// <summary>
    /// 注册视频播放目标时间回调
    /// </summary>
    /// <param name="time">目标时间</param>
    /// <param name="eventHandler">回调</param>
    public void RegisterEvent(float time, System.Action<MVideoPlayer> action)
    {
        if (action == null) return;
        long targetFrame = (long)(time * m_videoClip.frameRate);
        RegisterEvent(targetFrame, action);
        Debug.LogFormat("already RegisterEvent:time:{0},target frame:{1}", time, targetFrame);
    }

    /// <summary>
    /// 注册视频播放目标帧数回调
    /// </summary>
    /// <param name="targetFrame">目标帧数</param>
    /// <param name="eventHandler">回调</param>
    public void RegisterEvent(long targetFrame, System.Action<MVideoPlayer> action)
    {
        List<System.Action<MVideoPlayer>> list = null;
        if (!_mVideoPlayerEventHandlers.TryGetValue(targetFrame, out list))
            list = new List<System.Action<MVideoPlayer>>();
        list.Add(action);
        _mVideoPlayerEventHandlers[targetFrame] = list;
    }

    private void InternalRegisterEvent(float time, System.Action action)
    {
        if (action == null) return;
        long targetFrame = (long)(time * m_videoPlayer.frameRate);
        InternalRegisterEvent(targetFrame, action);
        Debug.LogFormat("already InternalRegisterEvent:time:{0},target frame:{1}", time, targetFrame);
    }

    private void InternalRegisterEvent(long targetFrame, System.Action action)
    {
        lock (_internalEventHandlers)
        {
            _internalEventHandlers[targetFrame] = action;
        }
        Debug.LogFormat("already InternalRegisterEvent:target frame:{0}", targetFrame);
    }

#if UNITY_EDITOR
    [ContextMenu("Play")]
    public void PlayTest()
    {
        Play();
    }

    [ContextMenu("Complete")]
    public void Complete()
    {
        this.time = length - 1f;
        m_videoPlayer.Play();
    }
#endif

    /// <summary>
    /// 播放视频
    /// </summary>
    /// <param name="normalTime">归一化的时间(0-1)</param>   
    /// <param name="mPlayType">播放模式，仅支持VideoRenderMode MaterialOverride 和RenderTexture模式</param>   
    public void Play(float normalTime = 0f, MPlayType mPlayType = MPlayType.Normal)
    {
        if (m_videoPlayer == null) return;
        InternalPlay((float)(normalTime * length), mPlayType);
    }

    /// <summary>
    /// 播放视频
    /// </summary>
    /// <param name="time">指定时间点</param>
    /// <param name="mPlayType">播放模式，仅支持VideoRenderMode MaterialOverride 和RenderTexture模式</param>   
    public void PlayOnTime(float time, MPlayType mPlayType = MPlayType.Normal)
    {
        if (m_videoPlayer == null) return;
        InternalPlay(time, mPlayType);
    }

    private void InternalPlay(float time, MPlayType mPlayType)
    {
        this.time = time;
        if (!isPlaying)
            m_videoPlayer.Play();
        _isPlaying = true;
        _mPlayType = mPlayType;
        _internalEventHandlers.Clear();
        switch (mPlayType)
        {
            case MPlayType.Normal://donothing
                break;
            case MPlayType.EasyIn:
                if (renderMode == VideoRenderMode.MaterialOverride || renderMode == VideoRenderMode.RenderTexture)
                {
                    alpha = 0f;
                    StartCoroutine("EaysInCoroutine");
                }
                break;
            case MPlayType.EasyOut:
                if (renderMode == VideoRenderMode.MaterialOverride || renderMode == VideoRenderMode.RenderTexture)
                {
                    ResetAlpha();
                    InternalRegisterEvent((long)(m_videoClip.frameCount - 1), InternalPlayCompleteAction);
                }
                break;
            case MPlayType.EasyInOut:
                if (renderMode == VideoRenderMode.MaterialOverride || renderMode == VideoRenderMode.RenderTexture)
                {
                    //Pause();
                    alpha = 0f;
                    StartCoroutine("EaysInCoroutine");
                    InternalRegisterEvent((long)(m_videoClip.frameCount - 1), InternalPlayCompleteAction);
                }
                break;
        }
#if UNITY_EDITOR
        Log();
#endif
    }

    private IEnumerator EaysInCoroutine()
    {
        //Pause();
        int count = (int)(alphaTime / (1f / 30));
        int i = 0;
        alpha = 0f;
        while (i < count - 1)
        {
            yield return null;
            alpha = Mathf.Lerp(alpha, 1f, 0.1f);
            //Debug.LogFormat("EaysInCoroutine count:{0},i:{1},alpha:{2}", count, i, alpha);
            i++;
        }
        alpha = 1;
    }

    private IEnumerator EasyOutCoroutine()
    {
        int times = (int)(alphaTime / (1f / 30));
        int j = 0;
        alpha = 1;
        while (j < times - 1)
        {
            yield return null;
            alpha = Mathf.Lerp(alpha, 0f, 0.1f);
            //Debug.LogFormat("EasyOutCoroutine count:{0},i:{1},alpha:{2}", count, i, alpha);
            j++;
        }
        alpha = 0;
        if (_isPlaying)
        {
            long fc = (long)m_videoClip.frameCount;
            if (_videoPlayerEventHandlers.ContainsKey(fc))
            {
                List<VideoPlayer.EventHandler> list = _videoPlayerEventHandlers[fc];
                for (int i = 0, count = list.Count; i < count; i++)
                {
                    list[i](m_videoPlayer);
                }
            }
            if (_eventHandlers.ContainsKey(fc))
            {
                List<System.Action> list = _eventHandlers[fc];
                for (int i = 0, count = list.Count; i < count; i++)
                {
                    Debug.Log(fc.ToString() + " clip:" + m_videoClip.name);
                    list[i]();
                }
            }
            if (_mVideoPlayerEventHandlers.ContainsKey(fc))
            {
                List<System.Action<MVideoPlayer>> list = _mVideoPlayerEventHandlers[fc];
                for (int i = 0, count = list.Count; i < count; i++)
                {
                    list[i](this);
                }
            }
        }
    }

    private void InternalPlayCompleteAction()
    {
        Debug.Log("InternalPlayCompleteAction");
        StartCoroutine("EasyOutCoroutine");
    }

    private void ResetAlpha()
    {
        alpha = 1f;
    }

#if UNITY_EDITOR    
    private void Log()
    {
        if (Application.isPlaying)
        {
            StopCoroutine("LogPerSeconds");
            StartCoroutine("LogPerSeconds");
        }
    }

    private IEnumerator LogPerSeconds()
    {
        WaitForSeconds wf = new WaitForSeconds(1f);
        while (true)
        {
            if (logInfo)
                Debug.Log(string.Format("MVideoPlayer:frame count:{0},frame:{1},length:{2},time:{3}", frameCount, frame, length, time));
            yield return wf;
        }
    }
#endif

    /// <summary>
    /// 恢复播放视频
    /// </summary>
    public void Resume()
    {
        if (!m_videoPlayer.isPlaying)
            m_videoPlayer.Play();
        _isPlaying = true;
    }

    /// <summary>
    /// 暂停播放视频
    /// </summary>
    [ContextMenu("Pause")]
    public void Pause()
    {
        if (m_videoPlayer == null) return;
        _isPlaying = false;
        m_videoPlayer.Pause();
    }

    /// <summary>
    /// 停止播放视频
    /// </summary>
    [ContextMenu("Stop")]
    public void Stop()
    {
        if (m_videoPlayer == null) return;
        _isPlaying = false;
        ResetEvents();
        m_videoPlayer.Stop();
        StopCoroutine("EasyOutCoroutine");
    }

    public void ClearEvents()
    {
        _videoPlayerEventHandlers.Clear();
        _eventHandlers.Clear();
        _mVideoPlayerEventHandlers.Clear();
        _internalEventHandlers.Clear();
    }

    private void ResetEvents()
    {
        eventeds.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isPlaying) return;
        if (!setAlphaComplete)
            alpha = _alpha;
        if (eventeds.Count > 0)
            if (eventeds.Contains(frame)) return;
        if (_internalEventHandlers.Count > 0)
        {
            lock (_internalEventHandlers)
            {
                if (_internalEventHandlers.Count > 0)
                {
                    Dictionary<long, System.Action>.Enumerator e = _internalEventHandlers.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (e.Current.Key == frame)
                        {
                            e.Current.Value();
                        }
                        eventeds.Add(frame);
                    }
                }
            }
        }
        if (_videoPlayerEventHandlers.Count > 0)
        {
            Dictionary<long, List<VideoPlayer.EventHandler>>.Enumerator e = _videoPlayerEventHandlers.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Key == frame)
                {
                    if ((ulong)frame == m_videoClip.frameCount && (_mPlayType == MPlayType.EasyInOut || _mPlayType == MPlayType.EasyOut))
                    {
                        //delay alpha time
                    }
                    else
                    {
                        List<VideoPlayer.EventHandler> list = e.Current.Value;
                        for (int i = 0, count = list.Count; i < count; i++)
                        {
                            list[i](m_videoPlayer);
                        }
                    }
                    eventeds.Add(frame);
                }
            }
        }
        if (_eventHandlers.Count > 0)
        {
            Dictionary<long, List<System.Action>>.Enumerator e = _eventHandlers.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Key == frame)
                {
                    if ((ulong)frame == m_videoClip.frameCount && (_mPlayType == MPlayType.EasyInOut || _mPlayType == MPlayType.EasyOut))
                    {
                        //delay alpha time
                    }
                    else
                    {
                        List<System.Action> list = e.Current.Value;
                        for (int i = 0, count = list.Count; i < count; i++)
                        {
                            list[i]();
                        }
                    }
                    eventeds.Add(frame);
                }
            }
        }
        if (_mVideoPlayerEventHandlers.Count > 0)
        {
            Dictionary<long, List<System.Action<MVideoPlayer>>>.Enumerator e = _mVideoPlayerEventHandlers.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Key == frame)
                {
                    if ((ulong)frame == m_videoClip.frameCount && (_mPlayType == MPlayType.EasyInOut || _mPlayType == MPlayType.EasyOut))
                    {
                        //delay alpha time
                    }
                    else
                    {
                        List<System.Action<MVideoPlayer>> list = e.Current.Value;
                        for (int i = 0, count = list.Count; i < count; i++)
                        {
                            list[i](this);
                        }
                    }
                    eventeds.Add(frame);
                }
            }
        }
    }

}
