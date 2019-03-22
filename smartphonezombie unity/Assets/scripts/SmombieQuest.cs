using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmombieQuest : MonoBehaviour {

   
    private const string gameIdPrefix = "0401";
    [Header("storyboard codes")]
    public string storyboardId = "00"; ///04.12
    public string storyboardSubId = ""; //A,B,C...
    public string stateIdIntro ;
    public string stateIdRun;
    public string stateIdPass;
    public string stateIdFail;

    //public string codeForFinaleTextOnPass = "";
    //public string codeForFinaleTextOnFail = "";
    //public string codeForFinaleTextOnRun = "";



    [Header("quest definition")]
    public QUESTTYPE questtype;
    public REACTION_FAIL reactionOnFail = REACTION_FAIL.NONE;
    public renderTextureSnapshot questPhotoCam;


    [Header("timings and details")]
    public float crashMinimumWalkingSpeed = 0f;
    public float maxTimeUntilPass = 3f;
    public float timeUntilPass = 0f;
    bool isMirrored = false;

    public delegate void handler(SmombieQuest quest);
    public handler onCrash;
    public handler onEnter;
    public handler onPass;

    public delegate float floatDelegate();
    public floatDelegate getCrashSpeed;

    [Header("objects to be seen")]
    public GameObject[] standbyObject = new GameObject[1];
    /// <summary>
    /// test
    /// </summary>
    public GameObject[] introObject = new GameObject[1];
    public GameObject[] passObject = new GameObject[1];
    public GameObject[] failObject = new GameObject[1];
    public GameObject[] continueAfterFailObject = new GameObject[1];


    [Header("info - dont touch")]
    public SmombieSpawnPoint spawnPoint;
    private AudioSource[] Sounds;
    public STATE state;
    public string codeForFinaleText = "";
    public string finaleSnapshotFilePath = "";

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
        PASS,
        FAIL,
        CONTINUE
    }

    private void Start()
    {
        foreach (GameObject obj in standbyObject) if (obj != null) obj.AddComponent<setActiveOnUpdate>();
        foreach (GameObject obj in introObject) if (obj != null) obj.AddComponent<setActiveOnUpdate>();
        foreach (GameObject obj in passObject) if (obj != null) obj.AddComponent<setActiveOnUpdate>();
        foreach (GameObject obj in failObject) if (obj != null) obj.AddComponent<setActiveOnUpdate>();
        foreach (GameObject obj in continueAfterFailObject) if (obj != null) obj.AddComponent<setActiveOnUpdate>();
        if (questPhotoCam == null) questPhotoCam = GetComponentInChildren <renderTextureSnapshot>();
        questPhotoCam.fileName="finaleSnapshot_"+gameObject.name;
        questPhotoCam.timestamp = false;
        questPhotoCam.keepCamDisabled = true;
    }

    public void spawnAt(SmombieSpawnPoint spawn)
    {
        spawnPoint = spawn;
        spawnPoint.soldToo = this;

        transform.position = spawnPoint.transform.position;
        transform.rotation = spawnPoint.transform.rotation;

        spawnPoint.activationTrigger.onTrigger = handleIntro;
        spawnPoint.passTrigger.onTrigger = handleRun;


        // take care of textures and motions beeing mirrored
        if (isMirrored != spawnPoint.isRightHandSide)
        {
            Vector3 ls = gameObject.transform.localScale;
            ls.z *= -1;
            gameObject.transform.localScale = ls;
            questPhotoCam.theCam.transform.Rotate(0, 180, 0);
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
        if (storyboardId == "0000") Debug.LogError("set storyboard id in " + gameObject.name);
    }

    void set(GameObject[]go, bool to)
    {
        foreach (GameObject obj in go)
            if (obj != null)
            {
                setActiveOnUpdate setter = obj.GetComponent<setActiveOnUpdate>();
                if (setter != null)
                    setter.set(to);
            }
    }

    void set(GameObject[] go)
    {
        foreach (GameObject obj in go)
            if (obj != null)
            {
                setActiveOnUpdate setter = obj.GetComponent<setActiveOnUpdate>();
                if (setter != null)
                    setter.set();
            }
    }

    public void setState(STATE newState)
    {
        state = newState;
        //make all objects visible or invisible depending on their state


        set( standbyObject,false); 
        set( introObject,false);
        set( passObject,false);
        set( failObject,false);
        set( continueAfterFailObject,false);

        if (state == STATE.STANDBY)
        {
            set(standbyObject,true);
        }
        else if (state == STATE.INTRO)
        { 
            set(introObject,true);
        }
        else if (state == STATE.PASS)
        {
            set(passObject,true);
        }
        else if(state == STATE.FAIL)
        {
             set(failObject,true);
        }
        else if (state == STATE.CONTINUE)
        {
            set(continueAfterFailObject,true);
        }

        set( standbyObject);
        set( introObject);
        set( passObject);
        set( failObject);
        set( continueAfterFailObject);
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


    public void handleCrash()
    {
        if (state == STATE.INTRO )
        {
            if (getCrashSpeed()!= null && getCrashSpeed() < crashMinimumWalkingSpeed)
            {
                //there are quests that can be done by walking slow
                handlePass();
                return;
            }


            setState(STATE.FAIL);
            //codeForFinaleText = codeForFinaleTextOnFail + storyboardSubId;
            codeForFinaleText = getCodeForFinalText(stateIdFail);
            finaleSnapshotFilePath = questPhotoCam.takeSnapShot();
            onCrash(this);
        }
        
    }

    public void handleContinue()
    {
        if (state == STATE.FAIL )
        {
            setState(STATE.CONTINUE);
        }

    }

    public void handlePass()
    {
        if (state == STATE.INTRO )
        {
            setState(STATE.PASS);
            spawnPoint.passTrigger.onTrigger = null;
            //codeForFinaleText = codeForFinaleTextOnPass;
            codeForFinaleText = getCodeForFinalText(stateIdPass);
            finaleSnapshotFilePath = questPhotoCam.takeSnapShot();
            onPass(this);
        }

    }

    public void handleRun()
    {
        if (state == STATE.INTRO )
        {
            setState(STATE.PASS);
            spawnPoint.passTrigger.onTrigger = null;
            //codeForFinaleText = codeForFinaleTextOnRun;
            codeForFinaleText = getCodeForFinalText(stateIdRun);
            finaleSnapshotFilePath = questPhotoCam.takeSnapShot();
            onPass(this);
        }
    }




    public void handleIntro()
    {
        setState(STATE.INTRO);
        codeForFinaleText = "";
        finaleSnapshotFilePath = "";
        spawnPoint.activationTrigger.onTrigger = null;
        onEnter(this);
    }



    void Update()
    {
        
        if (state == STATE.INTRO)
        {
            //wait for maxTimeUntilPass before going on 
            if (BenjasMath.countdownToZero(ref timeUntilPass))
            {
                handlePass();
            }
        }

    }
}
