using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��ʼ��Ϸ����
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

    //��ʼ��Ϸ
    private void onStartGameBtn()
    {
        //�رտ�ʼ����
        GameApp.ViewManager.Close(ViewId);
        Debug.Log("��ʼ��Ϸ");
        ApplyFunc(Defines.OpenLevelChooseView);
    }

    //���������
    private void onSetBtn()
    {
        ApplyFunc(Defines.OpenSetView);
    }

    //�˳���Ϸ
    private void onQuitGameBtn()
    {
        Controller.ApplyControllerFunc(ControllerType.GameUI, Defines.OpenMessageView, new MessageInfo()
        {
            okCallBack = delegate ()
            {
                Application.Quit(); //�˳���Ϸ
            },
            MsgTxt = "ȷ���˳���Ϸ��"
        });
    }
}
