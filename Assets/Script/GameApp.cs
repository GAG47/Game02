using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ͳһ������Ϸ�еĹ��������ڴ�����г�ʼ��
/// </summary>
public class GameApp : Singleton<GameApp>
{
    public static ControllerManager ControllerManager;//������������
    public static ViewManager ViewManager; //��ͼ������
    public static TimerManager TimerManager; //��ʱ��������
    public static SoundManager SoundManager; //����������
    public static ConfigManager ConfigManager; //���ݹ�����
    public static DataManager DataManager; //���ݹ�����
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
