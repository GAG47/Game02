using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�̳�momo�Ľű� ��Ҫ������Ϸ�ű� ��ת������ǰ�ű����岻�Ƴ�
public class GameScene : MonoBehaviour
{
    //public Texture2D mouseTxt;//ͼ��ͼƬ
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
        //���������ʽ
        //Cursor.SetCursor(mouseTxt, Vector2.zero, CursorMode.Auto);

        //ע�����ñ�
        RegisterConfigs();
        GameApp.ConfigManager.LoadAllConfigs();

        //���ű�������
        GameApp.SoundManager.PlayBGM("login");

        RegisterModule();
        InitModule();
    }

    //ע�������
    private void RegisterModule()
    {
        GameApp.ControllerManager.Register(ControllerType.Game, new GameController());
        GameApp.ControllerManager.Register(ControllerType.GameUI, new GameUIController());
        GameApp.ControllerManager.Register(ControllerType.Loading, new LoadingController());
        GameApp.ControllerManager.Register(ControllerType.Mission, new MissionController());
    }

    //ִ�����п�������ʼ��
    private void InitModule()
    {
        GameApp.ControllerManager.InitAllModule();
    }

    //ע�����ñ�
    private void RegisterConfigs()
    {

    }
    void Update()
    {
        dt = Time.deltaTime;
        GameApp.Instance.Update(dt);
    }
}
