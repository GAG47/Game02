using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public enum Mode
{
    Edit,
    Play
}

//任务类型
public enum MissionType
{
    None,
}

//零件种类
public enum PartType
{
    Cube,
    Slope,
    Thruster,
    Hitch,
    Wheel
}

/// <summary>
/// 数据管理器，用于存储载具、零件的基本数据
/// </summary>
public class DataManager
{
    private Mode m_mode = Mode.Edit;
    public Mode mode
    {
        get
        {
            return m_mode;
        }
        set
        {
            if (value != m_mode)
            {
                m_mode = value;
                onModeChange.Invoke();
            }
        }
    }

    [SerializeField] public List<Part> prefabParts;
    [SerializeField] public GameObject prefabExplosion;
    [SerializeField] public UnityEvent onModeChange = new UnityEvent();
    [SerializeField] public UnityEvent onPartChange = new UnityEvent();
    JsonSerializer serializer = new JsonSerializer();

    private int m_currentPartIndex = -1;
    public int currentPartIndex
    {
        get { return m_currentPartIndex; }
        set
        {
            if (m_currentPartIndex != value)
            {
                m_currentPartIndex = value;
                onPartChange.Invoke();
            }
        }
    }

    public MissionType missionType = MissionType.None;
    public bool missionEnded = false;
    public float missionTimer = -1.0f;
    public bool isPaused = false;

    public DataManager()
    {
        //载入共享资产
        AssetBundle shared = AssetBundleManager.LoadBundle("shared");
        shared.LoadAllAssets();

        //载入零件
        //todo: 将所有写好的零件打包然后从包加载
        prefabParts = new List<Part>();
        prefabParts.Add(Resources.Load<GameObject>("Model/Vehicle/Cube/Cube").GetComponent<Part>());
        prefabParts.Add(Resources.Load<GameObject>("Model/Vehicle/Slope/Slope").GetComponent<Part>());
        prefabParts.Add(Resources.Load<GameObject>("Model/Vehicle/Thruster/Thruster").GetComponent<Part>());
        prefabParts.Add(Resources.Load<GameObject>("Model/Vehicle/Hitch/Hitch").GetComponent<Part>());
        prefabParts.Add(Resources.Load<GameObject>("Model/Vehicle/Wheel/Wheel").GetComponent<Part>());
        //for (int i = 0; i < prefabParts.Count; i++)
        //{
        //    Debug.Log(prefabParts[i].name);
        //}
        //Debug.Log(prefabParts.Count + " parts loaded");
    }

    public string GetCurrentPartName()
    {
        if (m_currentPartIndex >= 0 && m_currentPartIndex < prefabParts.Count)
            return prefabParts[m_currentPartIndex].name;
        else
            return "None";
    }

    public Part GetPrefabFromName(string _name)
    {
        Part prefab = null;
        foreach (Part part in prefabParts)
        {
            if (part.name == _name)
            {
                prefab = part;
            }
        }
        return prefab;
    }

    public void LoadPartBundles()
    {
        prefabParts.Clear();

        //载入所有物体
        List<BundleFile> bundles = AssetBundleManager.ListBundles("Parts");
        foreach (BundleFile file in bundles)
        {
            Debug.Log("Bundle: " + file.name);
            AssetBundle bundle = AssetBundleManager.LoadBundle(file);
            Object[] assets = bundle.LoadAllAssets();
            foreach (Object asset in assets)
            {
                GameObject obj = asset as GameObject;
                if (obj != null)
                {
                    Part part = obj.GetComponent<Part>();
                    if (part != null)
                    {
                        prefabParts.Add(part);
                    }
                }
            }
        }
    }

    public void SaveVehicle(VehicleEditorController _vehicle, string _filename)
    {
        string path = Path.Combine(DirectoryManager.GetDirectory("Vehicles").FullName, _filename + ".vehicle");

        using (FileStream file = new FileStream(path, File.Exists(path) ? FileMode.Truncate : FileMode.Create))
        using (StreamWriter writer = new StreamWriter(file))
        //using (JsonWriter json = new JsonTextWriter(writer))
        {
            //json.Formatting = Formatting.Indented;
            //serializer.Serialize(json, _vehicle.Serialize());
            writer.Write(JsonUtility.ToJson(_vehicle.Serialize(), true));
        }
    }

    public void LoadVehicle(VehicleEditorController _vehicle, string _filename)
    {
        string path = Path.Combine(DirectoryManager.GetDirectory("Vehicles").FullName, _filename + ".vehicle");
        if (!File.Exists(path))
        {
            Debug.Log("Vehicle not found");
            return;
        }
        using (FileStream file = new FileStream(path, FileMode.Open))
        using (StreamReader reader = new StreamReader(file))
        //using (JsonReader json = new JsonTextReader(reader))
        {
            //_vehicle.Deserialize(serializer.Deserialize<JSONVehicle>(json));
            _vehicle.Deserialize(JsonUtility.FromJson<JSONVehicle>(reader.ReadToEnd()));
        }
    }
}