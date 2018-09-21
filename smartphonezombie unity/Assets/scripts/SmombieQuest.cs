using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmombieQuest : MonoBehaviour {

    private const string gameIdPrefix = "0401";
    public string storyboardId = "00"; ///04..12
    public string storyboardSubId = ""; //A,B,C...
    public QUESTTYPE questtype;
    public REACTION_FAIL reactionOnFail = REACTION_FAIL.NONE;
    public float delayIntroAfterActivation = 1f;
    public float timeUntilIntro;
    public float maxTimeUntilPass = 3f;
    public float timeUntilPass = 0f;
    public bool isMirrored = false;
    public bool isAnimated = false;
    public float animationMaxTime;
    public float animationTime;
    public delegate void handler(SmombieQuest quest);
    public handler onCrash;
    public handler onEnter;
    public handler onPass;

    public GameObject[] standbyQuad = new GameObject[1];
    /// <summary>
    /// test
    /// </summary>
    public GameObject[] introQuad = new GameObject[1];
    public GameObject[] failQuad = new GameObject[1];
    public GameObject[] passQuad = new GameObject[1];
    public Transform animatedObject;
    public Transform animationStart;
    public Transform animationEnd;

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
        ACTIVATION,
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
        timeUntilPass = maxTimeUntilPass;
        timeUntilIntro = delayIntroAfterActivation;
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

        if (state == STATE.STANDBY)
        {
            foreach (GameObject quad in standbyQuad) if (quad != null) quad.SetActive(true);
        }
        else if (state == STATE.INTRO || state == STATE.ACTIVATION)
        { 
                foreach (GameObject quad in introQuad) if (quad != null) quad.SetActive(true);
                }
        else if(state == STATE.FAIL)
            {
                foreach (GameObject quad in failQuad) if (quad != null) quad.SetActive(true);
                }
        else if(state == STATE.PASS)
                {
                    foreach (GameObject quad in passQuad) if (quad != null) quad.SetActive(true);
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
        if (state == STATE.INTRO || state == STATE.ACTIVATION)
        {
            setState(STATE.FAIL);
            //stopAnimation();
            onCrash(this);
        }
    }

    public void handlePass()
    {
        if (state == STATE.INTRO || state == STATE.ACTIVATION)
        {
            setState(STATE.PASS);
            //stopAnimation();
            onPass(this);
        }
    }


    public void handleActivation()
    {
        setState(STATE.ACTIVATION);
        onEnter(this);

    }

    public void handleIntro()
    {
        setState(STATE.INTRO);
        if (isAnimated)
        {
            startAnimation();
        }
    }

    private bool isAnimating = false;
    void startAnimation()
    {
        isAnimating = true;
        animationTime = 0;
        setAnimationPosition(0);
    }

    void doAnimation()
    {
        if (setAnimationPosition(BenjasMath.timer(ref animationTime, animationMaxTime)))
        {
            stopAnimation();
        }
    }

    

    public bool setAnimationPosition(float t)
    {
        if (animatedObject != null)
        {
            Mathf.Clamp01(t);
            animatedObject.position = Vector3.Lerp( animationStart.position, animationEnd.position, t);
        }
        return t == 1; 
    }

    void stopAnimation(bool reset = false)
    {
        isAnimating = false;
        if (reset) setAnimationPosition(0);

    }

    void Update()
    {
        
        if (state == STATE.ACTIVATION)
        {
            //wait for delayIntroAfterActivation before going on 
            if (BenjasMath.countdownToZero(ref timeUntilIntro))
            {
                handleIntro();
            }
        }

        if (state == STATE.INTRO)
        {
            //wait for maxTimeUntilPass before going on 
            if (BenjasMath.countdownToZero(ref timeUntilPass))
            {
                handlePass();
            }
        }

        if(isAnimating && (state == STATE.INTRO || state == STATE.PASS || state == STATE.FAIL))
        {
            doAnimation();
        }
    }
}
