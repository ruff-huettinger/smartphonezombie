﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmombieQuest : MonoBehaviour {

    public bool isStreetQuest = false;
    public bool isCrossingQuest = false;
    public bool isCarrierQuest = false;
    public bool isHouseQuest = false;
    public bool isFotoQuest = false;
    public bool isMirrored = false;
    public bool isAnimated = false;
    public int state = -1; //0 = ready, 1= intro (animation), 2= fail, 3= pass;
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
    public SmombieSpawnPoint spawnPoint;

    public void spawnAt(SmombieSpawnPoint spawn)
    {
        spawnPoint = spawn;
        state = -1;
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
        // take care of textures and motions beeing mirrored
        if (isMirrored != spawnPoint.isRightHandSide)
        {
            Vector3 ls = gameObject.transform.localScale;
            ls.z *= -1;
            gameObject.transform.localScale = ls;
            isMirrored = spawnPoint.isRightHandSide;
        }

        hasSpawned = true;
        reset();
        gameObject.SetActive(true);
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
        /*
        foreach (GameObject quad in standbyQuad)
        {
            if (quad != null) quad.SetActive(false);
        }
        foreach (GameObject quad in introQuad)
        {
            if (quad != null) quad.SetActive(false);
        }
        foreach (GameObject quad in failQuad)
        {
            if (quad != null) quad.SetActive(false);
        }
        foreach (GameObject quad in passQuad)
        {
            if (quad != null) quad.SetActive(false);
        }
        */
        switch (newState)
        {
            case 0:
                foreach (GameObject quad in standbyQuad)
                {
                    if (quad != null) quad.SetActive(true);
                }
                break;
            case 1:
                foreach (GameObject quad in introQuad)
                {
                    if (quad != null) quad.SetActive(true);
                }
                break;
            case 2:
                foreach (GameObject quad in failQuad)
                {
                    if (quad != null) quad.SetActive(true);
                }
                break;
            case 3:
                foreach (GameObject quad in passQuad)
                {
                    if (quad != null) quad.SetActive(true);
                }
                break;

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
