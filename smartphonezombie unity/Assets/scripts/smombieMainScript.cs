using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smombieMainScript : MonoBehaviour {

    smombieObstackleManager obstControl;
    recordAndPlayPath_Benja pathControl;
    public float speed;
    public float pathProgress;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        pathProgress = pathControl.playheadPosition01();

	}
}
