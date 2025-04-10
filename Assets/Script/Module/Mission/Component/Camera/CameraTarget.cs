﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [SerializeField] new OrbitalCamera camera;
    [SerializeField] Transform otherTarget;
    [SerializeField] float speed = 1.0f;
    [SerializeField] float mouseSpeed = 1.0f;

    void Start()
    {
        ChangeTarget();
        GameApp.DataManager.onModeChange.AddListener(() => ChangeTarget());
    }

    Vector3 targetDirection = Vector3.zero;
    Vector3 currentDirection = Vector3.zero;
    Vector3 speedDirection = Vector3.zero;
    float dampDirection = 0.1f;

    void Update()
    {
        if (GameApp.DataManager.mode == Mode.Edit)
        {
            Vector3 forward = Vector3.Scale(new Vector3(1, 0, 1), camera.transform.forward).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, forward);
            targetDirection = Vector3.zero;

            if(Input.GetMouseButton(2) || (Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftShift)))
            {
                targetDirection = -(right * Input.GetAxisRaw("Mouse X") + forward * Input.GetAxisRaw("Mouse Y")) * mouseSpeed;
            }
            else
            {
                targetDirection = speed * (right * Input.GetAxisRaw("Horizontal") + Vector3.up * Input.GetAxisRaw("Vertical") + forward * Input.GetAxisRaw("Vertical")).normalized;
            }

            currentDirection = Vector3.SmoothDamp(currentDirection, targetDirection, ref speedDirection, dampDirection);
            transform.position += currentDirection * Time.deltaTime;

            //if(Input.GetKeyDown(KeyCode.A))
            //{
            //    transform.position = Vector3.zero;
            //}
        }
        else
        {
            if(Input.GetMouseButtonDown(2))
            {
                camera.followTargetRotation = !camera.followTargetRotation;
            }
        }

    }

    void ChangeTarget()
    {
        if (camera != null)
        {
            switch (GameApp.DataManager.mode)
            {
                case Mode.Edit:
                    camera.target = transform;
                    break;
                case Mode.Play:
                    if (otherTarget != null)
                    {
                        camera.target = otherTarget;
                    }
                    break;
            }
        }
    }
}
