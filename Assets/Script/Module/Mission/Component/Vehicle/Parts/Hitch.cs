using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Hitch : Part
{
    [SerializeField] float leftAngleSide = 60.0f;
    [SerializeField] float rightAngleSide = 60.0f;
    [SerializeField] float rotateSpeed = 1.0f;
    [SerializeField] Transform movablePart;

    Ref<KeyCode> leftKey = new Ref<KeyCode>(KeyCode.None);
    Ref<KeyCode> rightKey = new Ref<KeyCode>(KeyCode.None);
    float rotateY = 0f;

    protected override void Awake()
    {
        base.Awake();
        properties.Add(new Property("向左旋转", leftKey));
        properties.Add(new Property("向右旋转", rightKey));
        rotateY = 0f;
    }
    protected void FixedUpdate()
    {
        if (transform.parent == null || transform.root.name != "Vehicle")
            return;

        if (GameApp.DataManager.mode != Mode.Play)
        {
            return;
        }

        bool leftRotate = false;
        if (leftKey.asValue != KeyCode.None)
        {
            leftRotate = Input.GetKey(leftKey.asValue);
        }
        if (leftRotate && rotateY > -leftAngleSide)
        {
            rotateY -= rotateSpeed * Time.deltaTime;
            movablePart.Rotate(0f, -rotateSpeed * Time.deltaTime, 0f, Space.Self);
            onRelativePositionChange.Invoke(transform);
            transform.parent.GetComponent<VehicleEditorController>().ComputeCenterOfMass();
        }

        bool rightRotate = false;
        if (rightKey.asValue != KeyCode.None)
        {
            rightRotate = Input.GetKey(rightKey.asValue);
        }
        if (rightRotate && rotateY < rightAngleSide)
        {
            rotateY += rotateSpeed * Time.deltaTime;
            movablePart.Rotate(0f, rotateSpeed * Time.deltaTime, 0f, Space.Self);
            onRelativePositionChange.Invoke(transform);
            transform.parent.GetComponent<VehicleEditorController>().ComputeCenterOfMass();
        }
    }
}
