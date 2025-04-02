using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// 控制器管理器
/// </summary>
public class ControllerManager
{
    private Dictionary<int, BaseController> _modules;//存储控制器的字典
    public ControllerManager()
    {
        _modules = new Dictionary<int, BaseController>();
    }

    //注册控制器
    public void Register(ControllerType type, BaseController ctl)
    {
        Register((int)type, ctl);
    }
    public void Register(int controllerKey, BaseController ctl)
    {
        if (_modules.ContainsKey(controllerKey) == false)
        {
            _modules.Add(controllerKey, ctl);
        }
    }

    //执行所有控制器Init函数
    public void InitAllModule()
    {
        foreach(var item in _modules)
        {
            item.Value.Init();
        }
    }

    //移除控制器
    public void UnRegister(int controllerKey)
    {
        if (_modules.ContainsKey(controllerKey))
        {
            _modules.Remove(controllerKey);
        }
    }

    //清除
    public void Clear()
    {
        _modules.Clear();
    }

    //清除所有控制器
    public void ClearAllModules()
    {
        List<int> keys = _modules.Keys.ToList();
        for (int i = 0; i < keys.Count; ++i)
        {
            _modules[keys[i]].Destroy();
            _modules.Remove(keys[i]);
        }
    }

    //跨模板触发消息
    public void ApplyFunc(int controllerKye, string eventName, params object[] args)
    {
        if(_modules.ContainsKey(controllerKye))
        {
            _modules[controllerKye].ApplyFunc(eventName, args);
        }
    }

    //获取某控制器的Model对象
    public BaseModel GetControllerModel(int controllerKey)
    {
        if(_modules.ContainsKey(controllerKey))
        {
            return _modules[controllerKey].GetModel();
        }
        else
        {
            return null;
        }
    }
}