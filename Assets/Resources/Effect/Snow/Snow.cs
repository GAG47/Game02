using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snow : MonoBehaviour
{
    public RenderTexture rt;
    public Texture drawImg;
    public Texture defaultImg;
    public Camera mainCamera;
    void Start()
    {
        mainCamera = Camera.main.GetComponent<Camera>();

        GetComponent<Renderer>().material.mainTexture = rt;

        DrawDefault();
    }

    public void DrawDefault()
    {
        RenderTexture.active = rt;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, rt.width, rt.height, 0);

        Rect rect = new Rect(0, 0, rt.width, rt.height);
        //TODO: ªÊ÷∆Ã˘Õº
        Graphics.DrawTexture(rect, defaultImg);

        GL.PopMatrix();

        RenderTexture.active = null;
    }

    public void Draw(int x,int y)
    {
        Debug.Log("Draw " + x + " " + y);

        RenderTexture.active = rt;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, rt.width, rt.height, 0);

        x -= (int)(drawImg.width * 0.5f);
        y -= (int)(drawImg.height * 0.5f);
        Rect rect = new Rect(x, y, drawImg.width, drawImg.height);
        //TODO: ªÊ÷∆Ã˘Õº
        Graphics.DrawTexture(rect, drawImg);

        GL.PopMatrix();

        RenderTexture.active = null;
    }

    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit " + hit.textureCoord);
                Draw((int)(hit.textureCoord.x * rt.width), (int)(rt.height - hit.textureCoord.y * rt.height));
            }
        }
    }
}
