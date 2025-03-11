using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoadVehicleView : BaseView
{
    protected override void OnAwake()
    {
        base.OnAwake();

    }

    public void RefreshLoadView()
    {
        Transform content = Find<Transform>("LoadVehicleView/Scroll View/Viewport/Content");
        GameObject prefabButton = Resources.Load("View/EditView") as GameObject;

        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        DirectoryManager.GetDirectory("Vehicles").GetFiles()
            .Where((FileInfo file) => file.Name.EndsWith(".vehicle")).ToList()
            .ForEach((FileInfo file) =>
            {
                Button button = Instantiate(prefabButton, content).GetComponent<Button>();
                string vehicleName = file.Name.Substring(0, file.Name.IndexOf('.'));
                button.GetComponentInChildren<Text>().text = vehicleName;

                //to do
                //button.onClick.AddListener(() =>
                //{
                //    GameApp.DataManager.LoadVehicle(vehicleEditor, vehicleName);
                //});
            });
    }
}
