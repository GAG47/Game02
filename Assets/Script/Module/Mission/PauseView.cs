using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseView : BaseView
{
    protected override void OnAwake()
    {
        base.OnAwake();

        Find<Button>("bg/BackGameBtn").onClick.AddListener(OnBackGameBtn);
        Find<Button>("bg/BackEditBtn").onClick.AddListener(OnBackEditBtn);
        Find<Button>("bg/BackMenuBtn").onClick.AddListener(OnBackMenuBtn);
    }

    private void OnBackGameBtn()
    {
        GameApp.ViewManager.Close(ViewId);
        ApplyFunc(Defines.Pause, false);
    }
    private void OnBackEditBtn()
    {
        GameApp.ViewManager.Close(ViewId);
        GameApp.ViewManager.Close(ViewType.PlayView);
        GameApp.ViewManager.Open(ViewType.EditView);
        ApplyFunc(Defines.OnBackGameBtn);
        ApplyFunc(Defines.Pause, false);
    }
    private void OnBackMenuBtn()
    {
        //关闭所有界面
        GameApp.ViewManager.CloseAll();
        GameApp.DataManager.mode = Mode.Edit;
        ApplyFunc(Defines.Pause, false);

        LoadingModel loadingModel = new LoadingModel();
        loadingModel.SceneName = "game";
        loadingModel.callback = delegate ()
        {
            GameApp.ViewManager.Open(ViewType.StartView);
        };
        Controller.ApplyControllerFunc(ControllerType.Loading, Defines.LoadingScene, loadingModel);
    }
}