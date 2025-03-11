using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 设置音量面板
public class SetView : BaseView
{
    protected override void OnAwake()
    {
        base.OnAwake();
        Find<Button>("bg/closeBtn").onClick.AddListener(onCloseBtn);
        Find<Toggle>("bg/IsOpnSound").onValueChanged.AddListener(onIsStopBtn);
        Find<Slider>("bg/soundCount").onValueChanged.AddListener(onSliderBgmBtn);
        Find<Slider>("bg/effectCount").onValueChanged.AddListener(onSliderEffectBtn);

        Find<Toggle>("bg/IsOpnSound").isOn = GameApp.SoundManager.IsStop;
        Find<Slider>("bg/soundCount").value = GameApp.SoundManager.BgmValume;
        Find<Slider>("bg/effectCount").value = GameApp.SoundManager.EffectVolume;
    }

    //是否静音
    private void onIsStopBtn(bool isStop)
    {
        GameApp.SoundManager.IsStop = isStop;
    }

    //bgm音量大小
    private void onSliderBgmBtn(float val)
    {
        GameApp.SoundManager.BgmValume = val;
    }

    private void onSliderEffectBtn(float val)
    {
        GameApp.SoundManager.EffectVolume = val;
    }

    //关闭设置界面
    private void onCloseBtn()
    {
        GameApp.ViewManager.Close(this.ViewId); //关闭自己
    }
}
