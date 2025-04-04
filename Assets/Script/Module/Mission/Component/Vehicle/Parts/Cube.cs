using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cube : Part
{
    protected override void Awake()
    {
        base.Awake();
        this.prefabName = "Cube";
    }
}