using UnityEngine;

public class RotateWithTorque : MonoBehaviour
{
    [Header("目标物体")]
    public Transform target; // 要跟随的物体A

    [Header("跟随设置")]
    [Tooltip("是否保持初始相对位置")]
    public bool keepPosition = true;

    [Tooltip("是否保持初始相对旋转")]
    public bool keepRotation = true;

    // 相对变换参数
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

        // 位置跟随（考虑目标物体的缩放）
        if (keepPosition)
        {
            transform.position = target.TransformPoint(originPositionOffset);
        }

        // 旋转跟随
        if (keepRotation)
        {
            transform.rotation = target.rotation * originRotationOffset;
        }
        Debug.Log("transform:" + transform.position + " " + transform.rotation);
    }
}