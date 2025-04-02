using Unity.VisualScripting;
using UnityEngine;

public class Wheel : Cube
{
    [SerializeField] float power = 200.0f;
    [SerializeField] float rotationSpeed = 200.0f;
    private Rigidbody rb;
    private Transform wheelMod;
    private bool onGround;

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

        GameApp.DataManager.onModeChange.AddListener(() =>
        {
            switch (GameApp.DataManager.mode)
            {
                case Mode.Edit:
                    wheelMod.localPosition = new Vector3(0, 0.5f, 0);
                    wheelMod.localRotation = new Quaternion(0, 0, 0, 1);
                    break;
                case Mode.Play:
                    break;
                default:
                    break;
            }
        });
    }

    void Update()
    {
        if (transform.parent == null || transform.root.name != "Vehicle")
        {
            return;
        }
        if (GameApp.DataManager.mode != Mode.Play || isLinkedCore == false)
        {
            return;
        }

        CollisionStay();
        //前进/后退
        if (forwardKey.asValue != KeyCode.None && Input.GetKey(forwardKey.asValue) && this.isLinkedCore)
        {
            m_rigidbody.AddTorque(transform.up * rotationSpeed);
            forward = true;
        }
        else
        {
            forward = false;
        }
        if (backKey.asValue != KeyCode.None && Input.GetKey(backKey.asValue) && this.isLinkedCore)
        {
            m_rigidbody.AddTorque(-transform.up * rotationSpeed);
            back = true;
        }
        else
        {
            back = false;
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
            HingeJoint hingejoint = this.transform.AddComponent<HingeJoint>();
            hingejoint.connectedBody = _joint.part.GetComponent<Rigidbody>();
            hingejoint.anchor = m_joint.transform.localPosition;
            hingejoint.connectedMassScale = 10.0f;
            Debug.Log(m_joint.transform.localPosition);
            hingejoint.axis = transform.forward;
            hingejoint.useSpring = true;
            JointSpring spring = hingejoint.spring;
            spring.spring = 50000.0f;
            spring.damper = 5000.0f;
            hingejoint.spring = spring;
            hingejoint.useLimits = false;
            hingejoint.breakForce = 50000.0f;
            hingejoint.breakTorque = 50000.0f;
            _joint.part.onDestroy.AddListener(() =>
            {
                Destroy(hingejoint);
            });
            if (m_coreLinkedJoint == m_joint)
            {
                _joint.part.onDestroy.AddListener(() =>
                {
                    RefindCoreLinkedJointRecursively(); //重新查找核心关节
                });
            }
        }
    }

    private void CollisionStay()
    {
        bool _notOnGround = true;
        float angleStep = 360f / rayCount;
        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * angleStep;
            float radian = angle * Mathf.Deg2Rad;

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
                    _notOnGround = false;
                }
                Debug.DrawLine(ray.origin, hit.point, Color.red);
            }
            else
            {
                Debug.DrawLine(ray.origin, ray.origin + direction * rayLength, Color.green);
            }
        }
        onGround = !_notOnGround;
        return;
    }

    #region 已禁用
    //private void CollisionStay()
    //{
    //    //Debug.Log("123");
    //    float angleStep = 360f / rayCount;
    //    for (int i = 0; i < rayCount; i++)
    //    {
    //        float angle = i * angleStep;
    //        float radian = angle * Mathf.Deg2Rad;

    //        //获得旋转四元数
    //        Quaternion currentRotation = transform.rotation;
    //        Quaternion rotationDelta = Quaternion.Inverse(originRotation) * currentRotation;

    //        Vector3 direction = new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian));
    //        direction = rotationDelta * direction;

    //        //射线检测
    //        Ray ray = new Ray(detectionCenter.position, direction);
    //        RaycastHit hit;

    //        if (Physics.Raycast(ray, out hit, rayLength, LayerMask.GetMask("Ground")))
    //        {
    //            if (GameApp.DataManager.mode == Mode.Play)
    //            {
    //                ForceAtPoint(hit.point, -direction);
    //            }
    //            Debug.DrawLine(ray.origin, hit.point, Color.red);
    //        }
    //        else
    //        {
    //            Debug.DrawLine(ray.origin, ray.origin + direction * rayLength, Color.green);
    //        }
    //    }
    //}
    //private void ForceAtPoint(Vector3 pos, Vector3 nor)
    //{
    //    Vector3 relativeVector = nor;
    //    Quaternion worldYRotation = Quaternion.Euler(0f, -90f, 0f);
    //    Quaternion localRotation = transform.rotation * worldYRotation * Quaternion.Inverse(transform.rotation);
    //    Vector3 rotatedNor = localRotation * relativeVector;

    //    Debug.DrawLine(pos, pos + rotatedNor, Color.green);
    //    if (forward)
    //    {
    //        Debug.Log("forward");
    //        //Debug.DrawLine(rotatedNor * power, pos, Color.green);
    //        m_rigidbody.AddForceAtPosition(rotatedNor * power, pos, ForceMode.Force);
    //    }
    //    if (back)
    //    {
    //        Debug.Log("back");
    //        //Debug.DrawLine(-rotatedNor * power, pos, Color.green);
    //        m_rigidbody.AddForceAtPosition(-rotatedNor * power, pos, ForceMode.Force);
    //    }
    //}
    #endregion
}