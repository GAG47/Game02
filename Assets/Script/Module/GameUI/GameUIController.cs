    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 处理一些游戏通用ui的控制器（设置面板 提示面板 开始游戏面板等在这个控制器注册）
/// </summary>
public class GameUIController : BaseController
{
    public GameUIController() : base()
    {
        //开始游戏视图
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
            Sorting_Order = 1 //挡住开始面板
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

        InitModuleEvent(); //初始化模板事件
        InitGlobalEvent(); //初始化全员事件
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
    //测试模板注册时间 例子
    private void openStartView(System.Object[] arg)
    {
        GameApp.ViewManager.Open(ViewType.StartView, arg);
    }
    //打开设置面板
    private void openSetView(System.Object[] arg)
    {
        GameApp.ViewManager.Open(ViewType.SetView, arg);
    }

    //打开提示面板
    private void openMessageView(System.Object[] arg)
    {
        GameApp.ViewManager.Open(ViewType.MessageView, arg);
    }

    //打开关卡选择面板
    private void openLevelChooseView(System.Object[] arg)
    {
        GameApp.ViewManager.Open(ViewType.LevelChooseView, arg);
    }
}
