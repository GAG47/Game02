using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// ������������
/// </summary>
public class ControllerManager
{
    private Dictionary<int, BaseController> _modules;//�洢���������ֵ�
    public ControllerManager()
    {
        _modules = new Dictionary<int, BaseController>();
    }

    //ע�������
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

    //ִ�����п�����Init����
    public void InitAllModule()
    {
        foreach(var item in _modules)
        {
            item.Value.Init();
        }
    }

    //�Ƴ�������
    public void UnRegister(int controllerKey)
    {
        if (_modules.ContainsKey(controllerKey))
        {
            _modules.Remove(controllerKey);
        }
    }

    //���
    public void Clear()
    {
        _modules.Clear();
    }

    //������п�����
    public void ClearAllModules()
    {
        List<int> keys = _modules.Keys.ToList();
        for (int i = 0; i < keys.Count; ++i)
        {
            _modules[keys[i]].Destroy();
            _modules.Remove(keys[i]);
        }
    }

    //��ģ�崥����Ϣ
    public void ApplyFunc(int controllerKye, string eventName, params object[] args)
    {
        if(_modules.ContainsKey(controllerKye))
        {
            _modules[controllerKye].ApplyFunc(eventName, args);
        }
    }

    //��ȡĳ��������Model����
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