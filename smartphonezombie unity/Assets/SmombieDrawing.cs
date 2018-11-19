using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmombieDrawing : MonoBehaviour {
    public Transform drowningPoint;
    public Transform evadePoint;
    public Transform playerCam;
    public float evadeDistance = 1;
    public float evadeLookAngle = -15;
    [Header("info only")]
    public bool evading = true;
    public bool drowning = false;

    Vector3 localpos;
    Vector3 localEul;
	// Use this for initialization
	void Start () {
        drowningPoint.gameObject.active = false;
        evadePoint.gameObject.active = false;
        localpos = playerCam.localPosition;
        localEul = playerCam.localEulerAngles;
	}

    public void Reset()
    {
        playerCam.localPosition = localpos;
        playerCam.localEulerAngles = localEul;
        drowning = false;
        //evading = ;
        time = 0;
    }

    public void evade()
    {
        evading = true;
    }

    public void drown()
    {
        drowning = true;
    }


    public float t;
    public float time = 0;
    // Update is called once per frame
    void Update () {
        Vector3 vec;
        if (drowning)
        {
            time += Time.deltaTime;
            vec = playerCam.position;
            vec = Vector3.Lerp(playerCam.position, drowningPoint.position, 0.5f);
            vec.y += .005f* Mathf.Sin(time * 3);
            playerCam.position = vec;
        }
        else if (evading)
        {
            // determine lerp factor t by z distance to target
            Vector3 localTarget = playerCam.InverseTransformPoint(evadePoint.position);
            float t = Mathf.Abs(playerCam.localPosition.z - localTarget.z);
            t = Mathf.InverseLerp(evadeDistance, 0 ,t);

            //lerp x position
            vec = playerCam.localPosition;
            vec.x = Mathf.Lerp(localpos.x, localTarget.x,t);
            playerCam.localPosition = vec;

            //lerp x rotation
            vec = playerCam.localEulerAngles;
            vec.x = localEul.x + Mathf.Lerp(0, evadeLookAngle, t);
            playerCam.localEulerAngles = vec;
        }
    }
}
