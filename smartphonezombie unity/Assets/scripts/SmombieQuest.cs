using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmombieQuest : MonoBehaviour {

    private const string gameIdPrefix = "0401";
    public string storyboardId  = "00"; ///04..12
    public string storyboardSubId = ""; //A,B,C...
    public QUESTTYPE questtype;
    public REACTION_FAIL reactionOnFail = REACTION_FAIL.NONE;
    public bool isMirrored = false;
    public bool isAnimated = false;
    public float animationMaxTime;
    public float animationTime;
    public delegate void handler(SmombieQuest quest);
    public handler onCrash;
    public handler onEnter;
    public handler onPass;

    public GameObject[] standbyQuad = new GameObject[1];
    public GameObject[] introQuad = new GameObject[1];

    public Transform animatedObject;
    public Transform animationStart;
    public Transform animationEnd;

    public GameObject[] failQuad = new GameObject[1];
    public GameObject[] passQuad = new GameObject[1];

    public SmombieSpawnPoint spawnPoint;
    private AudioSource Sound;
    public STATE state;

    public enum REACTION_FAIL
    {
        NONE,
        DELAY, //get delayed by 7 seconds
        WET, //wet screen for ... seconds
        DOG, //jumping dog
        FINALE_DRAWING, // fall into well before Finale crash
        FINALE_CRASH // end game 2300 unfall (accident)
    }

    public enum QUESTTYPE
    {
        NONE,
        STREET,
        CROSSING,
        CARRIER,
        HOUSE,
        FOTO
    }

    public enum STATE
    {
        NONE,
        STANDBY,
        INTRO,
        FAIL,
        PASS
    }

    public void spawnAt(SmombieSpawnPoint spawn)
    {
        spawnPoint = spawn;
        spawnPoint.soldToo = this;
       /* if (isFotoQuest)
        {
            transform.position = spawnPoint.fotoSpawnPoint.transform.position;
            transform.rotation = spawnPoint.fotoSpawnPoint.transform.rotation;
        }
        else*/
        
            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;
        
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


        
        reset();
        gameObject.SetActive(true);
        setState(STATE.STANDBY);
    }

    public void reset()
    {
        gameObject.SetActive(false);
        stopAnimation();
        if (storyboardId == "0000") Debug.LogError("set storyboard id in " + gameObject.name);
        if (isAnimated)
        {
            isAnimating = false;
            animationEnd.gameObject.SetActive(false);
            animationStart.gameObject.SetActive(false);
        }
        if (Sound == null)
        {
            Sound = gameObject.AddComponent<AudioSource>();
        }
        Sound.Stop();
       
    }

    public void setState(STATE newState)
    {
        state = newState;
        //make all objects visible or invisible depending on their state

        foreach (GameObject quad in standbyQuad) if (quad != null) quad.SetActive(false);

        foreach (GameObject quad in introQuad) if (quad != null) quad.SetActive(false);

        foreach (GameObject quad in failQuad) if (quad != null) quad.SetActive(false);

        foreach (GameObject quad in passQuad) if (quad != null) quad.SetActive(false);

        switch (state)
        {

            case STATE.STANDBY:
                foreach (GameObject quad in standbyQuad) if (quad != null) quad.SetActive(true);
                break;
            case STATE.INTRO:
                foreach (GameObject quad in introQuad) if (quad != null) quad.SetActive(true);
                break;
            case STATE.FAIL:
                foreach (GameObject quad in failQuad) if (quad != null) quad.SetActive(true);
                break;
            case STATE.PASS:
                foreach (GameObject quad in passQuad) if (quad != null) quad.SetActive(true);
                break;
        }


    }

    public void playAudio(string stateID)
    {
        // 0401.S.0530.A
        string audiofile = SmombieGame.GetInstance().audioFolder;
        audiofile += "0401.S." + storyboardId + stateID;
        if (storyboardSubId.Length > 0)
        {
            audiofile += "." + storyboardSubId;
        }
    }


    public void handleCrash()
    {
        if(state != STATE.PASS)
        setState(STATE.FAIL);
        stopAnimation();
        onCrash(this);
    }

    public void handlePass()
    {
        if (state != STATE.FAIL)
            setState(STATE.PASS);
        stopAnimation();
        onPass(this);
    }


    public void handleActivation()
    {
        setState(STATE.INTRO);
        if(isAnimated)
        {
            startAnimation();
        }
        onEnter(this);
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
            handlePass();
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
