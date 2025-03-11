using UnityEngine;

public class RotateWithTorque : MonoBehaviour
{
    // ��ת�ٶȣ�����Inspector����е���
    public float rotationSpeed = 10f;

    // ���ڴ洢Rigidbody���������
    [SerializeField]private Rigidbody rbParent;
    private Rigidbody rb;
    private FixedJoint fj;
    void Start()
    {
        // ��ȡ��ǰ��Ϸ�����ϵ�Rigidbody���
        rb = GetComponent<Rigidbody>();
        fj = GetComponent<FixedJoint>();
    }

    void Update()
    {
        // ����Ƿ���W��
        if (Input.GetKey(KeyCode.W))
        {
            // ��ǰ��ת��ͨ��ʩ����Y���������Ť����ʵ��
            ApplyTorque(Vector3.right);
        }
        // ����Ƿ���S��
        else if (Input.GetKey(KeyCode.S))
        {
            // �����ת��ͨ��ʩ����Y�Ḻ�����Ť����ʵ��
            ApplyTorque(Vector3.left);
        }
    }

    void ApplyTorque(Vector3 direction)
    {
        if (rb != null)
        {
            // ����Ť��ֵ������ת�ٶ��뷽����
            Vector3 torque = direction * rotationSpeed;
            // �����ʩ��Ť�أ�ʹ��ForceMode.Forceģʽ
            rb.AddTorque(torque, ForceMode.Force);
        }
    }
}