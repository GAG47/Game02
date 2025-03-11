using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Wheel : Part
{
    [SerializeField] float power = 50.0f;
    [SerializeField] float rotationSpeed = 50.0f;
    private Rigidbody rb;
    private Rigidbody wheelRb;
    private FixedJoint fixedJoint;

    Ref<KeyCode> forwardKey = new Ref<KeyCode>(KeyCode.None);
    Ref<KeyCode> backKey = new Ref<KeyCode>(KeyCode.None);

    private bool isGrounded = false;

    protected override void Awake()
    {
        base.Awake();
        properties.Add(new Property("前进", forwardKey));
        properties.Add(new Property("后退", backKey));
        wheelRb = GetComponentInChildren<Rigidbody>();
        fixedJoint = GetComponentInChildren<FixedJoint>();
    }

    void Update()
    {
        if (transform.parent == null || transform.root.name != "Vehicle")
            return;

        if (rb == null)
            rb = transform.parent.GetComponent<Rigidbody>();
        if(rb != null && fixedJoint.connectedBody == null)
            fixedJoint.connectedBody = rb;

        if (GameApp.DataManager.mode != Mode.Play)
        {
            return;
        }

        //Collider[] colliders = Physics.OverlapSphere(transform.position, 1.1f, LayerMask.GetMask("Ground"), QueryTriggerInteraction.Collide);
        //isGrounded = (colliders.Length > 0 ? true : false);
        //if (!isGrounded)
        //{
        //    // 使轮子看起来在空中旋转
        //    // to do
        //    Debug.Log("空中");
        //    return;
        //}


        if (forwardKey.asValue != KeyCode.None && Input.GetKey(forwardKey.asValue))
        {
            wheelRb.AddTorque(transform.up * power, ForceMode.Force);
        }
        if (backKey.asValue != KeyCode.None && Input.GetKey(backKey.asValue))
        {
            wheelRb.AddTorque(-transform.up * power, ForceMode.Force);
        }
    }
}