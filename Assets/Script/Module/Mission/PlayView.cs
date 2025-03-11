using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayView : BaseView
{
    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameApp.ViewManager.Open(ViewType.PauseView);
            ApplyFunc(Defines.Pause, !GameApp.DataManager.isPaused);
        }
    }
}