using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyProperty : UIProperty
{
    [SerializeField] Button button;
    bool inputKey = false;

    protected override void Start()
    {
        base.Start();
        if(button != null && property != null && property.type == Property.Type.Key)
        {
            button.GetComponentInChildren<Text>().text = (property.reference as Ref<KeyCode>).asValue.ToString();
            button.onClick.AddListener(() => {
                inputKey = true;
                button.GetComponentInChildren<Text>().text = "Press a key";
            });
        }
    }

    private void Update()
    {
        if(inputKey && property != null && property.type == Property.Type.Key)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
            {
                (property.reference as Ref<KeyCode>).asRef[0] = KeyCode.None;
                inputKey = false;
                button.GetComponentInChildren<Text>().text = (property.reference as Ref<KeyCode>).asValue.ToString();
            }
            else
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(key))
                    {
                        (property.reference as Ref<KeyCode>).asRef[0] = key;
                        inputKey = false;
                        button.GetComponentInChildren<Text>().text = (property.reference as Ref<KeyCode>).asValue.ToString();
                    }
                }
            }
        }
    }
}
