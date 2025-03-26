using UnityEngine;

public class RotateWithTorque : MonoBehaviour
{
    // 相对变换参数
    private Vector3 originPositionOffset;
    private Quaternion originRotationOffset;

    private float angle;
    public float angleSpeed;
    public float moveSpeed;

    //注意这八个变量，四个是获取车轮碰撞器的，四个是获取车轮模型的
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

    //控制移动 转向
    void WheelsControl_Update()
    {
        //垂直轴和水平轴
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        //前轮角度，后轮驱动
        //steerAngle:转向角度，总是围绕自身Y轴，转向
        //motorTorque:电机转矩，驱动车轮
        angle = angleSpeed * h;
        leftF.steerAngle = angle;
        rightF.steerAngle = angle;

        leftB.motorTorque = v * moveSpeed;
        rightB.motorTorque = v * moveSpeed;

        //当车轮碰撞器位置角度改变，随之也变更车轮模型的位置角度
        WheelsModel_Update(model_leftF, leftF);
        WheelsModel_Update(model_leftB, leftB);
        WheelsModel_Update(model_rightF, rightF);
        WheelsModel_Update(model_rightB, rightB);
    }

    //控制车轮模型移动 转向
    void WheelsModel_Update(Transform t, WheelCollider wheel)
    {
        Vector3 pos = t.position;
        Quaternion rot = t.rotation;

        wheel.GetWorldPose(out pos, out rot);

        t.position = pos;
        t.rotation = rot;
    }


}