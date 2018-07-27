using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmombieQuest : MonoBehaviour {

    public bool isStreetQuest = false;
    public bool isCornerQuest = false;
    public bool isHouseQuest = false;
    public bool isFotoQuest = false;
    public bool isMirrored = false;
    public bool isAnimated = false;
    public GameObject introQuad;
    public GameObject passQuad;
    public GameObject outroQuad;
    public bool hasSpawned = false;

    public void spawnAt(SmombieSpawnPoint spawnPoint)
    {
        if (isFotoQuest)
        {
            transform.position = spawnPoint.fotoSpawnPoint.transform.position;
            transform.rotation = spawnPoint.fotoSpawnPoint.transform.rotation;
        }
        else
        {
            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;
        }
        spawnPoint.activationTrigger.onTrigger = handleActivation; 
        spawnPoint.crashTrigger.onTrigger = handleCrash;
        if (spawnPoint.isRightHandSide)
        {
            isMirrored = true;
            // take care of textures and motions beeing mirrored
        }
        hasSpawned = true;
    }

    public void handleCrash()
    {

    }

    public void handleActivation()
    {
        if(isAnimated)
        {
            startAnimation();
        }
    }

    void startAnimation()
    {

    }


}
