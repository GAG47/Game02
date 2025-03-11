using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 声音管理器
/// </summary>
public class SoundManager
{
    private AudioSource bgmSource; //播放bgm的音频组件
    private Dictionary<string, AudioClip> clips; //音频缓存字典

    private bool isStop; //是否静音
    public bool IsStop
    {
        get
        {
            return isStop;
        }
        set
        {
            isStop = value;
            if(isStop == true)
            {
                bgmSource.Pause();
            }
            else
            {
                bgmSource.Play();
            }
        }
    }

    private float bgmValume; //bgm音量大小
    public float BgmValume
    {
        get => bgmValume;
        set
        {
            bgmValume = value;
            bgmSource.volume = bgmValume;
        }
    }

    private float effectVolume; //音量大小(攻击 受伤等短音效)
    public float EffectVolume
    {
        get => effectVolume;
        set
        {
            effectVolume = value;
        }
    }

    public SoundManager()
    {
        clips = new Dictionary<string, AudioClip>();
        bgmSource = GameObject.Find("Game").GetComponent<AudioSource>();
        isStop = false;
        bgmValume = 1;
        effectVolume = 1;
    }

    //播放bgm
    public void PlayBGM(string res)
    {
        if(isStop == true)
        {
            return;
        }
        //没有当前音频
        if(clips.ContainsKey(res)==false)
        {
            //加载音频
            AudioClip clip = Resources.Load<AudioClip>($"Sounds/{res}"); //$允许直接插值
            clips.Add(res, clip);
        }
        bgmSource.clip = clips[res];
        bgmSource.Play();
    }
    public void PlayEffect(string name, Vector3 pos)
    {
        if(isStop == true)
        {
            return;
        }
        AudioClip clip = null;
        if(clips.ContainsKey(name) == false)
        {
            clip = Resources.Load<AudioClip>($"Sounds/{name}");
            clips.Add(name, clip);
        }
        AudioSource.PlayClipAtPoint(clips[name], pos);
    }
}
