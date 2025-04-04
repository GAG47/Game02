using Unity.VisualScripting;
using UnityEngine;

public class Wheel : Part
{
    //��������
    [SerializeField] float power = 2000.0f;
    [SerializeField] float rotationSpeed = 2000.0f;
    private Transform wheelMod;
    private Quaternion originRotation;
    [SerializeField] private Transform wheelLink; //�������
    [SerializeField] private HingeJoint m_hingeJoint; //�����ؽ�

    //�����������
    Ref<KeyCode> forwardKey = new Ref<KeyCode>(KeyCode.None);
    Ref<KeyCode> backKey = new Ref<KeyCode>(KeyCode.None);
    bool forward = false;
    bool back = false;

    //���߼�����
    private bool onGround;
    [SerializeField] private int rayCount = 36;
    [SerializeField] private float rayLength = 1.05f;
    [SerializeField] private Transform detectionCenter;

    protected override void Awake()
    {
        base.Awake();
        this.prefabName = "Wheel";

        properties.Add(new Property("ǰ��", forwardKey));
        properties.Add(new Property("����", backKey));

        wheelMod = transform.Find("WheelModel");
        originRotation = new Quaternion(0, 0, 0, 1);
        m_rigidbody = transform.Find("WheelModel").GetComponent<Rigidbody>();

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
                    wheelMod.localPosition = new Vector3(0, 0.5f, 0);
                    wheelMod.localRotation = new Quaternion(0, 0, 0, 1);
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
        //ǰ��/����
        if (forwardKey.asValue != KeyCode.None && Input.GetKey(forwardKey.asValue) && this.isLinkedCore)
        {
            //if(onGround)
            //{
            //    m_hingeJoint.motor = new JointMotor { force = power, targetVelocity = rotationSpeed };
            //    m_hingeJoint.useMotor = true;
            //}
            //else
            //{
            //    m_hingeJoint.useMotor = false;
            //}
            m_rigidbody.AddTorque(transform.right * power, ForceMode.Force);
            forward = true;
        }
        else
        {
            forward = false;
        }
        if (backKey.asValue != KeyCode.None && Input.GetKey(backKey.asValue) && this.isLinkedCore)
        {
            //if(onGround)
            //{
            //    m_hingeJoint.motor = new JointMotor { force = power, targetVelocity = -rotationSpeed };
            //    m_hingeJoint.useMotor = true;
            //}
            //else
            //{
            //    m_hingeJoint.useMotor = false;
            //}
            m_rigidbody.AddTorque(-transform.right * power, ForceMode.Force);
            back = true;
        }
        else
        {
            back = false;
        }
    }

    public override void AttachToFixed(PartJoint m_joint, PartJoint _joint)
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
            FixedJoint fixedJoint = wheelLink.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = _joint.fixedPart;
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
    }

    private void CollisionStay()
    {
        bool _notOnGround = true;
        float angleStep = 360f / rayCount;
        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * angleStep;
            float radian = angle * Mathf.Deg2Rad;

            //�����ת��Ԫ��
            Quaternion currentRotation = transform.rotation;
            Quaternion rotationDelta = Quaternion.Inverse(originRotation) * currentRotation;

            Vector3 direction = new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian));
            direction = rotationDelta * direction;

            //���߼��
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

    #region ������
    //private void CollisionStay()
    //{
    //    float angleStep = 360f / rayCount;
    //    for (int i = 0; i < rayCount; i++)
    //    {
    //        float angle = i * angleStep;
    //        float radian = angle * Mathf.Deg2Rad;

    //        //�����ת��Ԫ��
    //        Quaternion currentRotation = transform.rotation;
    //        Quaternion rotationDelta = Quaternion.Inverse(originRotation) * currentRotation;

    //        Vector3 direction = new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian));
    //        direction = rotationDelta * direction;

    //        //���߼��
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