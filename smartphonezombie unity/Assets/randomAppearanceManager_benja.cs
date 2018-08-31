using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class randomAppearanceManager_benja : MonoBehaviour {

    public RandomAppearence_benja[] all;
    public int maxObjects = 0;
    public bool reduceDoublets = false;
    public bool changeNow = false;

    public bool getValuesFromFirstObject = false;

    public Material[] materials;
    public bool changeMaterials = false;

    public Mesh[] meshes;
    public bool changeMeshes = false;

    public Vector3[] posOffsets;
    public bool changePositions = false;

    public Vector3[] rotOffsets;
    public bool changeRotations = false;

    public void randomizeAppearance()
    {
        changeNow = false;
        if (!(reduceDoublets || maxObjects > 0))
        {
            foreach (RandomAppearence_benja obj in all)
            {
                obj.randomizeAppearance();
            }
            return;
        }

        if (maxObjects > 0)
        {
            maxObjects = Mathf.Min(maxObjects, all.Length);
        }
        else
        {
            maxObjects = all.Length;
        }

        reduceDoublets = true;
        if (reduceDoublets)
        {
            int[] mat = new int[0];          
            if (changeMaterials)
                mat = BenjasMath.repeatArray(BenjasMath.intArray(0, materials.Length), maxObjects);

            int[] mes = new int[0];
            if (changeMeshes)
                mes = BenjasMath.repeatArray(BenjasMath.intArray(0, meshes.Length), maxObjects);

            int[] pos = new int[0];
            if (changePositions)
                pos = BenjasMath.repeatArray(BenjasMath.intArray(0, posOffsets.Length), maxObjects);

            int[] rot = new int[0];
            if (changeRotations)
                rot = BenjasMath.repeatArray(BenjasMath.intArray(0, rotOffsets.Length), maxObjects);

            List<RandomAppearence_benja> allObj = all.ToList<RandomAppearence_benja>();
            int count = allObj.Count;

            for (int j = 0; j < count; j++)
            {
                if (j < maxObjects)
                {
                    int i = Random.Range(0, allObj.Count);
                    
                    if (changeMaterials)
                    {
                        allObj[i].rendi.material = materials[mat[j]];
                        
                    }
                    if (changeMeshes)
                    {
                        allObj[i].meshi.mesh = meshes[mes[j]];
                    }
                    if (changePositions)
                    {
                        allObj[i].transform.localPosition = allObj[i].posOriginal + posOffsets[pos[j]];
                    }
                    if (changeRotations)
                    {
                        allObj[i].transform.localEulerAngles = allObj[i].rotOriginal + rotOffsets[rot[j]];
                    }
                    allObj.RemoveAt(i);

                }
                else
                {
                    allObj[0].rendi.enabled = false;
                    allObj.RemoveAt(0);
                }
                
            }
        }

    }


	// Use this for initialization
	void Start () {
        all = GetComponentsInChildren<RandomAppearence_benja>();

        if (getValuesFromFirstObject)
        {
            materials = all[0].materials;
            changeMaterials = all[0].changeMaterials;

            meshes = all[0].meshes;
            changeMeshes = all[0].changeMeshes;

            posOffsets = all[0].posOffsets;
            changePositions = all[0].changePosition;

            rotOffsets = all[0].rotOffsets;
            changeRotations = all[0].changeRotations;
        }
}
	
	// Update is called once per frame
	void Update () {
        if (changeNow)
            randomizeAppearance();
	}
}
