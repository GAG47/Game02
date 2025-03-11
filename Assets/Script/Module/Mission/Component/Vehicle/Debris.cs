using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour
{
    Rigidbody rb = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        GameApp.DataManager.onModeChange.AddListener(() =>
        {
            if (GameApp.DataManager.mode == Mode.Edit)
            {
                Destroy(gameObject);
            }
        });
    }

    public void ComputeCenterOfMass()
    {
        Vector3 com = Vector3.zero;
        float massSum = 0.0f;
        foreach (Part part in GetComponentsInChildren<Part>())
        {
            massSum += part.mass;
            com += part.mass * part.transform.localPosition;
        }
        rb.centerOfMass = com / massSum;
        rb.mass = massSum;
    }
}
