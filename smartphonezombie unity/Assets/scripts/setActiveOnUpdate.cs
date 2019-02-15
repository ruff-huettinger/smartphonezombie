using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setActiveOnUpdate : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        //survival of the fittest ... last
        foreach (setActiveOnUpdate sibling in gameObject.GetComponents<setActiveOnUpdate>()) if (sibling != this) DestroyImmediate(sibling);
    }

    public bool willBeActive = true;

    public void set(bool to)
    {
        willBeActive = to;
    }

    public void set()
    {
       if(gameObject.active != willBeActive) gameObject.SetActive(willBeActive);
    }

    // Update is called once per frame
}
