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
    private bool initiated = false;

    public float propabilityOfExistance = 1f;
    string name = "";

    // Use this for initialization
    private void Awake () {
		rendi=this.gameObject.GetComponent<MeshRenderer>();
		meshi = this.gameObject.GetComponent<MeshFilter>();
        rotOriginal = transform.localEulerAngles;
        posOriginal = transform.localPosition;
        name = gameObject.name;
        initiated = true;
	}

    public void initiate()
    {
        if (!initiated) Awake();
    }

	int randomInt (float i)
	{
		return (int) Mathf.Floor(Random.value*(float)(i+0.999999f));
	}

    public float probValue = 0;

    public void randomizeAppearance()
    {
        changeNow = false;

        Mathf.Clamp01(propabilityOfExistance);
        probValue = Random.value;
        if (probValue >= propabilityOfExistance)
            invisibalize();
        else
        {
            visibalize();

            if (changeMeshes && meshes.Length > 1)
            {
                int i = randomInt(meshes.Length - 1);
                if (meshes[i] != null)
                {
                    meshi.mesh = meshes[i];
                    gameObject.name += " mesh " + i.ToString();
                }
            }
            if (changeMaterials && materials.Length > 1)
            {
                int i = randomInt(materials.Length - 1);
                if (materials[i] != null)
                {
                    rendi.material = materials[i];
                    gameObject.name += " material " + i.ToString();
                }
            }
            if (changePosition && posOffsets.Length > 1)
            {
                int i = randomInt(posOffsets.Length - 1);
                transform.localPosition = posOriginal + posOffsets[i];
                gameObject.name += " pos " + i.ToString();
            }
            if (changeRotations && rotOffsets.Length > 1)
            {
                int i = randomInt(rotOffsets.Length - 1);
                transform.localEulerAngles = rotOriginal + rotOffsets[i];
                gameObject.name += " rot " + i.ToString();
            }
        }


    }

    public void invisibalize()
    {
        rendi.enabled = false;
        gameObject.name = name + "(invisible)";
    }

    public void visibalize()
    {
        rendi.enabled = true;
        gameObject.name = name;
    }

    // Update is called once per frame
    void Update ()
    {
        if (changeNow) randomizeAppearance();


    }
}
