using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.CanvasScaler;

public class Part : MonoBehaviour
{
    [SerializeField] public bool isRoot = false;
    [SerializeField] public float mass = 10.0f;
    [SerializeField] int health = 10;

    List<PartJoint> m_joints = new List<PartJoint>();
    [HideInInspector] public PartJoint parentJoint { get { return m_rootJoint.connection; } }
    PartJoint m_rootJoint = null; //设定零件旋转和放置 

    [HideInInspector] public int id { get { return transform.GetSiblingIndex(); } }
    [HideInInspector] public int rootJointId { get; private set; } = -1;
    [HideInInspector] public int nbJoint { get { return m_joints.Count; } }
    [HideInInspector] public string prefabName = "Core";

    [HideInInspector] public float angle = 0.0f;

    private bool m_isSelected = false;
    public bool isSelected
    {
        get { return m_isSelected; }
        set
        {
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                mr.material.SetShaderPassEnabled("Always", value);
            }
            m_isSelected = value;
        }
    }

    [HideInInspector] public List<Property> properties = new List<Property>(); //属性

    public UnityEvent onDestroy = new UnityEvent(); //毁坏事件

    public bool unInit = true; //是否初始化
    private Vector3 originPositionOffset; //初始方向
    private Quaternion originRotationOffset; //初始旋转
    public UnityEvent<Transform> onRelativePositionChange = new UnityEvent<Transform>(); //方向改变事件

    protected virtual void Awake()
    {
        //把所有关节加入列表
        PartJoint joint = null;
        for (int i = 0; i < transform.childCount; i++)
        {
            joint = transform.GetChild(i).GetComponent<PartJoint>();
            if (joint != null)
            {
                joint.part = this;
                m_joints.Add(joint);
            }
        }

        //设置根关节（跟关节连接物体损坏则脱离）
        if (!isRoot && m_joints.Count > 0)
        {
            rootJointId = 0;
            m_rootJoint = m_joints[0];
        }
        isSelected = false;
    }

    //设置根部关节（跟关节所在零件毁坏则脱离）
    public void SetRootJoint(int _id)
    {
        if (_id >= 0 && _id < m_joints.Count)
        {
            //PartJoint tmp = m_rootJoint.connection;
            //Detach();
            rootJointId = _id;
            m_rootJoint = m_joints[_id];
        }
    }

    //将当前零件连接到指定的关节
    public void AttachTo(PartJoint _joint, bool dontDisableOther = false)
    {
        if (m_rootJoint != null && _joint != null)
        {
            //链接关节并根据零件的位置和旋转设置零件的位置和旋转
            m_rootJoint.AttachTo(_joint, dontDisableOther);
            Vector3 vOffset = m_rootJoint.transform.localPosition;
            Quaternion qOffset = m_rootJoint.transform.localRotation;
            transform.rotation = m_rootJoint.rotation * Quaternion.Euler(0, 0, angle) * Quaternion.Inverse(qOffset);
            transform.position = m_rootJoint.position - transform.rotation * vOffset;
            if (transform.root != transform)
            {
                transform.root.GetComponent<Rigidbody>().mass += mass;
            }

            //重新设置旋转同步事件监听
            Part parObj = _joint.part;
            if (parObj == this)
            {
                return;
            }
            unInit = true;
            parObj.onRelativePositionChange.AddListener((target) =>
            {
                if(this.unInit)
                {
                    this.unInit = false;
                    Debug.Log("target: "+target.position + " " + transform.position);
                    originPositionOffset = target.InverseTransformPoint(transform.position);
                    originRotationOffset = Quaternion.Inverse(target.rotation) * transform.rotation;
                }
                 Debug.Log("RelativePositionChange " + originPositionOffset + " " + originRotationOffset);


                //位置跟随（考虑目标物体的缩放）
                transform.position = target.TransformPoint(originPositionOffset);
                transform.rotation = target.rotation * originRotationOffset;

                //迭代传递
                onRelativePositionChange.Invoke(transform);
            });
        }
    }

    //将当前零件从其连接的关节上分离
    public void Detach()
    {
        if (m_rootJoint != null)
        {
            m_rootJoint.Detach();
            if (transform.root != transform)
            {
                transform.root.GetComponent<Rigidbody>().mass -= mass;
            }
        }
    }

    //删除当前零件除根关节连接的其他所有连接零件
    public void DeleteAll()
    {
        foreach (PartJoint joint in m_joints)
        {
            if (joint != m_rootJoint && joint.connection != null && joint.connection.part != null)
            {
                joint.connection.part.DestroyRecursively();
            }
        }
    }

    //使零件受到指定量的伤害
    public void GetDamage(int _amount)
    {
        health -= _amount;
        if (health <= 0)
        {
            Destroy();
        }
    }

    //破坏这一部分并将其他部分移动到新的空游戏对象中
    public void Destroy()
    {
        Detach();
        Debris debris = null;
        Part part = null;

        foreach (PartJoint joint in m_joints)
        {
            if (joint != m_rootJoint && joint.connection != null && joint.connection.part != null)
            {
                part = joint.connection.part;
                debris = Instantiate(
                    GameApp.DataManager.prefabDebris,
                    part.transform.position, part.transform.rotation);
                part.SetParentRecursively(debris.transform);

                debris.ComputeCenterOfMass();
                ApplyExplosion(debris.GetComponent<Rigidbody>());
            }
        }

        VehicleEditorController vehicle = transform.root.GetComponent<VehicleEditorController>();
        Debris deb = transform.root.GetComponent<Debris>();
        if (vehicle != null)
        {
            vehicle.ComputeCenterOfMass();
            ApplyExplosion(vehicle.GetComponent<Rigidbody>());
        }
        else if (deb != null)
        {
            deb.ComputeCenterOfMass();
            ApplyExplosion(deb.GetComponent<Rigidbody>());
        }

        Debug.Log("Explode");
        Instantiate(GameApp.DataManager.prefabExplosion, transform.position, Quaternion.identity);

        onDestroy.Invoke();
        Destroy(gameObject);
    }

    //应用爆炸
    public void ApplyExplosion(Rigidbody _rb)
    {
        if (_rb != null)
        {
            _rb.AddExplosionForce(2000.0f, transform.position, 10.0f);
        }
    }

    //设置父对象
    public void SetParentRecursively(Transform _parent)
    {
        transform.SetParent(_parent, true);
        foreach (PartJoint joint in m_joints)
        {
            if (joint != m_rootJoint && joint.connection != null && joint.connection.part != null)
            {
                joint.connection.part.SetParentRecursively(_parent);
            }
        }
    }

    //递归销毁零件
    public void DestroyRecursively()
    {
        Detach();
        foreach (PartJoint joint in m_joints)
        {
            if (joint != m_rootJoint && joint.connection != null && joint.connection.part != null)
            {
                joint.connection.part.DestroyRecursively();
            }
        }
        //DeleteAll();
        Destroy(gameObject);
    }

    //获取关节ID
    public int GetJointID(PartJoint _joint)
    {
        int jointId = -1;
        for (int i = 0; i < m_joints.Count; i++)
        {
            if (m_joints[i] == _joint)
            {
                jointId = i;
                break;
            }
        }
        return jointId;
    }

    //根据ID获取关节
    public PartJoint GetJointFromID(int _id)
    {
        if (_id >= 0 && _id < m_joints.Count)
            return m_joints[_id];
        return null;
    }

    //序列化

    public JSONPart Serialize()
    {
        JSONPart json = new JSONPart
        {
            id = id,
            prefabName = prefabName,
            partTargetId = m_rootJoint != null && parentJoint != null ? parentJoint.part.id : -1,
            rootJoint = rootJointId,
            constraintJoint = m_rootJoint != null && parentJoint != null ? parentJoint.id : -1,
            angle = angle,
            properties = new List<JSONProperty>()
        };

        foreach (Property prop in properties)
        {
            json.properties.Add(prop.Serialize());
        }

        return json;
    }

    //反序列化
    public void Deserialize(JSONPart _json)
    {
        prefabName = _json.prefabName;
        //id = _json.id;
        angle = _json.angle;
        rootJointId = _json.rootJoint;
        m_rootJoint = m_joints[_json.rootJoint];

        if (_json.properties != null)
        {
            for (int i = 0; i < _json.properties.Count; i++)
            {
                properties[i].Deserialize(_json.properties[i]);
            }
        }
    }

}

/// <summary>
/// 零件ID、预制体名称、目标零件ID、根关节ID、约束关节ID、角度和属性列表
/// </summary>
[System.Serializable]
public struct JSONPart
{
    public int id;
    public string prefabName;
    public int partTargetId;
    public int rootJoint;
    public int constraintJoint;
    public float angle;
    public List<JSONProperty> properties;
}
