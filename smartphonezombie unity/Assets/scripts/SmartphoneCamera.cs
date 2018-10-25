using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartphoneCamera : MonoBehaviour {
    Camera cam;

    static SmartphoneCamera instance;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        cam = GetComponent<Camera>();

        float x = (float)Convert.ToDouble(Configuration.GetAttricuteByTagName("camera_position", "x"));
        float y = (float)Convert.ToDouble(Configuration.GetAttricuteByTagName("camera_position", "y"));
        float z = (float)Convert.ToDouble(Configuration.GetAttricuteByTagName("camera_position", "z"));

        float r = (float)Convert.ToDouble(Configuration.GetAttricuteByTagName("camera_rotation", "x"));
        transform.localPosition = new Vector3(x, y, z);
        transform.localEulerAngles = new Vector3(r, 0, 0);
        
        float fieldOfView = (float)Convert.ToDouble(Configuration.GetInnerTextByTagName("camera_field_of_view", 60));
        cam.fieldOfView = fieldOfView;
        
        float vX = (float)Convert.ToDouble(Configuration.GetAttricuteByTagName("camera_viewport", "x"));
        float vY = (float)Convert.ToDouble(Configuration.GetAttricuteByTagName("camera_viewport", "y"));
        float vW = (float)Convert.ToDouble(Configuration.GetAttricuteByTagName("camera_viewport", "w"));
        float vH = (float)Convert.ToDouble(Configuration.GetAttricuteByTagName("camera_viewport", "h"));

        Rect rect = cam.rect;
        rect.position = new Vector2(vX, vY);
        rect.size = new Vector2(vW, vH);
        cam.rect = rect;
    }

    public void showView(bool on)
    {
        cam.enabled = on;
    }

    public static SmartphoneCamera GetInstance()
    {
        return instance;
    }
}
