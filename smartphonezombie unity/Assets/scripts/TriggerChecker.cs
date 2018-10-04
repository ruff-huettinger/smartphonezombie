using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerChecker : MonoBehaviour {


    public delegate void Deli();
    public Deli onTrigger;
    public bool changeOnTrigger = false;
    
	// Use this for initialization
	void Start () {
        GetComponent<Collider>().isTrigger = true;
	}

    void OnTriggerEnter(Collider other)
    {
        changeOnTrigger = !changeOnTrigger;
        if (onTrigger != null)
        {
            onTrigger();
        }
        else
        {
            Debug.LogWarning("no function attached to onTrigger");
        }
    }

  

}
