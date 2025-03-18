using UnityEngine;

public class RotateWithTorque : MonoBehaviour
{
    [Header("Ŀ������")]
    public Transform target; // Ҫ���������A

    [Header("��������")]
    [Tooltip("�Ƿ񱣳ֳ�ʼ���λ��")]
    public bool keepPosition = true;

    [Tooltip("�Ƿ񱣳ֳ�ʼ�����ת")]
    public bool keepRotation = true;

    // ��Ա任����
    private Vector3 originPositionOffset;
    private Quaternion originRotationOffset;

    void Start()
    {
        if (target == null) return;

        originPositionOffset = target.InverseTransformPoint(transform.position);
        originRotationOffset = Quaternion.Inverse(target.rotation) * transform.rotation;
        Debug.Log(originPositionOffset + " " + originRotationOffset);
    }

    void LateUpdate()
    {
        if (target == null) return;

        // λ�ø��棨����Ŀ����������ţ�
        if (keepPosition)
        {
            transform.position = target.TransformPoint(originPositionOffset);
        }

        // ��ת����
        if (keepRotation)
        {
            transform.rotation = target.rotation * originRotationOffset;
        }
        Debug.Log("transform:" + transform.position + " " + transform.rotation);
    }
}