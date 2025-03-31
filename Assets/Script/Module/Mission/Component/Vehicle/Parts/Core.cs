using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : Part
{
    protected override void Awake()
    {
        base.Awake();
        this.isLinkedCore = true;

        GameApp.DataManager.onModeChange.AddListener(() =>
        {
            switch (GameApp.DataManager.mode)
            {
                case Mode.Edit:
                    this.transform.position = Vector3.zero;
                    this.transform.rotation = Quaternion.identity;
                    break;
                case Mode.Play:
                    break;
                default:
                    break;
            }
        });
    }
}