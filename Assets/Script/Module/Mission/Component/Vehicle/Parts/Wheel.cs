using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using static UnityEngine.Rendering.HableCurve;

public class Wheel : Part
{
    [SerializeField] float power = 50.0f;
    [SerializeField] float rotationSpeed = 50.0f;
    private Rigidbody rb;
    private Transform wheelMod;
    private WheelCollider wheelCol;

    Ref<KeyCode> forwardKey = new Ref<KeyCode>(KeyCode.None);
    Ref<KeyCode> backKey = new Ref<KeyCode>(KeyCode.None);

    private bool isGrounded = false;

    protected override void Awake()
    {
        base.Awake();
        properties.Add(new Property("前进", forwardKey));
        properties.Add(new Property("后退", backKey));

        wheelMod = transform.Find("WheelModel");
        wheelCol = transform.Find("WheelCollider").GetComponent<WheelCollider>();
    }

    void Update()
    {
        if (GameApp.DataManager.mode != Mode.Play)
        {
            wheelMod.localPosition = new Vector3(0, 0.5f, 0);
            wheelMod.localRotation = new Quaternion(0, 0, 0, 1);
            Debug.Log(wheelMod.position);
            return;
        }

        if (transform.parent == null || transform.root.name != "Vehicle")
            return;

        if (rb == null)
            rb = transform.parent.GetComponent<Rigidbody>();


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
            wheelCol.motorTorque = rotationSpeed;

            //Quaternion rot = wheelMod.rotation;
            //wheelCol.GetWorldPose(out _, out rot);
            //wheelMod.rotation = rot; //* Quaternion.Euler(0, 0, 90);
        }
        if (backKey.asValue != KeyCode.None && Input.GetKey(backKey.asValue))
        {
            wheelCol.motorTorque = -rotationSpeed;

            //Quaternion rot = wheelMod.rotation;
            //wheelCol.GetWorldPose(out _, out rot);
            //wheelMod.rotation = rot; //* Quaternion.Euler(0, 0, 90); 
        }
    }

    void OnDrawGizmosSelected()
    {
        //Color gizmoColor = Color.cyan;
        //int segments = 32;
        //Gizmos.color = gizmoColor;

        //Vector3 wheelWorldPos;
        //Quaternion wheelWorldRot;
        //wheelCol.GetWorldPose(out wheelWorldPos, out wheelWorldRot);

        //float wheelRadius = wheelCol.radius;

        //for (int i = 0; i < segments; i++)
        //{
        //    float angle1 = i * 2 * Mathf.PI / segments;
        //    float angle2 = (i + 1) * 2 * Mathf.PI / segments;

        //    Vector3 point1 = wheelWorldPos + wheelWorldRot * new Vector3(Mathf.Cos(angle1), 0, Mathf.Sin(angle1)) * wheelRadius;
        //    Vector3 point2 = wheelWorldPos + wheelWorldRot * new Vector3(Mathf.Cos(angle2), 0, Mathf.Sin(angle2)) * wheelRadius;

        //    Gizmos.DrawLine(point1, point2);
        //}
    }
}