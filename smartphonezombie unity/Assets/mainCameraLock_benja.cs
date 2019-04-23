using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainCameraLock_benja : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (mainCamera == null)
        {
            mainCamera = GetComponent<Camera>();
        }
    }

    Camera mainCamera;



    private void OnPreRender()
    {
        if (mainCamera != null)
        {
            Camera currentCam = Camera.main;
            if (currentCam != mainCamera)
            {
                mainCamera.tag = "MainCamera";
                if (currentCam.tag == "MainCamera")
                    currentCam.tag = "Untagged";
                currentCam.enabled = false;
                currentCam.enabled = true;
                Debug.Log("Main Camera has been set to " + mainCamera.gameObject.name);
            }
        }
    }
}
