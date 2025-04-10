using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 统一定义游戏中的管理器，在此类进行初始化
/// </summary>
public class GameApp : Singleton<GameApp>
{
    public static ControllerManager ControllerManager;//控制器管理器
    public static ViewManager ViewManager; //视图管理器
    public static TimerManager TimerManager; //计时器管理器
    public static SoundManager SoundManager; //声音管理器
    public static ConfigManager ConfigManager; //数据管理器
    public static DataManager DataManager; //数据管理器
    public override void Init()
    {
        ControllerManager = new ControllerManager();
        ViewManager = new ViewManager();
        TimerManager = new TimerManager();
        SoundManager = new SoundManager();
        ConfigManager = new ConfigManager();
        DataManager = new DataManager();
    }

    public override void Update(float deltaTime)
    {
        TimerManager.OnUpdate(deltaTime);
    }
}
