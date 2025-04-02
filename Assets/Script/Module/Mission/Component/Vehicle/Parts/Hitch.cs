using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Hitch : Part
{
    Ref<KeyCode> leftKey = new Ref<KeyCode>(KeyCode.None);
    Ref<KeyCode> rightKey = new Ref<KeyCode>(KeyCode.None);

    protected override void Awake()
    {
        base.Awake();
        properties.Add(new Property("向左旋转", leftKey));
        properties.Add(new Property("向右旋转", rightKey));

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
        GameApp.DataManager.onModeChange.AddListener(() =>
        {
            switch (GameApp.DataManager.mode)
            {
                case Mode.Edit:
                    //遍历所有孩子
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        Transform child = transform.GetChild(i);
                        Rigidbody rb = child.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.isKinematic = true;
                        }
                    }
                    break;
                case Mode.Play:
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        Transform child = transform.GetChild(i);
                        Rigidbody rb = child.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.isKinematic = false;
                        }
                    }
                    break;
                default:
                    break;
            }
        });
    }
    protected void Update()
    {
        if (transform.parent == null || transform.root.name != "Vehicle")
            return;

        if (GameApp.DataManager.mode != Mode.Play || isLinkedCore == false)
        {
            return;
        }
    }

    public override void AttachToFixed(PartJoint m_joint, PartJoint _joint, bool anotherAttach = true)
    {
        if (_joint == null || m_joint == null) return;
        //设置连核关节
        if (this.isLinkedCore == false && _joint.part.isLinkedCore)
        {
            SetCoreLinkedJoint(m_joint);
        }
        //增加该物体物理关节
        if (m_joint.canAttach)
        {
            FixedJoint fixedJoint = null;
            if (m_joint.name == "Joint1")
            {
                Transform targetChild = transform.Find("Buttom");
                fixedJoint = targetChild.AddComponent<FixedJoint>();
            }
            else
            {
                Transform targetChild = transform.Find("Top");
                fixedJoint = targetChild.AddComponent<FixedJoint>();
            }
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
        //if (anotherAttach)
        //{
        //    _joint.part.AttachToFixed(_joint, m_joint, false);
        //}
    }
}