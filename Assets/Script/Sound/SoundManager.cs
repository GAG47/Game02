using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����������
/// </summary>
public class SoundManager
{
    private AudioSource bgmSource; //����bgm����Ƶ���
    private Dictionary<string, AudioClip> clips; //��Ƶ�����ֵ�

    private bool isStop; //�Ƿ���
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

    private float bgmValume; //bgm������С
    public float BgmValume
    {
        get => bgmValume;
        set
        {
            bgmValume = value;
            bgmSource.volume = bgmValume;
        }
    }

    private float effectVolume; //������С(���� ���˵ȶ���Ч)
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

    //����bgm
    public void PlayBGM(string res)
    {
        if(isStop == true)
        {
            return;
        }
        //û�е�ǰ��Ƶ
        if(clips.ContainsKey(res)==false)
        {
            //������Ƶ
            AudioClip clip = Resources.Load<AudioClip>($"Sounds/{res}"); //$����ֱ�Ӳ�ֵ
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
