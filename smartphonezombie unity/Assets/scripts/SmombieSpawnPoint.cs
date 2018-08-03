using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmombieSpawnPoint : MonoBehaviour {

    public bool isRightHandSide = false;
    public bool acceptsStreetQuest = false;
    public bool acceptsCrossingQuest = false;
    public bool acceptsCarrierQuest = false;
    public bool acceptsHouseQuest = false;
    public bool acceptsFotoQuest = false;
    public TriggerChecker activationTrigger;
    public TriggerChecker crashTrigger;
    public Transform fotoSpawnPoint;

    //place this spawnpoint exactly where the quest should spawn
    //if it is a foto quest it will use the fotoSpawnPoint instead which is "near the horrizon"

    // Use this for initialization
    void Start () {
		if(activationTrigger == null || crashTrigger == null || fotoSpawnPoint == null)
        {
            Debug.LogError("something could not be found in this SmombieSpawnPoint: please apply SpawnPoints and Triggers per Inspector");
        }
	}
	
}
