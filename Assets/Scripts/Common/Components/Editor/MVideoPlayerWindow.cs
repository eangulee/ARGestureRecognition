using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MVideoPlayerWindow : UnityEditor.EditorWindow
{

    [MenuItem("Tools/视频播放器", false, 100)]
    static void OpenMVideoPlayerWindow()
    {
        EditorWindow.GetWindow<MVideoPlayerWindow>();
    }

    bool isPlaying = false, pause = false, stop = false;
    float speed = 1f, progress = 0f, newProgress = 0;
    double currentTime = 0f, length = 0f;
    ulong frameCount = 0, frame = 0;
    MVideoPlayer mVideoPlayer;

    void OnGUI()
    {
        if (Selection.activeGameObject != null)
        {
            mVideoPlayer = Selection.activeGameObject.GetComponent<MVideoPlayer>();
            if (mVideoPlayer.clip != null)
            {
                length = mVideoPlayer.clip.length;
                frameCount = mVideoPlayer.clip.frameCount;
            }
            speed = mVideoPlayer.playbackSpeed;
            currentTime = mVideoPlayer.time;
            frame = (ulong)mVideoPlayer.frame;
            if (length != 0)
                progress = (float)(currentTime / length);

        }
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Play"))
        {
            if (mVideoPlayer == null)
                Debug.LogError("Please select a MVideoPlayer script GameObject!");
            else
            {
                mVideoPlayer.Play();
                isPlaying = true;
                pause = false;
                stop = false;
            }
        }
        if (GUILayout.Button("Pause"))
        {
            if (mVideoPlayer == null)
                Debug.LogError("Please select a MVideoPlayer script GameObject!");
            else
            {
                mVideoPlayer.Pause();
                isPlaying = false;
                pause = true;
                stop = false;
            }
        }
        if (GUILayout.Button("Stop"))
        {
            if (mVideoPlayer == null)
                Debug.LogError("Please select a MVideoPlayer script GameObject!");
            else
            {
                mVideoPlayer.Stop();
                isPlaying = false;
                pause = false;
                stop = true;
            }
        }
        EditorGUILayout.EndHorizontal();

        newProgress = EditorGUILayout.Slider(progress, 0, 1);
        if (Mathf.Abs(newProgress - progress) > 0.001f)//滑动改变播放进度
        {
            if (mVideoPlayer != null)
            {
                if (isPlaying)
                    mVideoPlayer.time = (newProgress * length);
            }
        }
    }
}
