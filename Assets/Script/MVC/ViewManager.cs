using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// 视图信息类
/// </summary>
public class ViewInfo
{
    public string prefabName; //视图预制体名称
    public Transform parentTf; //所在父级
    public BaseController controller; //试图所属控制器
    public int Sorting_Order; //显示层级 改变显示顺序
}

/// <summary>
/// 视图管理器
/// </summary>
public class ViewManager
{
    public Transform canvasTf; //画布组件
    public Transform worldCanvasTf; //世界画布组件
    Dictionary<int, IBaseView> _opens; //开启中的视图
    Dictionary<int, IBaseView> _viewCache; //视图缓存
    Dictionary<int, ViewInfo> _views; //注册的试图信息
    public ViewManager()
    {
        canvasTf = GameObject.Find("Canvas").transform;
        worldCanvasTf = GameObject.Find("WorldCanvas").transform;
        _opens = new Dictionary<int, IBaseView>();
        _viewCache = new Dictionary<int, IBaseView>();
        _views = new Dictionary<int, ViewInfo>();
    }
    
    //注册视图信息
    public void Register(int key,ViewInfo viewInfo)
    {
        if (_views.ContainsKey(key) == false)
        {
            _views.Add(key, viewInfo);
        }
    }
    public void Register(ViewType viewType,ViewInfo viewInfo)
    {
        Register((int)viewType, viewInfo);
    }
    
    //移除视图信息
    public void UnRegister(int key)
    {
        if(_views.ContainsKey(key))
        {
            _views.Remove(key);
        }
    }

    //移除面板
    public void RemoveView(int key)
    {
        _views.Remove(key);
        _viewCache.Remove(key);
        _opens.Remove(key);
    }

    //移除控制器的面板视图
    public void RemoveViewByController(BaseController ctl)
    {
        foreach(var item in _views)
        {
            if(item.Value.controller==ctl)
            {
                RemoveView(item.Key);
            }
        }
    }

    //是否开启中
    public bool IsOpen(int key)
    {
        return _opens.ContainsKey(key);
    }

    //获得某个视图
    public IBaseView GetView(int key)
    {
        if(_opens.ContainsKey(key))
        {
            return _opens[key];
        }
        if(_viewCache.ContainsKey(key))
        {
            return _viewCache[key];
        }
        return null;
    }

    public T GetView<T>(int key) where T:class,IBaseView
    {
        IBaseView view = GetView(key);
        if(view!=null)
        {
            return view as T;
        }
        return null;
    }

    //销毁视图
    public void Destroy(int key)
    {
        IBaseView oldView = GetView(key);
        if (oldView != null)
        {
            UnRegister(key);
            oldView.DestroyView();
            _viewCache.Remove(key);
        }
    }

    //关闭面板视图
    public void Close(ViewType key, params object[] args)
    {
        Close((int)key, args);
    }
    public void Close(int key,params object[] args)
    {
        //没有打开
        if(IsOpen(key)==false)
        {
            return;
        }
        IBaseView view = GetView(key);
        if(view != null)
        {
            _opens.Remove(key);
            view.Close(args);
            _views[key].controller.CloseView(view);
        }
    }
    public void CloseAll()
    {
        List<IBaseView> list = _opens.Values.ToList();
        for(int i = list.Count - 1;i>=0;--i)
        {
            Close(list[i].ViewId);
        }
    }

    //打开某个视图的面板
    public void Open(ViewType viewType, params object[] args)
    {
        Open((int)viewType, args);
    }

    //打开某个视图面板
    public void Open(int key,params object[] args)
    {
        IBaseView view = GetView(key);
        ViewInfo viewInfo = _views[key];
        if(view==null) //不存在的视图 进行资源加载
        {
            string type = ((ViewType)key).ToString(); //类型字符串和脚本名称对应
            GameObject uiObj = UnityEngine.Object.Instantiate(Resources.Load($"View/{viewInfo.prefabName}"),viewInfo.parentTf) as GameObject;
            Canvas canvas = uiObj.GetComponent<Canvas>();
            if(canvas==null)
            {
                canvas = uiObj.AddComponent<Canvas>();
            }
            if(uiObj.GetComponent<GraphicRaycaster>()==null)
            {
                uiObj.AddComponent<GraphicRaycaster>();
            }
            canvas.overrideSorting = true;//可以设置层级
            canvas.sortingOrder = viewInfo.Sorting_Order; //设置层级
            view = uiObj.AddComponent(Type.GetType(type)) as IBaseView;
            view.ViewId = key; //视图id
            view.Controller = viewInfo.controller; //设置控制器
            //添加到视图缓存
            _viewCache.Add(key, view);
            viewInfo.controller.OnLoadView(view);
        }
        //已经打开则返回
        if(this._opens.ContainsKey(key)==true)
        {
            return;
        }
        this._opens.Add(key, view);
        
        //已经初始化过 
        if(view.IsInit())
        {
            view.SetVisible(true); //显示
            view.Open(args);  //打开
            viewInfo.controller.OpenView(view);
        }
        else
        {
            view.InitUI();
            view.InitData();
            view.Open(args);
            viewInfo.controller.OpenView(view);
        }
    }
    
}
