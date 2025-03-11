using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : Part
{
    [SerializeField] float power = 500.0f;
    [SerializeField] ParticleSystem particles;
    Rigidbody rb;
    Vector3 direction;

    Ref<KeyCode> key = new Ref<KeyCode>(KeyCode.None);

    protected override void Awake()
    {
        base.Awake();
        properties.Add(new Property("推进按键", key));
    }

    void Update()
    {
        if (transform.parent == null || transform.root.name != "Vehicle")
            return;

        if (rb == null)
            rb = transform.parent.GetComponent<Rigidbody>();

        if (GameApp.DataManager.mode != Mode.Play)
        {
            particles.enableEmission = parentJoint == null;

            return;
        }

        bool thrust = false;
        if (key.asValue != KeyCode.None)
        {
            thrust = Input.GetKey(key.asValue);
        }
        
        if (thrust)
        {
            rb.AddForceAtPosition(transform.up * power, transform.position);
            if(particles != null)
            {
                particles.enableEmission = true;
            }
        }
        else
        {
            if (particles != null)
            {
                particles.enableEmission = false;
            }
        }
    }
}
