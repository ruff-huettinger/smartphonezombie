using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmombieQuest : MonoBehaviour {

   
    private const string gameIdPrefix = "0401";
    [Header("storyboard codes")]
    public string storyboardId = "00"; ///04..12
    public string storyboardSubId = ""; //A,B,C...
    public string stateIdIntro ;
    public string stateIdRun;
    public string stateIdPass;
    public string stateIdFail;
    public AudioClip soundIntro;
    public bool loopIntro = false;
    public AudioClip soundPass;
    public AudioClip soundFail;
    private AudioSource audioSource;
    AudioLoader_benja[] audioIntro;
    AudioLoader_benja[] audioPass;
    AudioLoader_benja[] audioFail;
    //public string codeForFinaleTextOnPass = "";
    //public string codeForFinaleTextOnFail = "";
    //public string codeForFinaleTextOnRun = "";



    [Header("quest definition")]
    public QUESTTYPE questtype;
    public REACTION_FAIL reactionOnFail = REACTION_FAIL.NONE;


    [Header("timings and details")]
    public float crashMinimumWalkingSpeed = 0f;
    public float delayIntroAfterActivation = 1f;
    public float timeUntilIntro;
    public float maxTimeUntilPass = 3f;
    public float timeUntilPass = 0f;
    bool isMirrored = false;

    public ANI startsAnimation = ANI.NEVER;
    public float animationMaxTime;
    public float animationTime;
    public Transform animatedObject;
    public Transform animationStart;
    public Transform animationEnd;

    public delegate void handler(SmombieQuest quest);
    public handler onCrash;
    public handler onEnter;
    public handler onPass;

    public delegate float floatDelegate();
    public floatDelegate getCrashSpeed;

    [Header("objects to be seen")]
    public GameObject[] standbyQuad = new GameObject[1];
    /// <summary>
    /// test
    /// </summary>
    public GameObject[] introQuad = new GameObject[1];
    public GameObject[] failQuad = new GameObject[1];
    public GameObject[] passQuad = new GameObject[1];


    [Header("info - dont touch")]
    public SmombieSpawnPoint spawnPoint;
    private AudioSource[] Sounds;
    public STATE state;
    public string codeForFinaleText = "";

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

    public enum ANI
    {
        NEVER,
        ONINTRO,
        ONPASS,
        ONFAIL
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

    private void Start()
    {
        setupAudio();
    }

    public void spawnAt(SmombieSpawnPoint spawn)
    {
        spawnPoint = spawn;
        spawnPoint.soldToo = this;

        transform.position = spawnPoint.transform.position;
        transform.rotation = spawnPoint.transform.rotation;

        spawnPoint.activationTrigger.onTrigger = handleActivation;
        spawnPoint.passTrigger.onTrigger = handleRun;


        // take care of textures and motions beeing mirrored
        if (isMirrored != spawnPoint.isRightHandSide)
        {
            Vector3 ls = gameObject.transform.localScale;
            ls.z *= -1;
            gameObject.transform.localScale = ls;
            isMirrored = spawnPoint.isRightHandSide;
        }

        if (questtype != QUESTTYPE.FOTO)
        {
            TriggerChecker CrashTrigger = GetComponentInChildren<TriggerChecker>();
            if (CrashTrigger != null && CrashTrigger.gameObject.name == "crashTrigger")
                CrashTrigger.onTrigger = handleCrash;
            else
                Debug.Log("//////////////// Warning, quest lacks crashTrigger //////////////////////");
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
        
        if (storyboardId == "0000") Debug.LogError("set storyboard id in " + gameObject.name);
        if (startsAnimation != ANI.NEVER)
        {
            animationEnd.gameObject.SetActive(false);
            animationStart.gameObject.SetActive(false);
            stopAnimation(true);
        }
        stopAudio();
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
            stopAudio();
            playaudio(soundIntro, loopIntro);
        }
        else if(state == STATE.FAIL)
        {
             foreach (GameObject quad in failQuad) if (quad != null) quad.SetActive(true);
            stopAudio();
            playaudio(soundFail);
        }
        else if(state == STATE.PASS)
        {
            foreach (GameObject quad in passQuad) if (quad != null) quad.SetActive(true);
            stopAudio();
            playaudio(soundPass);
        }
    }



    private string storryboardCodeFile(string stateID)
    {
        // 0401.S.0530.A
        string audiofile = "0401_S_" + storyboardId + stateID;
        if (storyboardSubId.Length > 0)
        {
            audiofile += storyboardSubId;
        }
        return audiofile;
    }

    private string getCodeForFinalText(string stateID)
    {
        // 0401.S.0530.A
        string code = storyboardId +"."+ stateID;
        if (storyboardSubId.Length > 0)
        {
            code += storyboardSubId;
        }
        return code;
    }

    private string storryboardCode(string stateID)
    {
        // 0401.S.0530.A
        string audiofile = "0401.S." + storyboardId + stateID;
        if (storyboardSubId.Length > 0)
        {
            audiofile += "." + storyboardSubId;
        }
        return audiofile;
    }

    /*
    public void playaudio(AudioLoader_benja[] loaders, bool loop = false)
    {
        stopAudio();
        if (loaders != null)
        {
            //Debug.Log("//////starting audio");
            for (int i = 0; i < loaders.Length && i < Sounds.Length; i++)
            {

                Sounds[i].clip = loaders[i].audioClip;
                //Sounds[i].loop = true;
                Sounds[i].Play();
            }
        }
    }*/

    public void playaudio(AudioClip clip, bool loop = false)
    {
        stopAudio();
        if (clip != null)
        {
            //Debug.Log("//////starting audio");
            audioSource.clip = clip;
            audioSource.loop = loop;
            audioSource.Play();
        }
    }

    public void stopAudio()
    {
        if (Sounds != null) foreach (AudioSource sound in Sounds) sound.Stop();
        if (audioSource != null) audioSource.Stop();
    }

    public void setupAudio()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }
    /*
    public void setupAudio(string AudioFolder)
    {

        if (stateIdIntro.Length == 2) audioIntro = Helpers_benja.findAudioClips(AudioFolder, storryboardCodeFile(stateIdIntro)+"*.wav" ,gameObject);
        if (stateIdPass.Length == 2) audioPass = Helpers_benja.findAudioClips(AudioFolder, storryboardCodeFile(stateIdPass) + "*.wav",gameObject);
        if (stateIdFail.Length == 2) audioFail = Helpers_benja.findAudioClips(AudioFolder, storryboardCodeFile(stateIdFail) + "*.wav",gameObject);
        int audioCounter = 0;

        if ( audioIntro!= null) audioCounter = Mathf.Max(audioCounter, audioIntro.Length);
        if ( audioPass != null)audioCounter = Mathf.Max(audioCounter, audioPass.Length);
        if ( audioFail != null) audioCounter = Mathf.Max(audioCounter, audioFail.Length);
        if (audioCounter > 0)
        {
            Debug.Log(gameObject.name + " has "+ audioCounter + " sounds");
            Sounds = new AudioSource[audioCounter];
            for (int i = 0; i < audioCounter; i++)
            {
                Sounds[i] = gameObject.AddComponent<AudioSource>();
                Sounds[i].playOnAwake = false;
            }
        }
        else Debug.Log(gameObject.name + " has no sounds");
    }
    */



    public void handleCrash()
    {
        if (state == STATE.INTRO || state == STATE.ACTIVATION)
        {
            if (getCrashSpeed()!= null && getCrashSpeed() < crashMinimumWalkingSpeed)
            {
                //there are quests that can be done by walking slow
                handlePass();
                return;
            }


            setState(STATE.FAIL);
            if (startsAnimation == ANI.ONFAIL)
            {
                startAnimation();
            }
            // else stopAnimation();
            //codeForFinaleText = codeForFinaleTextOnFail + storyboardSubId;
            codeForFinaleText = getCodeForFinalText(stateIdFail);
            onCrash(this);
        }
        
    }

    public void handlePass()
    {
        if (state == STATE.INTRO || state == STATE.ACTIVATION)
        {
            setState(STATE.PASS);
            if (startsAnimation == ANI.ONPASS)
            {
                startAnimation();
            }
            // else stopAnimation();
            spawnPoint.passTrigger.onTrigger = null;
            //codeForFinaleText = codeForFinaleTextOnPass;
            codeForFinaleText = getCodeForFinalText(stateIdPass);
            onPass(this);
        }

    }

    public void handleRun()
    {
        if (state == STATE.INTRO || state == STATE.ACTIVATION)
        {
            setState(STATE.PASS);
            if (startsAnimation == ANI.ONPASS)
            {
                startAnimation();
            }
            // else stopAnimation();
            spawnPoint.passTrigger.onTrigger = null;
            //codeForFinaleText = codeForFinaleTextOnRun;
            codeForFinaleText = getCodeForFinalText(stateIdRun);
            onPass(this);
        }
    }


    public void handleActivation()
    {
        if (state == STATE.STANDBY)
        {
            setState(STATE.ACTIVATION);
            codeForFinaleText = "";
            spawnPoint.activationTrigger.onTrigger = null;
            onEnter(this);

        }
        
        
    }

    public void handleIntro()
    {
        setState(STATE.INTRO);
        if (startsAnimation == ANI.ONINTRO)
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
