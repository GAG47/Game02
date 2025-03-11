using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 开始游戏界面
/// </summary>
public class StartView : BaseView
{
    protected override void OnAwake()
    {
        base.OnAwake();

        Find<Button>("startBtn").onClick.AddListener(onStartGameBtn);
        Find<Button>("setBtn").onClick.AddListener(onSetBtn);
        Find<Button>("quitBtn").onClick.AddListener(onQuitGameBtn);
    }

    //开始游戏
    private void onStartGameBtn()
    {
        //关闭开始界面
        GameApp.ViewManager.Close(ViewId);
        Debug.Log("开始游戏");
        ApplyFunc(Defines.OpenLevelChooseView);
    }

    //打开设置面板
    private void onSetBtn()
    {
        ApplyFunc(Defines.OpenSetView);
    }

    //退出游戏
    private void onQuitGameBtn()
    {
        Controller.ApplyControllerFunc(ControllerType.GameUI, Defines.OpenMessageView, new MessageInfo()
        {
            okCallBack = delegate ()
            {
                Application.Quit(); //退出游戏
            },
            MsgTxt = "确定退出游戏吗？"
        });
    }
}
