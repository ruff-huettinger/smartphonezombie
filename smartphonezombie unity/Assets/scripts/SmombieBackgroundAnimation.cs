using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmombieBackgroundAnimation : MonoBehaviour {


    public bool isMirrored = false;
    public bool keepSpawning = true;
    //how far away should the object be before spawning next;
    public float minSpawnDistance = 7f; 
    public float maxSpawnDistance = 9f;
    float spawnDistance = 0f;
    public List<RandomAppearence_benja> copies = new List<RandomAppearence_benja>();
    public List<float> animationTimes = new List<float>();
    public RandomAppearence_benja animatedObject;
    public Transform animationStart;
    public Transform animationEnd;
     float animationMaxTimeChanging;
    float animationTimePhase = 0;
    public float animationMaxTime;

    public TriggerChecker deactivationTrigger;


    public void Start()
    {
        deactivationTrigger.onTrigger = handleDeactivation;
        // take care of textures and motions beeing mirrored
        if (isMirrored )
        {
            Vector3 ls = gameObject.transform.localScale;
            ls.z *= -1;
            gameObject.transform.localScale = ls;
        }
        animatedObject.gameObject.SetActive(false);
        animationStart.gameObject.SetActive(false);
        animationEnd.gameObject.SetActive(false);
        reset();
        animationTimePhase = Random.value * 2 * Mathf.PI;
    }

    public void handleDeactivation()
    {
        keepSpawning = false;
    }

    public void reset()
    {
        keepSpawning = true;
    }


    void spawnCopy()
    {
        RandomAppearence_benja copy = Instantiate(animatedObject.gameObject.GetComponent<RandomAppearence_benja>());
        copy.gameObject.SetActive(true);
        copy.transform.parent = animatedObject.transform.parent;
        copy.transform.position = animationStart.position;
        copy.transform.rotation = animatedObject.transform.rotation;
        copy.transform.localScale = animatedObject.transform.localScale;
        copy.randomizeAppearance();
        copies.Add(copy);
        animationTimes.Add(animationMaxTimeChanging);
        spawnDistance = (Mathf.Lerp(minSpawnDistance, maxSpawnDistance, Random.value));
    }

    void killCopy(int i)
    {
        Destroy(copies[i].gameObject);
        copies.RemoveAt(i);
        animationTimes.RemoveAt(i);
    }

    void doAnimation()
    {

    }

    public float minDistToSpawnPoint = 0;
    void Update()
    {
        animationMaxTimeChanging = animationMaxTime * (1 + 0.1f * Mathf.Sin(animationTimePhase+ Time.realtimeSinceStartup  ));
        minDistToSpawnPoint = Vector3.Distance(animationEnd.position, animationStart.position);

            
            for (int i = 0; i < copies.Count; i++)
            {
                float t = animationTimes[i];
                if (BenjasMath.countdownToZero(ref t))
                {
                    killCopy(i);
                }
                else
                {
                    animationTimes[i] = t;
                    t /= animationMaxTimeChanging;
                    copies[i].transform.position = Vector3.Lerp(animationEnd.position, animationStart.position, t);
                    minDistToSpawnPoint = Mathf.Min(minDistToSpawnPoint, Vector3.Distance(copies[i].transform.position, animationStart.position));
                }
            }

        if (keepSpawning && minDistToSpawnPoint >= spawnDistance)
        {
            spawnCopy();
        }

    }
}
