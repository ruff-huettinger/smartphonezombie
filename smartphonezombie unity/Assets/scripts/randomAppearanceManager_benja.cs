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

    public bool randomizeOrder = true;
    public Material[] materials;
    public bool changeMaterials = false;

    public Mesh[] meshes;
    public bool changeMeshes = false;

    public Vector3[] posOffsets;
    public bool changePositions = false;

    public Vector3[] rotOffsets;
    public bool changeRotations = false;

    public bool test = false;

    public void randomizeAppearance()
    {
        changeNow = false;
        if (!(reduceDoublets || maxObjects > 0))
        {
            test = true;
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
            int offset = (int) Random.Range(0, 10);
            int[] mat = new int[0];          
            if (changeMaterials)
                mat = BenjasMath.repeatArray(BenjasMath.intArray(0, materials.Length, randomizeOrder), maxObjects+ offset);

            int[] mes = new int[0];
            if (changeMeshes)
                mes = BenjasMath.repeatArray(BenjasMath.intArray(0, meshes.Length, randomizeOrder), maxObjects + offset);

            int[] pos = new int[0];
            if (changePositions)
                pos = BenjasMath.repeatArray(BenjasMath.intArray(0, posOffsets.Length, randomizeOrder), maxObjects+ offset);

            int[] rot = new int[0];
            if (changeRotations)
                rot = BenjasMath.repeatArray(BenjasMath.intArray(0, rotOffsets.Length, randomizeOrder), maxObjects+ offset);

            //enable all
            for (int j=0;j<all.Length; j++)
            {
                all[j].visibalize();
            }

            //disable some randomly
            int[] disable = BenjasMath.intArray(0, all.Length, true);

            for (int j = 0; j < all.Length-maxObjects; j++)
            {
                all[disable[j]].invisibalize();
            }

            //distribute the rest


            int mati = offset;
            int mesi = offset;
            int posi = offset;
            int roti = offset;

            for (int j = 0; j < all.Length; j++)
            {
                Debug.Log(all[j].name + " " + all[j].rendi.enabled);
                if (all[j].rendi.enabled)
                {

                    if (changeMaterials && mat.Length>0)
                    {
                         BenjasMath.cycle(ref mati , mat.Length-1);
                        Debug.Log(all[j].name +" "+mati);
                        all[j].rendi.material = materials[mat[mati]];
                    }
                    if (changeMeshes && mes.Length > 0)
                    {
                        BenjasMath.cycle(ref mesi, mes.Length-1);
                        all[j].meshi.mesh = meshes[mes[mesi]];
                    }
                    if (changePositions && pos.Length > 0)
                    {
                        BenjasMath.cycle(ref posi, pos.Length-1);
                        all[j].transform.localPosition = all[j].posOriginal + posOffsets[pos[posi]];
                    }
                    if (changeRotations && rot.Length > 0)
                    {

                        BenjasMath.cycle(ref roti, rot.Length-1);
                        all[j].transform.localEulerAngles = all[j].rotOriginal + rotOffsets[rot[roti]];
                    }
                }
            }
        }

    }



	// Use this for initialization
	void Awake () {
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
