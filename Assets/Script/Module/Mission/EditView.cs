using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using UnityEngine.Events;

public class EditView : BaseView
{
    private bool LoadVisible = true;
    Transform content;
    GameObject prefabButton;
    Text tutorial;

    protected override void OnAwake()
    {
        base.OnAwake();

        UpdateLoadVisible();
        content = Find<Transform>("LoadVehicle/Scroll View/Viewport/Content");
        prefabButton = Resources.Load("View/VehicleBtn") as GameObject;
        tutorial = Find<Text>("Tutorial");

        UpdateToturial();

        Find<Button>("PlayBtn").onClick.AddListener(OnPlayBtn);

        Find<Button>("Tool/BlockBtn").onClick.AddListener(OnBlockBtn);
        Find<Button>("Tool/EraseBtn").onClick.AddListener(OnEraseBtn);
        Find<Button>("Tool/SelectBtn").onClick.AddListener(OnSelectGameBtn);
        Find<Button>("Tool/ClearBtn").onClick.AddListener(OnClearBtn);
        Find<Button>("Save/SaveBtn").onClick.AddListener(OnSaveVehicleBtn);
        Find<Button>("Save/LoadBtn").onClick.AddListener(OnLoadVehicleBtn);
        Find<Button>("LoadVehicle/bg").onClick.AddListener(OnLoadVehicleBtn);

        Find<Button>("Part/Viewport/Content/CubeBtn").onClick.AddListener(() => { 
            GameApp.DataManager.currentPartIndex = 0;
            UpdateToturial();
        });
        Find<Button>("Part/Viewport/Content/SlopeBtn").onClick.AddListener(() => {
            GameApp.DataManager.currentPartIndex = 1;
            UpdateToturial();
        });
        Find<Button>("Part/Viewport/Content/ThrusterBtn").onClick.AddListener(() => {
            GameApp.DataManager.currentPartIndex = 2;
            UpdateToturial();
        });
        Find<Button>("Part/Viewport/Content/HitchBtn").onClick.AddListener(() => {
            GameApp.DataManager.currentPartIndex = 3;
            UpdateToturial();
        });
        Find<Button>("Part/Viewport/Content/WheelBtn").onClick.AddListener(() => {
            GameApp.DataManager.currentPartIndex = 4;
            UpdateToturial();
        });
    }

    private void UpdateToturial()
    {
        tutorial.text = "R：旋转物体\n" +
            "F：改变连接面\n" +
            "当前选择零件: " + GameApp.DataManager.GetCurrentPartName() + "\n";
    }

    private void OnSaveVehicleBtn()
    {
        ApplyFunc(Defines.OnSaveVehicleBtn, Find<Text>("Save/InputName/Text"));
    }
    private void OnLoadVehicleBtn()
    {
        RefreshLoadView();
        UpdateLoadVisible();
    }
    private void UpdateLoadVisible()
    {
        LoadVisible = !LoadVisible;
        Find<Transform>("LoadVehicle").gameObject.SetActive(LoadVisible);
    }
    private void RefreshLoadView()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        DirectoryManager.GetDirectory("Vehicles").GetFiles()
            .Where((FileInfo file) => file.Name.EndsWith(".vehicle")).ToList()
            .ForEach((FileInfo file) =>
            {
                Button button = Instantiate(prefabButton, content).GetComponent<Button>();
                string vehicleName = file.Name.Substring(0, file.Name.IndexOf('.'));
                Debug.Log(button.gameObject.name);
                button.GetComponentInChildren<Text>().text = vehicleName;

                ApplyFunc(Defines.OnLoadVehicleBtn, button, vehicleName);
            });
    }

    private void OnPlayBtn()
    {
        GameApp.ViewManager.Close(ViewId);
        Debug.Log("任务开始");
        ApplyFunc(Defines.OnPlayBtn);
    }

    private void OnBlockBtn()
    {
        ApplyFunc(Defines.OnBlockBtn);
    }
    private void OnEraseBtn()
    {
        ApplyFunc(Defines.OnEraseBtn);
    }
    private void OnSelectGameBtn()
    {
        ApplyFunc(Defines.OnSelectBtn);
    }
    private void OnClearBtn()
    {
        ApplyFunc(Defines.OnClearBtn);
    }

    public void SetProperties(List<Part> _parts)
    {
        Transform container = Find<Transform>("Properties");
        for (int i = 1; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }

        if (_parts.Count > 1)
        {
            Debug.Log("选择过多");
        }
        else if (_parts.Count <= 0)
        {
            Debug.Log("未选择");
        }
        else 
        {
            foreach (Property prop in _parts[0].properties)
            {
                UIProperty obj = null;
                switch (prop.type)
                {
                    case Property.Type.Bool:
                        UIProperty boolPrefab = Resources.Load<GameObject>("Icon/BoolProperty").GetComponent<UIProperty>();
                        obj = Instantiate(boolPrefab.gameObject, container).GetComponent<UIProperty>();
                        break;
                    case Property.Type.Float:
                        UIProperty floatPrefab = Resources.Load<GameObject>("Icon/FloatProperty").GetComponent<UIProperty>();
                        obj = Instantiate(floatPrefab.gameObject, container).GetComponent<UIProperty>();
                        break;
                    case Property.Type.Key:
                        UIProperty keyPrefab = Resources.Load<GameObject>("Icon/KeyProperty").GetComponent<UIProperty>();
                        obj = Instantiate(keyPrefab.gameObject, container).GetComponent<UIProperty>();
                        break;
                    default: break;
                }
                if (obj != null)
                {
                    obj.property = prop;
                }
            }
        }
    }
    public void ClearSetProperties()
    {
        Transform container = Find<Transform>("Properties");
        for (int i = 1; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }
    }
}