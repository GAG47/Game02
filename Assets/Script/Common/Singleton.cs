using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ��������
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T>
{
    private static readonly T instance = Activator.CreateInstance<T>();

    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    //��ʼ��
    public virtual void Init()
    {

    }

    //ÿִ֡��
    public virtual void Update(float deltaTime)
    {

    }

    //�ͷ�
    public virtual void OnDestroy()
    {

    }
}
