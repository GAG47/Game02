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
        properties.Add(new Property("������ת", leftKey));
        properties.Add(new Property("������ת", rightKey));

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
                    //�������к���
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
        //�������˹ؽ�
        if (this.isLinkedCore == false && _joint.part.isLinkedCore)
        {
            SetCoreLinkedJoint(m_joint);
        }
        //���Ӹ���������ؽ�
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
                    RefindCoreLinkedJointRecursively(); //���²��Һ��Ĺؽ�
                });
            }
        }
        //����������������ؽ�
        //if (anotherAttach)
        //{
        //    _joint.part.AttachToFixed(_joint, m_joint, false);
        //}
    }
}