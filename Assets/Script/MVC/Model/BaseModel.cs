using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ģ�ͻ���,��Ҫ���ڴ洢����
/// </summary>
public class BaseModel
{
    public BaseController controller;
    public BaseModel(BaseController ctl)
    {
        this.controller = ctl;
    }    
    public BaseModel()
    {
    }
    public virtual void Init()
    {

    }
}
