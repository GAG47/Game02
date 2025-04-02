using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionController : BaseController
{
    VehicleEditorController Vec { get; set; }
    public MissionController() : base()
    {
        GameApp.ViewManager.Register(ViewType.EditView, new ViewInfo()
        {
            prefabName = "EditView",
            controller = this,
            parentTf = GameApp.ViewManager.canvasTf
        });
        GameApp.ViewManager.Register(ViewType.PlayView, new ViewInfo()
        {
            prefabName = "PlayView",
            controller = this,
            parentTf = GameApp.ViewManager.canvasTf,
            Sorting_Order = 1
        });
        GameApp.ViewManager.Register(ViewType.PauseView, new ViewInfo()
        {
            prefabName = "PauseView",
            controller = this,
            parentTf = GameApp.ViewManager.canvasTf,
            Sorting_Order = -1
        });
        GameApp.ViewManager.Register(ViewType.LoadVehicleView, new ViewInfo()
        {
            prefabName = "LoadVehicleView",
            controller = this,
            parentTf = GameApp.ViewManager.canvasTf,
            Sorting_Order = 2
        });

        //监听模式改变
        GameApp.DataManager.onModeChange.AddListener(() =>
        {
            switch (GameApp.DataManager.mode)
            {
                case Mode.Edit:
                    Cursor.lockState = CursorLockMode.None;
                    break;
                case Mode.Play:
                    Cursor.lockState = CursorLockMode.Locked;
                    break;
                default:
                    break;
            }
        });

        InitModuleEvent(); //初始化模板事件
        InitGlobalEvent(); //初始化全员事件
    }
    public override void InitModuleEvent()
    {
        RegisterFunc(Defines.OnSaveVehicleBtn, onSaveVehicleBtn);
        RegisterFunc(Defines.OnLoadVehicleBtn, OnLoadVehicleBtn);

        RegisterFunc(Defines.OnVehicleChoose, onVehicleChoose);

        RegisterFunc(Defines.OnBackGameBtn, onBackGameBtn);
        RegisterFunc(Defines.Pause, Pause);

        RegisterFunc(Defines.OnPlayBtn, onPlayBtn);
        RegisterFunc(Defines.OnBlockBtn, onBlockBtn);
        RegisterFunc(Defines.OnEraseBtn, onEraseBtn);
        RegisterFunc(Defines.OnSelectBtn, onSelectGameBtn);
        RegisterFunc(Defines.OnClearBtn, onClearBtn);
    }

    private void onSaveVehicleBtn(System.Object[] arg)
    {
        Text txt = arg[0] as Text;
        if (txt.text != "")
        {
            Vec.vehicleName = txt.text;
            GameApp.DataManager.SaveVehicle(Vec, Vec.vehicleName);
        }
    }
    private void OnLoadVehicleBtn(System.Object[] arg)
    {
        Button button = arg[0] as Button;
        string vehicleName = arg[1] as string;
        button.onClick.AddListener(() =>
        {
            GameApp.DataManager.LoadVehicle(Vec, vehicleName);
        });
    }

    private void onVehicleChoose(System.Object[] arg)
    {
        Vec = arg[0] as VehicleEditorController;
        Vec.onPartSelected.AddListener(() => {
            (GameApp.ViewManager.GetView((int)ViewType.EditView) as EditView).SetProperties(Vec.selectedParts);
        });
    }

    private void onBackGameBtn(System.Object[] arg)
    {
        GameApp.DataManager.mode = Mode.Edit;
        Vec.ResetVehicle();
    }
    public void Pause(System.Object[] arg)
    {
        bool _pause = (bool)arg[0];
        Time.timeScale = _pause? 0.0f : 1.0f;
        if (!_pause)
        {
            GameApp.ViewManager.Close((int)ViewType.PauseView);
        }
        else if (_pause)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (GameApp.DataManager.mode == Mode.Play)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        GameApp.DataManager.isPaused = _pause;
    }

    #region 编辑界面事件
    private void onPlayBtn(System.Object[] arg)
    {
        GameApp.ViewManager.Open(ViewType.PlayView);
        GameApp.DataManager.mode = Mode.Play;
    }
    private void onBlockBtn(System.Object[] arg)
    {
        Vec.currentTool = VehicleEditorController.Tool.Place;
    }
    private void onEraseBtn(System.Object[] arg)
    {
        Vec.currentTool = VehicleEditorController.Tool.Erase;
    }
    private void onSelectGameBtn(System.Object[] arg)
    {
        Vec.currentTool = VehicleEditorController.Tool.Select;
    }
    private void onClearBtn(System.Object[] arg)
    {
        Vec.Clear();
    }
    #endregion
}