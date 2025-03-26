using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using static UnityEngine.Rendering.HableCurve;

public class Wheel : Part
{
    [SerializeField] float power = 200.0f;
    [SerializeField] float rotationSpeed = 200.0f;
    private Rigidbody rb;
    private Transform wheelMod;

    Ref<KeyCode> forwardKey = new Ref<KeyCode>(KeyCode.None);
    Ref<KeyCode> backKey = new Ref<KeyCode>(KeyCode.None);
    bool forward = false;
    bool back = false;

    private Quaternion originRotation;

    [SerializeField] private int rayCount = 36;
    [SerializeField] private float rayLength = 1.05f;
    [SerializeField] private Transform detectionCenter;

    protected override void Awake()
    {
        base.Awake();
        properties.Add(new Property("前进", forwardKey));
        properties.Add(new Property("后退", backKey));

        wheelMod = transform.Find("WheelModel");
        originRotation = new Quaternion(0, 0, 0, 1);
    }

    void Update()
    {
        CollisionStay();
        if (GameApp.DataManager.mode != Mode.Play)
        {
            wheelMod.localPosition = new Vector3(0, 0.5f, 0);
            wheelMod.localRotation = new Quaternion(0, 0, 0, 1);
            //Debug.Log(wheelMod.position);
            return;
        }

        if (transform.parent == null || transform.root.name != "Vehicle")
            return;

        if (rb == null)
            rb = transform.parent.GetComponent<Rigidbody>();
    


        if (forwardKey.asValue != KeyCode.None && Input.GetKey(forwardKey.asValue))
        {
            forward = true;
        }
        else
        {
            forward = false;
        }
        if (backKey.asValue != KeyCode.None && Input.GetKey(backKey.asValue))
        {
            back = true;
        }
        else
        {
            back = false;
        }
    }

    private void CollisionStay()
    {
        //Debug.Log("123");
        float angleStep = 360f / rayCount;
        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * angleStep;
            float radian = angle * Mathf.Deg2Rad;

            //forward会有问题 更换为四元数
            //Debug.Log("位置：" + transform.position);
            //Debug.Log("朝向：" + transform.forward);
            //Vector3 v1 = originForward, v2 = transform.forward;
            //Vector3 n = Vector3.Cross(v1, v2).normalized;
            //float cos_theta = Vector3.Dot(v1.normalized, v2.normalized);
            //float theta = Mathf.Acos(cos_theta);
            //Quaternion q = Quaternion.AngleAxis(theta * Mathf.Rad2Deg, n);
            //Debug.Log("旋转： " + q);

            //获得旋转四元数
            Quaternion currentRotation = transform.rotation;
            Quaternion rotationDelta = Quaternion.Inverse(originRotation) * currentRotation;

            Vector3 direction = new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian));
            direction = rotationDelta * direction;
    
            //射线检测
            Ray ray = new Ray(detectionCenter.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayLength, LayerMask.GetMask("Ground")))
            {
                if (GameApp.DataManager.mode == Mode.Play)
                {
                    ForceAtPoint(hit.point, -direction);
                    //Debug.Log("车轮摩擦力方向： "+-direction);
                }
                Debug.DrawLine(ray.origin, hit.point, Color.red);
                //Debug.Log("射线 " + i + " 击中了物体: " + hit.collider.gameObject.name);
            }
            else
            {
                Debug.DrawLine(ray.origin, ray.origin + direction * rayLength, Color.green);
            }
        }
    }
    private void ForceAtPoint(Vector3 pos, Vector3 nor)
    {
        Vector3 relativeVector = nor;
        Quaternion worldYRotation = Quaternion.Euler(0f, -90f, 0f);
        Quaternion localRotation = transform.rotation * worldYRotation * Quaternion.Inverse(transform.rotation);
        Vector3 rotatedNor = localRotation * relativeVector;

        Debug.DrawLine(pos, pos + rotatedNor, Color.green);
        if (forward)
        {
            Debug.Log("forward");
            //Debug.DrawLine(rotatedNor * power, pos, Color.green);
            rb.AddForceAtPosition(rotatedNor * power, pos, ForceMode.Force);
        }
        if(back)
        {
            Debug.Log("back");
            //Debug.DrawLine(-rotatedNor * power, pos, Color.green);
            rb.AddForceAtPosition(-rotatedNor * power, pos, ForceMode.Force);
        }
    }
}