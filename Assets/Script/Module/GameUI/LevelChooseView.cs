using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelChooseView : BaseView
{
    protected override void OnAwake()
    {
        base.OnAwake();
        Find<Button>("LevelChoose/Level1Btn").onClick.AddListener(Level1BtnStart);
    }
    private void Level1BtnStart()
    {
        //关闭开始界面
        GameApp.ViewManager.Close(ViewId);

        LoadingModel loadingModel = new LoadingModel();
        loadingModel.SceneName = "edit";
        loadingModel.callback = delegate ()
        {
            GameApp.ViewManager.Open(ViewType.EditView);
            ApplyControllerFunc((int)ControllerType.Mission, Defines.OnVehicleChoose,
                GameObject.Find("Vehicle").GetComponent<VehicleEditorController>());
        };
        Controller.ApplyControllerFunc(ControllerType.Loading, Defines.LoadingScene, loadingModel);
    }
}
