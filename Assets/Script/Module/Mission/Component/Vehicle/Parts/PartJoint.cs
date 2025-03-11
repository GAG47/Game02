using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartJoint : MonoBehaviour
{
    public bool isAttached { get; private set; } = false;

    //连接的关节
    public PartJoint connection { get; private set; } = null;

    //基底位置和旋转
    public Vector3 position { get; private set; } = Vector3.zero;
    public Quaternion rotation { get; private set; } = Quaternion.identity;
    
    //包含该关节的零件
    public Part part = null;

    private SphereCollider m_collider = null;
    private SphereCollider coll
    {
        get
        {
            if (m_collider == null)
            {
                m_collider = GetComponent<SphereCollider>();
            }
            return m_collider;
        }
    }

    public int id
    {
        get
        {
            if (part != null)
            {
                return part.GetJointID(this);
            }
            return -1;
        }
    }

    public void AttachTo(PartJoint _joint, bool dontDisableOther = false)
    {
        Detach();
        if (_joint != null && !_joint.isAttached)
        {
            //该物体链接到其他物体
            isAttached = true;
            connection = _joint;
            coll.enabled = false;

            //其他物体链接到该物体
            connection.isAttached = true;
            if (!dontDisableOther)
            {
                connection.connection = this;
                connection.coll.enabled = false;
            }

            position = connection.transform.position;
            rotation = connection.transform.rotation * Quaternion.Euler(0.0f, 180.0f, 0.0f);
        }
    }

    public void Detach()
    {
        if(connection != null)
        {
            //讲其他物体链接到该物体的链接断开
            connection.isAttached = false;
            connection.coll.enabled = true;
            connection.connection = null;

            //将此物体链接到其他物体的链接断开
            connection = null;
            isAttached = false;
            coll.enabled = true;
        }
    }
}
