using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimerData
{
    private float timer; //计时器
    private System.Action callback; //回调函数
    public GameTimerData(float timer, System.Action callback)
    {
        this.timer = timer;
        this.callback = callback;
    }
    public bool OnUpdate(float dt)
    {
        timer -= dt;
        if(timer <= 0)
        {
            this.callback.Invoke();
            return true;
        }
        return false;
    }
}
