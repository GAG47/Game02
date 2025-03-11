using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//继承momo的脚本 需要挂载游戏脚本 跳转场景后当前脚本物体不移除
public class GameScene : MonoBehaviour
{
    //public Texture2D mouseTxt;//图标图片
    float dt;
    private static bool isLoaded = false;
    private void Awake()
    {
        if(isLoaded == true)
        {
            Destroy(gameObject);
        }
        else
        {
            isLoaded = true;
            DontDestroyOnLoad(gameObject);
            GameApp.Instance.Init();
        }
    }

    void Start()
    {
        //设置鼠标样式
        //Cursor.SetCursor(mouseTxt, Vector2.zero, CursorMode.Auto);

        //注册配置表
        RegisterConfigs();
        GameApp.ConfigManager.LoadAllConfigs();

        //播放背景音乐
        GameApp.SoundManager.PlayBGM("login");

        RegisterModule();
        InitModule();
    }

    //注册控制器
    private void RegisterModule()
    {
        GameApp.ControllerManager.Register(ControllerType.Game, new GameController());
        GameApp.ControllerManager.Register(ControllerType.GameUI, new GameUIController());
        GameApp.ControllerManager.Register(ControllerType.Loading, new LoadingController());
        GameApp.ControllerManager.Register(ControllerType.Mission, new MissionController());
    }

    //执行所有控制器初始化
    private void InitModule()
    {
        GameApp.ControllerManager.InitAllModule();
    }

    //注册配置表
    private void RegisterConfigs()
    {

    }
    void Update()
    {
        dt = Time.deltaTime;
        GameApp.Instance.Update(dt);
    }
}
