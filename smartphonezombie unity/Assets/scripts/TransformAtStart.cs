using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformAtStart : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        float x = (float)Convert.ToDouble(Configuration.GetAttricuteByTagName("camera_position", "x"));
        float y = (float)Convert.ToDouble(Configuration.GetAttricuteByTagName("camera_position", "y"));
        float z = (float)Convert.ToDouble(Configuration.GetAttricuteByTagName("camera_position", "z"));

        float r = (float)Convert.ToDouble(Configuration.GetAttricuteByTagName("camera_rotation", "x"));

        transform.localPosition = new Vector3(x, y, z);
        transform.localEulerAngles = new Vector3(r, 0, 0);
    }
}
