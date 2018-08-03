﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAppearence_benja : MonoBehaviour {

    public bool changeNow = false;

    public Material[] materials;
    public bool changeMaterials = false;
    public MeshRenderer rendi;

    public Mesh[] meshes;
    public bool changeMeshes = false;
    public MeshFilter meshi;

    public Vector3[] posOffsets;
    public bool changePosition = false;
    public Vector3 posOriginal;

    public Vector3[] rotOffsets;
    public bool changeRotations = false;
    public Vector3 rotOriginal;

    public float propabilityOfExistance = 1f;

    // Use this for initialization
    void Start () {
		rendi=this.gameObject.GetComponent<MeshRenderer>();
		meshi = this.gameObject.GetComponent<MeshFilter>();
        rotOriginal = transform.localEulerAngles;
        posOriginal = transform.localPosition;
	}


	int randomInt (float i)
	{
		return (int) Mathf.Floor(Random.value*(float)(i+0.999999f));
	}

    public void randomizeAppearance()
    {
        changeNow = false;
        if (changeMeshes && meshes.Length > 1)
        {
            int i = randomInt(meshes.Length - 1);
            if (meshes[i] != null)
            {
                meshi.mesh = meshes[i];
            }
        }
        if (changeMaterials && materials.Length > 1)
        {
            int i = randomInt(materials.Length - 1);
            if (materials[i] != null)
            {
                rendi.material = materials[i];
            }
        }
        if (changePosition && posOffsets.Length > 1)
        {
            int i = randomInt(posOffsets.Length - 1);
            if (posOffsets[i] != null)
            {
                transform.localPosition = posOriginal + posOffsets[i];
            }
        }
        if (changeRotations && rotOffsets.Length > 1)
        {
            int i = randomInt(rotOffsets.Length - 1);
            if (rotOffsets[i] != null)
            {

                transform.localEulerAngles = rotOriginal + rotOffsets[i];
            }
        }
        Mathf.Clamp01(propabilityOfExistance);
        if (propabilityOfExistance < 1)
        {
            rendi.enabled = Random.value < propabilityOfExistance;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (changeNow) randomizeAppearance();


    }
}
