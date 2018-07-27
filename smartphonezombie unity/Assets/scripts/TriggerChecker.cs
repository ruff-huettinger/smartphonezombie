using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerChecker : MonoBehaviour {


    public delegate void Deli();
    public Deli onTrigger;
    
	// Use this for initialization
	void Start () {
        GetComponent<Collider>().isTrigger = true;
	}

    public void OnTriggerEnter(Collider other)
    {
        if (onTrigger != null)
        {
            onTrigger();
        }
    }

}
