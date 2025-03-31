using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Part : MonoBehaviour
{
    //基本信息
    [HideInInspector] public string prefabName = "Core";
    [HideInInspector] public float angle = 0.0f;
    [HideInInspector] public Vector3 position = Vector3.zero;
    [HideInInspector] public Quaternion rotaion = Quaternion.identity;
    [SerializeField] public bool isRoot = false;
    public bool isLinkedCore = false; //是否连接核心
    [SerializeField] public float mass = 10.0f;
    [SerializeField] int health = 10;

    //关节相关
    protected List<PartJoint> m_joints = new List<PartJoint>();
    [HideInInspector] public PartJoint parentJoint { get { return m_rootJoint.connection; } } //跟关节连接的关节
    protected PartJoint m_rootJoint = null; //根关节,作为放置的时候和物体连接的关节
    protected PartJoint m_coreLinkedJoint = null; //连接核心的关节
    [HideInInspector] public int id { get { return transform.GetSiblingIndex(); } }
    [HideInInspector] public int rootJointId { get; private set; } = -1;
    [HideInInspector] public int coreLinkedId { get; private set; } = -1;
    [HideInInspector] public int nbJoint { get { return m_joints.Count; } }

    protected Rigidbody m_rigidbody = null;


    //是否被选择
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

    //旋转事件相关(已弃用)
    //[HideInInspector] public bool unInit = true; //是否初始化
    private Vector3 m_originPosition; //初始位置
    private Quaternion m_originRotation; //初始方向
    //public UnityEvent<Transform> onRelativePositionChange = new UnityEvent<Transform>(); //方向改变事件

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

        //设置根关节（跟关节连接物体损坏则脱离（已弃用））
        //根关节作为放置的时候和物体连接的关节
        if (!isRoot && m_joints.Count > 0)
        {
            rootJointId = 0;
            m_rootJoint = m_joints[0];
        }
        isSelected = false;

        //初始化刚体
        if (m_rigidbody == null)
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_rigidbody.isKinematic = true;
            m_rigidbody.mass = mass;
        }

        GameApp.DataManager.onModeChange.AddListener(() =>
        {
            switch (GameApp.DataManager.mode)
            {
                case Mode.Edit:
                    m_rigidbody.isKinematic = true;
                    break;
                case Mode.Play:
                    m_rigidbody.isKinematic = false;
                    break;
                default:
                    break;
            }
        });
    }

    //设置根部关节
    public void SetRootJoint(int _id)
    {
        if (_id >= 0 && _id < m_joints.Count)
        {
            rootJointId = _id;
            m_rootJoint = m_joints[_id];
        }
    }

    //重新查找连核关节
    public bool RefindCoreLinkedJointRecursively()
    {
        ClearCoreLinkedJoint();
        for (int i=0;i< m_joints.Count; i++)
        {
            if (m_joints[i].connection != null && m_joints[i].connection.part.isLinkedCore
                && m_joints[i].connection.part.RefindCoreLinkedJointRecursively())
            {
                SetCoreLinkedJoint(m_joints[i]);
                //核心关节连接损坏则删除
                m_joints[i].connection.part.onDestroy.AddListener(() =>
                {
                    RefindCoreLinkedJointRecursively();
                });
                return true;
            }
        }
        return false;
    }
    //清除核心关节
    public void ClearCoreLinkedJoint()
    {
        m_coreLinkedJoint = null;
        coreLinkedId = -1;
        isLinkedCore = false;
    }
    //设置核心关节
    public void SetCoreLinkedJoint(PartJoint _joint)
    {
        m_coreLinkedJoint = _joint;
        coreLinkedId = GetJointID(_joint);
        isLinkedCore = true;
    }
    //递归设置核心关节
    public void SetCoreLinkedJointRecursively(PartJoint _joint)
    {
        SetCoreLinkedJoint(_joint);
        for(int i = 0; i < m_joints.Count; i++)
        {
            if (m_joints[i].connection != null && !m_joints[i].connection.part.isLinkedCore)
            {
                m_joints[i].connection.part.SetCoreLinkedJointRecursively(m_joints[i].connection);
            }
        }
    }

    //将当前零件连接到指定的关节(通过根关节连接)
    public virtual void AttachTo(PartJoint _joint, bool dontDisableOther = false)
    {
        if (m_rootJoint != null && _joint != null && _joint != m_rootJoint)
        {
            //链接关节并根据零件的位置和旋转设置零件的位置和旋转
            m_rootJoint.AttachTo(_joint, dontDisableOther);
            Vector3 vOffset = m_rootJoint.transform.localPosition;
            Quaternion qOffset = m_rootJoint.transform.localRotation;
            transform.rotation = m_rootJoint.rotation * Quaternion.Euler(0, 0, angle) * Quaternion.Inverse(qOffset);
            transform.position = m_rootJoint.position - transform.rotation * vOffset;

            //to do:浮点数优化？（也许不用）
            position = transform.position;
            rotaion = transform.rotation;

            //物理连接和核心关节判断
            if (!dontDisableOther)
            {
                AttachToFixed(m_rootJoint, _joint);
                for (int i = 0; i < m_joints.Count; i++)
                {
                    if (m_joints[i] != m_rootJoint)
                    {
                        AttachToFixed(m_joints[i], m_joints[i].CheckNearJoint());
                    }
                }
                if (isLinkedCore)
                {
                    for (int i = 0; i < m_joints.Count; i++)
                    {
                        if (m_joints[i].connection != null)
                        {
                            m_joints[i].connection.part.SetCoreLinkedJointRecursively(m_joints[i].connection);
                        }
                    }
                }
            }

            #region 旋转同步（已弃用）
            ////设置旋转同步事件监听
            //Part parObj = _joint.part;
            //if (parObj == this)
            //{
            //    return;
            //}
            //unInit = true;
            //parObj.onRelativePositionChange.AddListener((target) =>
            //{
            //    if(this.unInit)
            //    {
            //        this.unInit = false;
            //        Debug.Log("target: "+target.position + " " + transform.position);
            //        originPositionOffset = target.InverseTransformPoint(transform.position);
            //        originRotationOffset = Quaternion.Inverse(target.rotation) * transform.rotation;
            //    }
            //     Debug.Log("RelativePositionChange " + originPositionOffset + " " + originRotationOffset);

            //    //位置跟随（考虑目标物体的缩放）
            //    transform.position = target.TransformPoint(originPositionOffset);
            //    transform.rotation = target.rotation * originRotationOffset;

            //    //迭代传递
            //    onRelativePositionChange.Invoke(transform);
            //});
            #endregion
        }
    }
    public void AttachEveryJoint()
    {
        for (int i = 0; i < m_joints.Count; i++)
        {
            AttachToFixed(m_joints[i], m_joints[i].CheckNearJoint());
        }
        if (isLinkedCore)
        {
            for (int i = 0; i < m_joints.Count; i++)
            {
                if (m_joints[i].connection != null)
                {
                    m_joints[i].connection.part.SetCoreLinkedJointRecursively(m_joints[i].connection);
                }
            }
        }
    }

    //如果子类对应的物体的连接关节需要修改，则重写该函数
    protected virtual void AttachToFixed(PartJoint m_joint, PartJoint _joint)
    {
        if (_joint == null || m_joint == null) return;
        //设置连核关节
        if(this.isLinkedCore == false && _joint.part.isLinkedCore)
        {
            SetCoreLinkedJoint(m_joint);
        }
        //增加该物体物理关节
        if(m_joint.canAttach)
        {
            FixedJoint fixedJoint = this.transform.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = _joint.part.GetComponent<Rigidbody>();
            fixedJoint.connectedMassScale = 5.0f;
            fixedJoint.breakForce = 50000.0f;
            fixedJoint.breakTorque = 50000.0f;
            _joint.part.onDestroy.AddListener(() =>
            {
                Destroy(fixedJoint);
            });
            if (m_coreLinkedJoint == m_joint)
            {
                _joint.part.onDestroy.AddListener(() =>
                {
                    RefindCoreLinkedJointRecursively(); //重新查找核心关节
                });
            }
        }
        //增加其他物体物理关节
        if (_joint.canAttach)
        {
            FixedJoint fixedJoint2 = _joint.part.transform.AddComponent<FixedJoint>();
            fixedJoint2.connectedBody = this.GetComponent<Rigidbody>();
            fixedJoint2.connectedMassScale = 5.0f;
            fixedJoint2.breakForce = 50000.0f;
            fixedJoint2.breakTorque = 50000.0f;
            //Debug.Log("fixedJoint2 Anchor: " +  fixedJoint2.anchor);
            //Debug.Log("fixedJoint2 connectedAnchor: " +  fixedJoint2.connectedAnchor);
            if (_joint.part.isLinkedCore)
            {
                _joint.part.SetCoreLinkedJointRecursively(_joint);
            }
            this.onDestroy.AddListener(() =>
            {
                Destroy(fixedJoint2);
            });
        }
    }

    //将当前零件从其连接的根关节上分离
    public virtual void Detach()
    {
        if (m_rootJoint != null)
        {
            m_rootJoint.Detach();
        }
    }

    //删除当前零件除根关节连接的其他所有连接零件
    public void DeleteAllJointConnection()
    {
        foreach (PartJoint joint in m_joints)
        {
            if (joint != m_rootJoint && joint.connection != null && joint.connection.part != null)
            {
                joint.connection.part.Destroy();
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

    //破坏这一部分并解除对应的物理关节
    public void Destroy()
    {
        Detach();
        Part part = null;
        foreach (PartJoint joint in m_joints)
        {
            if (joint != m_rootJoint && joint.connection != null && joint.connection.part != null)
            {
                part = joint.connection.part;
            }
        }
        onDestroy.Invoke();
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
            position = position,
            rotation = rotaion,
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
        position = _json.position;
        rotaion = _json.rotation;

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
    public Vector3 position;
    public Quaternion rotation;
    public List<JSONProperty> properties;
}