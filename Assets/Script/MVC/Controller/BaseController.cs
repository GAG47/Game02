using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  控制器基类,主要用于管理各类函数以及委托,用于对用户的指定操作做出反应
/// </summary>
public class BaseController
{
    private Dictionary<string, System.Action<object[]>> message;//事件字典
    protected BaseModel model;
    public BaseController()
    {
        message = new Dictionary<string, System.Action<object[]>>();
    }

    //注册后通用的初始化函数（要所有控制器初始化后执行）
    public virtual void Init()
    {
        
    }
    public virtual void OnLoadView(IBaseView view) { } //加载视图
    public virtual void OpenView(IBaseView view) { }  //打开视图
    public virtual void CloseView(IBaseView view) { }  //关闭视图

    //注册模板事件
    public void RegisterFunc(string eventName,System.Action<object[]> callback)
    {
        if(message.ContainsKey(eventName))
        {
            message[eventName] += callback;
        }
        else
        {
            message.Add(eventName, callback);
        }
    }

    //移除模板事件
    public void UnRegiterFunc(string eventName)
    {
        if(message.ContainsKey(eventName))
        {
            message.Remove(eventName);
        }
    }

    //触发本模板事件
    public void ApplyFunc(string eventName,params object[] args)
    {
        //Debug.Log("ApplyFunc:" + eventName + " invoke!");
        if (message.ContainsKey(eventName))
        {
            message[eventName].Invoke(args);
        }
        else
        {
            Debug.Log("error:" + eventName);
        }
    }
    
    //触发其他模板的事件
    public void ApplyControllerFunc(int controllerKey,string eventName,params object[] args)
    {
        GameApp.ControllerManager.ApplyFunc(controllerKey, eventName, args);
    }
    public void ApplyControllerFunc(ControllerType type,string eventName,params object[] args)
    {
        ApplyControllerFunc((int)type, eventName, args);
    }
     
    //设置模型数据
    public void SetModel(BaseModel model)
    {
        this.model = model;
        this.model.controller = this;
    }
    public BaseModel GetModel()
    {
        return model;
    }
    public T GetModel<T>() where T : BaseModel
    {
        return model as T;
    }
    public BaseModel GetControllerModel(int controllerKey)
    {
        return GameApp.ControllerManager.GetControllerModel(controllerKey);
    }

    public virtual void Destroy()
    {
        RemoveModuleEvent();
        RemoveGlobalEvent();
    }
    
    //初始化模板事件
    public virtual void InitModuleEvent()
    {

    }

    //移除模板事件
    public virtual void RemoveModuleEvent()
    {

    }

    //初始化全局事,全局事件需要通过额外的管理器管理
    public virtual void InitGlobalEvent()
    {

    }

    //移除全局事件
    public virtual void RemoveGlobalEvent()
    { 
    }
}