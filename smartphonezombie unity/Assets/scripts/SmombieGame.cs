﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO.Compression;
using System;
//using IGP;

public class SmombieGame : MonoBehaviour {

    private static SmombieGame instance;

    [Header("quests")]
    public SmombieQuestManager questControl;
    public SmombieFinale finaleControl;
    
    public randomAppearanceManager_benja[] cityAppearance;
    public SmombieDog doggy;
    public SmombiePiano piano;
    public SmombieDrawing drawing;
    public DebugInfo_benja debugInfo;
    public bool debug = false;
    public bool godModeNoCrashes = false;
    public delegate void boolDelegate(bool theBool);
    public boolDelegate onDebugChange;

    [Header("path")]
    public recordAndPlayPath_Benja pathControl;
    public float speed;
    public float speedSmoothing = 0.5f;
    public float pathProgress;
    public float setSpeedTarget;
    public float overrideSpeedTarget = 0;

    [Header("audio")]
    public string audioFolder = "";

    [Header("timings")]
    public float gametimeBeforeTimeout = 210.0f;          //maximale zeit des spiels
    public float gametimeBeforeFriends = 180.0f;          //time before friends gone
    public float gameTime = 0;                   //zeit des spiels (starts at zero)
    public bool pausing = false;                // pauses the game
    public bool friendsWaiting = true;

    public bool dogAnoying = false;
    public float dogAnoyTime = 0;
    public float dogAnoyTimeMax = 3f;

    public bool handleDelay = false;
    public float delayTimeOnCrash = 7f;
    public float delayTime;

    public bool prepareCrashFade = false;
    public float delayCrashFade = 3f;
    public float fadeDelayTime = 0f;

    public float finishReachedAtMeter = 243;   //no changes to friends accepted after this, otherwise appear or disappear visible

    public STATE state;

    public event EventHandler<string> callback;

    public enum STATE
    {
        NONE,
        RESETTING,          //
        READY,              //
        ATSTART,            //0300
        PLAYING,            //0400 - 1300
        FINISH_CRASH,       //2300
        FINISH_FRIENDS,     //2310
        FINISH_NO_FRIENDS,  //2320
        FINISH_TIMEOUT      //2330
    }

   

    /// <summary>
    /// will change the current state to the given
    /// </summary>
    /// <param name="newState"></param>
    void setState(STATE newState)
    {
        state = newState;
        debugInfo.log("state", state.ToString());
    }
        
    public static SmombieGame GetInstance()
    {
        return instance;
    }
    // Use this for initialization
    void Start()
    {
        instance = this;

        Application.runInBackground = true;

        gametimeBeforeTimeout   = (float)Configuration.GetInnerTextByTagName("gametimeBeforeTimeout", gametimeBeforeTimeout);
        gametimeBeforeFriends   = (float)Configuration.GetInnerTextByTagName("gametimeBeforeFriends", gametimeBeforeFriends);
        delayTimeOnCrash        = (float)Configuration.GetInnerTextByTagName("delayTimeOnCrash", delayTimeOnCrash);

        if (debugInfo == null) debugInfo = FindObjectOfType<DebugInfo_benja>();
        if (finaleControl == null) finaleControl = FindObjectOfType<SmombieFinale>();
        finaleControl.onReachedPointOfNoReturn = GAMEprepareFinale;
        finaleControl.onReachedFinale = GAMEfinaleReached;
        if (questControl == null) questControl = FindObjectOfType<SmombieQuestManager>();
        if (pathControl == null) pathControl = FindObjectOfType<recordAndPlayPath_Benja>();

        //questControl.setupAudio( Application.streamingAssetsPath + "/" + audioFolder );
        if (doggy == null) doggy = FindObjectOfType<SmombieDog>();
        doggy.Setup(audioFolder);
        if (piano == null) piano = FindObjectOfType<SmombiePiano>();
        if (drawing == null) drawing = FindObjectOfType<SmombieDrawing>();
        //piano.Setup(audioFolder);
        //audioFolder = Application.streamingAssetsPath + "/" + audioFolder ;
        cityAppearance = FindObjectsOfType<randomAppearanceManager_benja>();
        instance.GAMEreset();
        instance.setDebugState(false);

    }

    public void setDebugState(bool debugState)
    {
        debug = debugState;
        if(onDebugChange!=null) onDebugChange(debugState);
        debugInfo.setDebugState(debugState);
    }

    /// <summary>
    /// set back game to start, randomize appearance of city and randomly distribute quests
    /// </summary>
    public void GAMEreset()
    {
        instance.setState(STATE.RESETTING);
        
        foreach (randomAppearanceManager_benja ram in instance.cityAppearance)
        {
            ram.randomizeAppearance();
        }
        instance.questControl.Reset();
        instance.finaleControl.Reset();
        instance.pathControl.stopPlaying(true);

        instance.gameTime = 0;
        instance.speed = 0;
        instance.setSpeedTarget = 0;
        instance.overrideSpeed = false;

        instance.handleDelay = false;
        instance.delayTime = 0;

        instance.prepareCrashFade = false;
        instance.fadeDelayTime = 0f;

        instance.dogAnoying = false;
        instance.dogAnoyTime = 0;
        instance.doggy.DOGstop();
        instance.piano.Reset();
        instance.drawing.Reset();

        instance.setState(STATE.READY);
    }

    /// <summary>
    /// set game to start, game timer will not be running!
    /// </summary>
    public void GAMEstart()
    {
        setState(STATE.ATSTART);
    }

    /// <summary>
    /// erally start playing the game including game timer
    /// </summary>
    public void GAMEstartPlaying()
    {
        setState(STATE.PLAYING);

        pathControl.play();
    }

    /// <summary>
    /// delay on crash without game over
    /// </summary>
    public void GAMEdelay()
    {
        updateDog(false);
        delayTime = delayTimeOnCrash;
        handleDelay = true;
    }

    /// <summary>
    /// call the anoying dog
    /// </summary>
    public void GAMEdog()
    {
        dogAnoying = true;
        dogAnoyTime = dogAnoyTimeMax;
        finaleControl.dogNewFriend();
        doggy.DOGstart();
    }

    /// <summary>
    /// called when foto motive appears
    /// </summary>
    public void GAMEfotoEnter()
    {
        Debug.Log("handle foto quest enter here");
        if (callback != null)
        {
            callback(this, "fotoenter");
        }
    }

    /// <summary>
    /// called when foto motive disappears
    /// </summary>
    public void GAMEfotoExit()
    {
        Debug.Log("handle foto quest exit here");
        if (callback != null)
        {
            callback(this, "fotoexit");
        }
    }



    void updateDog(bool keepDogAnnoying = true)
    {

        dogAnoying = keepDogAnnoying ? !BenjasMath.countdownToZero(ref dogAnoyTime) : false;

        if (!dogAnoying)
        {
            doggy.DOGstop();
        }
    }

    /// <summary>
    /// make gadget screen wet
    /// </summary>
    public void GAMEwet()
    {
        if (callback != null)
        {
            callback(this, "13.20");
        }
    }

    public void sendCodeForFinalTextCollection(string code)
    {
        if (code != "")
        {
            Debug.Log("code " + code);
            if (callback != null)
            {
                callback(this, code);
            }
        }
    }

    /// <summary>
    /// game timer run down
    /// </summary>
    public void GAMEtimeout()
    {
        if (state == STATE.PLAYING)
        {

            setState(STATE.FINISH_TIMEOUT);
             updateDog(false);
            piano.PIANOstart();

            if (callback != null)
            {
                string code = "23.30";
                callback(this, code);
            }
        }

    }

    /// <summary>
    /// game over due to crash
    /// </summary>
    public void GAMEfinaleCrash()
    {
         updateDog(false);
        setState(STATE.FINISH_CRASH);
        if (callback != null)
        {
            string code = "23.00";
            callback(this, code);
        }
    }

    /// <summary>
    /// game over due to falling into the well drawing
    /// </summary>
    public void GAMEfinaleDrawing()
    {
        setState(STATE.FINISH_CRASH);
         updateDog(false);
        drawing.drown();
        if (callback != null)
        {
            string code = "23.00";
            callback(this, code);
        }
    }

    /// <summary>
    /// reach end, friends there, happy end
    /// </summary>
    public void GAMEfinaleFriends()
    {
        Debug.Log("FINALE: friends waiting");
        setState(STATE.FINISH_FRIENDS);
        if(callback != null)
        {
            callback(this, "23.10");
        }
    }

    /// <summary>
    /// reach end, friends gone
    /// </summary>
    public void GAMEfinaleNoFriends()
    {
        Debug.Log("FINALE: friends gone");
        setState(STATE.FINISH_NO_FRIENDS);
        if (callback != null)
        {
            callback(this, "23.20");
        }
    }

 

    /// <summary>
    /// uses this to set the speed in m/s
    /// </summary>
    /// <param name="metersPerSecond"></param>
    public void GAMEsetSpeed(float metersPerSecond)
    {
        setSpeedTarget = metersPerSecond;
    }

    /// <summary>
    /// get current speed in m/s
    /// </summary>
    /// <returns>speed in m/s</returns>
    public float GAMEgetSpeed()
    {
        return speed;
    }

    void updateSpeed()
    {

        speed = Mathf.Lerp(   overrideSpeed ? overrideSpeedTarget : setSpeedTarget
                            , speed
                            , speedSmoothing
                          );
        pathControl.speedInMPerS = speed;
    }

    public bool overrideSpeed = false;

    bool hotkeysToDebug = true;

    void cheatkeys()
    {
        if (hotkeysToDebug)
        {
            hotkeysToDebug = false;
            debugInfo.log(": CHEATKEYS", "",true);
            debugInfo.log("[ D ]", "toggle Debug",true);
            debugInfo.log("[ G ]", "toggle God Mode (no crashes)", true);
            debugInfo.log("[down] / [ up ]", "spped +/-",true);
            debugInfo.log("[ Q ]", "reset",true);
            debugInfo.log("[ W ]", "go to start",true);
            debugInfo.log("[ E ]", "start playing",true);
            debugInfo.log("[ R ]", "the dog", true);
            debugInfo.log("[ T ]", "timeout", true);
        }
        if (Input.GetKeyDown("d"))
        {
            setDebugState(!debug);
        }
        if (Input.GetKeyDown("down"))
        {
            GAMEsetSpeed(0);
        }
        if (Input.GetKey("up"))
        {
            if(Input.GetKey("left shift"))
            GAMEsetSpeed(6.99f);
            else
            GAMEsetSpeed(.99f);
        }
        if (Input.GetKeyDown("q"))
        {
            GAMEreset();
        }
        if (Input.GetKeyDown("w"))
        {
            GAMEstart();
        }
        if (Input.GetKeyDown("e"))
        {
            GAMEstartPlaying();
        }
        if (Input.GetKeyDown("r"))
        {
            GAMEdog();
        }
        if (Input.GetKeyDown("t"))
        {
            GAMEtimeout();
        }
        if (Input.GetKeyDown("g"))
        {
            godModeNoCrashes = !godModeNoCrashes;
            debugInfo.log("[ G ]", "toggle God Mode (no crashes) (state: "+ (godModeNoCrashes?"on":"off"), true);
        }

    }

    public void GAMEprepareFinale()
    {
        if (gameTime > gametimeBeforeFriends)
        {
                finaleControl.friendsLeave();
                friendsWaiting = false;
        }
        Debug.Log("PONR: friends " + (friendsWaiting ? "waiting":"gone"));
    }

    public void GAMEfinaleReached()
    {
      if (friendsWaiting) GAMEfinaleFriends();
      else GAMEfinaleNoFriends();
    }

    // Update is called once per frame
    void Update () {
        cheatkeys();
        debugInfo.log("game time", instance.gameTime);
        debugInfo.log("speed", instance.speed);
        //debugInfo.log("position (" + instance.pathControl.pathDistance + ")" , instance.pathControl.playheadPositionInM );
        
        //pathProgress = pathControl.playheadPosition01();
        if (state == STATE.PLAYING)
        {
            if( BenjasMath.timer(ref gameTime, gametimeBeforeTimeout, pausing)>=1 )
            {
                GAMEtimeout();
            }

            if (dogAnoying)
            {
                updateDog();
            }

            if (handleDelay)
            {
                handleDelay = !BenjasMath.countdownToZero(ref delayTime);
            }
            overrideSpeed = handleDelay ;
            updateSpeed();
        }
        if (state == STATE.FINISH_CRASH || state == STATE.FINISH_TIMEOUT)
        {
            overrideSpeed = true;
            updateSpeed();
         }

    }
}

