using UnityEngine;

public class RotateWithTorque : MonoBehaviour
{
    // ��Ա任����
    private Vector3 originPositionOffset;
    private Quaternion originRotationOffset;

    private float angle;
    public float angleSpeed;
    public float moveSpeed;

    //ע����˸��������ĸ��ǻ�ȡ������ײ���ģ��ĸ��ǻ�ȡ����ģ�͵�
    public WheelCollider leftF;
    public WheelCollider leftB;
    public WheelCollider rightF;
    public WheelCollider rightB;

    public Transform model_leftF;
    public Transform model_leftB;
    public Transform model_rightF;
    public Transform model_rightB;

    void Update()
    {
        WheelsControl_Update();
    }

    //�����ƶ� ת��
    void WheelsControl_Update()
    {
        //��ֱ���ˮƽ��
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        //ǰ�ֽǶȣ���������
        //steerAngle:ת��Ƕȣ�����Χ������Y�ᣬת��
        //motorTorque:���ת�أ���������
        angle = angleSpeed * h;
        leftF.steerAngle = angle;
        rightF.steerAngle = angle;

        leftB.motorTorque = v * moveSpeed;
        rightB.motorTorque = v * moveSpeed;

        //��������ײ��λ�ýǶȸı䣬��֮Ҳ�������ģ�͵�λ�ýǶ�
        WheelsModel_Update(model_leftF, leftF);
        WheelsModel_Update(model_leftB, leftB);
        WheelsModel_Update(model_rightF, rightF);
        WheelsModel_Update(model_rightB, rightB);
    }

    //���Ƴ���ģ���ƶ� ת��
    void WheelsModel_Update(Transform t, WheelCollider wheel)
    {
        Vector3 pos = t.position;
        Quaternion rot = t.rotation;

        wheel.GetWorldPose(out pos, out rot);

        t.position = pos;
        t.rotation = rot;
    }


}