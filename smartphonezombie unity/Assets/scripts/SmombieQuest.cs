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
    public int state = 0; //0 = ready, 1= intro (animation), 2= fail, 3= pass;
    public GameObject[] standbyQuad = new GameObject[1];
    public GameObject[] introQuad = new GameObject[1];
    public GameObject[] failQuad = new GameObject[1];
    public GameObject[] passQuad = new GameObject[1];
    public Transform animatedObject;
    public Transform animationStart;
    public Transform animationEnd;
    public float animationMaxTime;
    public float animationTime;
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
        reset();
    }

    public void handleCrash()
    {
        setState(2);
        stopAnimation();
    }

    public void handleWait()
    {
        setState(3);
        stopAnimation();
    }

    public void reset()
    {
        setState(0);
        stopAnimation();
    }

    public void setState(int newState)
    {
        state = newState;
        //make all objects visible or invisible depending on their state
        foreach (GameObject quad in standbyQuad)
        {
            if (quad != null) quad.SetActive(state == 0);
        }
        foreach (GameObject quad in introQuad)
        {
            if (quad != null) quad.SetActive(state == 1);
        }
        foreach (GameObject quad in failQuad)
        {
            if (quad != null) quad.SetActive(state == 2);
        }
        foreach (GameObject quad in passQuad)
        {
            if (quad != null) quad.SetActive(state == 3);
        }
    }

    public void handleActivation()
    {
        if(isAnimated)
        {
            startAnimation();
        }
    }

    private bool isAnimating = false;
    void startAnimation()
    {
        isAnimating = true;
        animationTime = animationMaxTime;
        setAnimationPosition(animationStart.position);
    }

    void doAnimation()
    {
        if (BenjasMath.countdownToZero(ref animationTime))
        {
            setAnimationPosition(animationEnd.position);
            stopAnimation();
            handleWait();
        }
        else
        {
            //lerp from end to start because we count down
            setAnimationPosition(Vector3.Lerp(animationEnd.position, animationStart.position, animationTime / animationMaxTime));
        }
    }

    public void setAnimationPosition(Vector3 position)
    {
        if (animatedObject != null)
        {
            animatedObject.position = position;
        }
    }

    void stopAnimation()
    {
        isAnimating = false;
    }

    void Update()
    {
        if (isAnimating) doAnimation();
    }
}
