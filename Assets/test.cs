using UnityEngine;

public class RotateWithTorque : MonoBehaviour
{
    // 旋转速度，可在Inspector面板中调整
    public float rotationSpeed = 10f;

    // 用于存储Rigidbody组件的引用
    [SerializeField]private Rigidbody rbParent;
    private Rigidbody rb;
    private FixedJoint fj;
    void Start()
    {
        // 获取当前游戏对象上的Rigidbody组件
        rb = GetComponent<Rigidbody>();
        fj = GetComponent<FixedJoint>();
    }

    void Update()
    {
        // 检查是否按下W键
        if (Input.GetKey(KeyCode.W))
        {
            // 向前旋转，通过施加绕Y轴正方向的扭矩来实现
            ApplyTorque(Vector3.right);
        }
        // 检查是否按下S键
        else if (Input.GetKey(KeyCode.S))
        {
            // 向后旋转，通过施加绕Y轴负方向的扭矩来实现
            ApplyTorque(Vector3.left);
        }
    }

    void ApplyTorque(Vector3 direction)
    {
        if (rb != null)
        {
            // 计算扭矩值，将旋转速度与方向结合
            Vector3 torque = direction * rotationSpeed;
            // 向刚体施加扭矩，使用ForceMode.Force模式
            rb.AddTorque(torque, ForceMode.Force);
        }
    }
}