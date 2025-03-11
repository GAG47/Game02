    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����һЩ��Ϸͨ��ui�Ŀ�������������� ��ʾ��� ��ʼ��Ϸ���������������ע�ᣩ
/// </summary>
public class GameUIController : BaseController
{
    public GameUIController() : base()
    {
        //��ʼ��Ϸ��ͼ
        GameApp.ViewManager.Register(ViewType.StartView, new ViewInfo()
        {
            prefabName = "StartView",
            controller = this,
            parentTf = GameApp.ViewManager.canvasTf
        });
        GameApp.ViewManager.Register(ViewType.SetView, new ViewInfo()
        {
            prefabName = "SetView",
            controller = this,
            parentTf = GameApp.ViewManager.canvasTf,
            Sorting_Order = 1 //��ס��ʼ���
        });
        GameApp.ViewManager.Register(ViewType.MessageView, new ViewInfo()
        {
            prefabName = "MessageView",
            controller = this,
            Sorting_Order = 999,
            parentTf = GameApp.ViewManager.canvasTf
        });
        GameApp.ViewManager.Register(ViewType.LevelChooseView, new ViewInfo()
        {
            prefabName = "LevelChooseView",
            controller = this,
            Sorting_Order = 1,
            parentTf = GameApp.ViewManager.canvasTf
        });

        InitModuleEvent(); //��ʼ��ģ���¼�
        InitGlobalEvent(); //��ʼ��ȫԱ�¼�
    }
    public override void InitModuleEvent()
    {
        RegisterFunc(Defines.OpenStartView, openStartView);
        RegisterFunc(Defines.OpenSetView, openSetView);
        RegisterFunc(Defines.OpenMessageView, openMessageView);
        RegisterFunc(Defines.OpenLevelChooseView, openLevelChooseView);
        //RegisterFunc(Defines.OpenLevelChooseView, openLevelChooseView);
        // RegisterFunc(Defines);
    }
    //����ģ��ע��ʱ�� ����
    private void openStartView(System.Object[] arg)
    {
        GameApp.ViewManager.Open(ViewType.StartView, arg);
    }
    //���������
    private void openSetView(System.Object[] arg)
    {
        GameApp.ViewManager.Open(ViewType.SetView, arg);
    }

    //����ʾ���
    private void openMessageView(System.Object[] arg)
    {
        GameApp.ViewManager.Open(ViewType.MessageView, arg);
    }

    //�򿪹ؿ�ѡ�����
    private void openLevelChooseView(System.Object[] arg)
    {
        GameApp.ViewManager.Open(ViewType.LevelChooseView, arg);
    }
}
