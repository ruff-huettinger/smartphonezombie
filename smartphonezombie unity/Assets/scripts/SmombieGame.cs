using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using IGP;

public class SmombieGame : MonoBehaviour {

    private static SmombieGame instance;
    public SmombieQuestManager questControl;
    public recordAndPlayPath_Benja pathControl;
    public string audioFolder = "";
    public float speed;
    public float setSpeedTarget;
    float overrideSpeedTarget = 0;
    public bool delayingPlayer = false;
    public float delayOnCrash = 7f;
    public float delayTime;
    public bool prepareCrashFade = false;
    public float delayCrashFade = 3f;
    public float fadeDelayTime = 0f;
    public float speedSmoothing = 0.5f;
    public float pathProgress;
    public randomAppearanceManager_benja[] cityAppearance;
    public DebugInfo_benja debugInfo;
    public bool debug = false;

    public delegate void boolDelegate(bool theBool);
    public boolDelegate onDebugChange;
    public float maxGameTime = 180.0f;          //maximale zeit des spiels 
    public float gameTime = 0;                   //zeit des spiels (countdown)
    public bool pausing = false;                // pauses the game
    public STATE state;


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
    void changestate(STATE newState)
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
        debugInfo = FindObjectOfType<DebugInfo_benja>();
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
        instance.changestate(STATE.RESETTING);
        
        foreach (randomAppearanceManager_benja ram in instance.cityAppearance)
        {
            ram.randomizeAppearance();
        }
        instance.questControl.Reset();
        instance.pathControl.stopPlaying(true);

        
        

        instance.gameTime = 0;
        instance.speed = 0;
        instance.setSpeedTarget = 0;

        instance.delayingPlayer = false;
        instance.delayTime = 0;

        instance.prepareCrashFade = false;
        instance.fadeDelayTime = 0f;

        instance.changestate(STATE.READY);
    }

    /// <summary>
    /// set game to start, game timer will not be running!
    /// </summary>
    public void GAMEstart()
    {
        changestate(STATE.ATSTART);
    }

    /// <summary>
    /// erally start playing the game including game timer
    /// </summary>
    public void GAMEstartPlaying()
    {
        changestate(STATE.PLAYING);
        pathControl.play();
    }

    /// <summary>
    /// delay on crash without game over
    /// </summary>
    public void GAMEdelay()
    {
        delayTime = delayOnCrash;
        delayingPlayer = true;
    }

    /// <summary>
    /// call the anoying dog
    /// </summary>
    public void GAMEdog()
    {
    }

    /// <summary>
    /// game over due to crash
    /// </summary>
    public void GAMEfinaleCrash()
    {

    }

    /// <summary>
    /// game over due to falling into the well drawing
    /// </summary>
    public void GAMEfinaleDrawing()
    {

    }

    /// <summary>
    /// make gadget screen wet
    /// </summary>
    public void GAMEwet()
    {
        
    }

    /// <summary>
    /// game timer run down
    /// </summary>
    public void GAMEtimeout()
    {
       
    }

    /// <summary>
    /// reach end, friends there, happy end
    /// </summary>
    public void GAMEfinaleFriends()
    {
        
    }

    /// <summary>
    /// reach end, friends gone
    /// </summary>
    public void GAMEfinaleNoFriends()
    {

    }

    /// <summary>
    /// uses this to set the speed in m/s
    /// </summary>
    /// <param name="metersPerSecond"></param>
    public void GAMEsetSpeed(float metersPerSecond)
    {
        setSpeedTarget = metersPerSecond;
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
            debugInfo.log("[1] / [2]", "spped +/-",true);
            debugInfo.log("[ Q ]", "reset",true);
            debugInfo.log("[ W ]", "go to start",true);
            debugInfo.log("[ E ]", "start playing",true);

        }
        if (Input.GetKeyDown("d"))
        {
            setDebugState(!debug);
        }
        if (Input.GetKeyDown("1"))
        {
            GAMEsetSpeed(speed - .1f);
        }
        if (Input.GetKeyDown("2"))
        {
            GAMEsetSpeed(speed + .1f);
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
    }

    // Update is called once per frame
    void Update () {
        //pathProgress = pathControl.playheadPosition01();
        if (state == STATE.PLAYING)
        {
            BenjasMath.timer(ref gameTime, maxGameTime, pausing);
            updateSpeed();
        }
        cheatkeys();

    }
}

/*
     private static ParagliderGame instance;

    public bool dontDestroyOnLoad = false;
    private static bool created = false;
    ParagliderLevelControl levelControl;
	public ParagliderControler glider;			//script für paraglider interaktionen mit umgebung
	Rigidbody gliderRig;						//rigidbody of glider
	public GameMap_benja Map;					//steuert die karte
	public paragliderHUD HUD;			        //steuert den höhenmesser etc
    public compassStrip_benja compass;
    public DebugInfo_benja debugInfo;			// gibt debug daten auf dem Bildschirm aus
												// DebugInfo.Log("Zeile","WERT") Befehl erstellt beim ersten Aufruf 
												// die Zeile "Zeile" und danach überschreibt es bei jedem aufruf 
												//den wert dieser Zeile

	public float maxGameTime= 180.0f;			//maximale zeit des spiels 
	public float gameTime =0;                   //zeit des spiels (countdown)
    public float distanceFinishNear = 100;      //needed for giving a message when the finish will be near
    bool finishIsNear = false;
    public bool thermicBeenUsed = false;

    public Sprite mapTexture; //wird aus level info des levels übernommen und an map übergeben

    public bool spawnByCollider = false;	//dürfen collider spawn von Hindernissen auslösen
    public bool spawnByTime = false;		//wird automatisch nach einer gewissen Zeit ein Hinderniss gespawned
    public float maxTimeToSpawn = 6.0f;		//mindestzeit zwischen dem spawnen zweier hindernisse, vor ablauf werden collider ignoriert
	public float timeToSpawn = 6.0f;        //Zeit bis zum nächsten spawn

    public bool pausing = false;
    public bool debug = false;				// sollen debug daten erscheinen
    public bool finishGame = false;         // spiel wird beendet, kein weitere spawning
    private bool godmode = false;
    public delegate void boolDelegate(bool theBool);
    public boolDelegate onDebugChange;

    public STATE state;
    public int currentLevel = 0;

    public enum STATE
    {
        NONE,
        RESETTING,
        READY,
        ATSTART,
        PLAYING,
        CRASH,
        FINISH,
        TIMEOUT
    }

    void changestate(STATE newState)
    {
        state = newState;
        debugInfo.log("state", state.ToString());
    }



    public void getValuesFromConfig()
    {
        maxGameTime = (float)Configuration.GetInnerTextByTagName("maxGameTime", maxGameTime);
        maxTimeToSpawn = (float)Configuration.GetInnerTextByTagName("maxTimeToSpawn", maxTimeToSpawn);
        glider.dockingDuration = (float)Configuration.GetInnerTextByTagName("dockingDuration", glider.dockingDuration);
        glider.fogDuration = (float)Configuration.GetInnerTextByTagName("fogDuration", glider.fogDuration);
        glider.maxHeight = (float)Configuration.GetInnerTextByTagName("maxHeight", glider.maxHeight);
        glider.maxThermicHeight = (float)Configuration.GetInnerTextByTagName("maxThermicHeight", glider.maxThermicHeight);
        glider.minThermicHeight = (float)Configuration.GetInnerTextByTagName("minThermicHeight", glider.minThermicHeight);
        glider.forceOnThermicUp = (float)Configuration.GetInnerTextByTagName("forceOnThermicUp", glider.forceOnThermicUp);
        glider.forceOnThermicDown = (float)Configuration.GetInnerTextByTagName("forceOnThermicDown", glider.forceOnThermicDown);
        glider.forceOnMaxHeight = (float)Configuration.GetInnerTextByTagName("forceOnMaxHeight", glider.forceOnMaxHeight);
    }

    void Start()
    {
        instance = this;
        onDebugChange += instance.debugInfo.setDebugState;
        getValuesFromConfig();

        if (!created && dontDestroyOnLoad)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
            Debug.Log("Awake: " + this.gameObject);
        }
		Map.Setup(mapTexture,new Vector3(-1000,0,-1000),new Vector3(1000,0,1000));
		gliderRig=glider.GetComponent<Rigidbody>();
		ParagliderCrashDetection CrashDetect = glider.GetComponent<ParagliderCrashDetection>();
		glider.onCrash = onCrash;
		//glider.onFog = onFog;
		glider.onFinishReached = onLevelFinish;
        glider.onThermicDown = onThermic;
        glider.onThermicUp = onThermic;
        //glider.onThermicLeave = onThermicLeave;
        glider.onSpawn = onSpawn;
        glider.onDocking = onDocking;
        levelControl = GetComponent<ParagliderLevelControl>();
        gameReset();
    }

    /// <summary>
    ///  can only be called when IGP is overlaying whole screen 
    ///  because it takes several seconds and things will be visible
    /// </summary>
    public void gameReset()
    {
        instance.debug =false; 
        instance.onDebugChange(false);
        //onDoldrums();
        instance.glider.togglePhysics(false);
        instance.levelControl.onPreloadDone = instance.onLevelsLoaded;
        instance.levelControl.onLevelAwake = instance.onLevelawake;
        instance.gameTime = 0;
        instance.levelControl.goToLevel(0);
        currentLevel = 0;
        instance.levelControl.preloadAllLevels();
        instance.debugInfo.log("last big event", "game reset started");
        //HUD.setGameTime(0);
        instance.HUD.appearHIDDEN();
        updateHudTexts();
        instance.gamePause(true);
        instance.finishIsNear = false;
        // state can be used by GameManager to see if main is ready
        instance.changestate(STATE.RESETTING);
    }



    /// <summary>
    /// toggle debug true or false
    /// </summary>
    /// <param name="setDebug"></param>
    void setDebug(bool setDebug)
    {
        instance.debug = setDebug;
        instance.onDebugChange(setDebug);
    }

    public string stateToString(STATE state)
    {
        switch (state)
        {
            case STATE.ATSTART: return "0300";
            case STATE.CRASH: return "0800";
            case STATE.FINISH: return "0600";
            case STATE.PLAYING: return "0400";
            case STATE.TIMEOUT: return "0900";
            default: return "0000";
        }
    }

    public void updateHudTexts()
    {
        // 0404.MT.0600.L.01[Höhe] m
        //0404.MT.0600.L.02[Geschwindigkeit] km / h
        //0404.MT.0600.L.03   m / sek
        //0404.MT.0600.C  Welt[Zahl] / 4
        // TextProvider.GetText("")


        string alt      = "0404.MT." + stateToString(state) + ".L.01";
        string speed    = "0404.MT." + stateToString(state) + ".L.02";
        string level    = "0404.MT." + stateToString(state) + ".C";
        HUD.altUnit = TextProvider.GetText(alt);
        HUD.speedUnit = TextProvider.GetText(speed);
        HUD.levelString = TextProvider.GetText(level) +" "+ currentLevel + "/" + levelControl.getLastLevel();
    }

    /// <summary>
    /// 03.00 Spielstart
    /// Prepare before start and set to start point
    /// </summary>
    public void gameStart()
    {
        changestate(STATE.ATSTART);
        NextLevel();     
        HUD.appearCOMPLETE();
        updateHudTexts();
        updateHUD(0);
        GameManager.ChangePromptTextInGame(1); //03.00 Spielstart
        spawnByCollider = false;
        spawnByTime = false;
        gamePause(true);
    }

    /// <summary>
    /// 04.00 Welt 1: Steuerung
    /// Actually set the paraglider free
    /// </summary>
    public void gameStartPlaying()
    {
        GameManager.ChangePromptTextInGame(2);//04.00 Welt 1: Steuerung (thermics)
        gamePause(false);
        changestate(STATE.PLAYING);
        // set a countdown to spawn afer certain amount of time
        spawnByCollider = false;
        spawnByTime = true;
        timeToSpawn = maxTimeToSpawn;
    }




    /// <summary>
    /// No STATE change
    /// stores current physics values and turns off/on aeroplane scripts
    /// 07.00 Spielende
    /// 08.00 crash / Game Over
    /// 09.00 time out/game over
    /// 10.00 Abbruch
    /// 11.00 Inaktivität
    /// 
    /// </summary>
    /// <param name="doPause"></param>
    public void gamePause(bool doPause = true)
    {
        pausing = doPause;
        glider.togglePhysics(!pausing);

    }







public void onCrash()
{
    debugInfo.log("collider info", "crash");
    if (godmode) return;
    changestate(STATE.CRASH);
    HUD.appearINFOONLY();
    updateHudTexts();

    GameManager.GameOver(false);
}

public void onFog()
{
    Debug.Log("FOG");
    debugInfo.log("collider info", "fog");
}



/// <summary>
/// 06.00 Welt 4: Ziel
/// called when finish is close
/// </summary>
void onFinalFinishNear()
{
    if (!finishIsNear)
    {
        finishIsNear = true;
        HUD.appearCOMPLETE();
        updateHudTexts();
        GameManager.ChangePromptTextInGame(4);//06.00 Welt 4: Ziel
    }

}

/// <summary>
/// called when level finish is reached
/// </summary>
public void onLevelFinish()
{
    debugInfo.log("collider info", "finish reached");
    Debug.Log("level " + levelControl.level + " finish reached");
    if (!finishGame)
    {
        if (levelControl.incrementLevel() != 0) NextLevel();
        else onFinalFinish();
    }
}

/// <summary>
/// called on final finish
/// 07.00 Spielende
/// </summary>
public void onFinalFinish()
{
    gamePause();
    changestate(STATE.FINISH);
    HUD.appearHIDDEN();
    updateHudTexts();
    GameManager.GoToResult(gameTime);
}


/// <summary>
/// 09.00 Zeit abgelaufen
/// call this when time is max game time
/// </summary>
public void onTimeout()
{
    onDoldrums();
    HUD.appearINFOONLY();
    updateHudTexts();
    GameManager.ChangePromptTextInGame(5);//09.00 Zeit abgelaufen
    GameManager.GameOver(true);

}







/// <summary>
/// called when started to dock to the level changing frame
/// </summary>
void onDocking()
{
    debugInfo.log("last big event", "started docking to level frame for level change");
    gamePause();
}

/// <summary>
/// spawn new obstacle
/// spawnByCollider must be true
/// </summary>
public void onSpawn()
{
    if (spawnByCollider)
    {
        GameObject spawned = levelControl.levelInfo().spawnFlyingObject().gameObject;
        if (debug && spawned != null)
            debugInfo.log("spawned", spawned.name);

        Debug.Log("triggered spawn point");
        timeToSpawn = maxTimeToSpawn;
    }
}



private bool hotkeysToDebug = true;


public void cheatkeys()
{
    if (hotkeysToDebug)
    {
        hotkeysToDebug = false;
        debugInfo.log(": CHEATKEYS", "");
        debugInfo.log("key D", "toggle Debug");
        debugInfo.log("key R", "respawn glider");
        debugInfo.log("key S", "spawn obstacle");
        debugInfo.log("key L", "next level");
        debugInfo.log("key X", "reset game");
        debugInfo.log("key g", "toggle godmode");
        debugInfo.log("key p", "toggle physics");
        debugInfo.log("key q/w", "quiet<>windy ");
        debugInfo.log("key space", "superspeed");
        debugInfo.log("key 1", "start game");
        debugInfo.log("key 2", "start playing game (start glider)");
        debugInfo.log(": :", ": : : : :");
    }
    if (Input.GetKeyDown("1"))
    {
        gameStart();
    }

    if (Input.GetKeyDown("2"))
    {
        gameStartPlaying();
    }

    if (Input.GetKeyDown("d"))
    {
        setDebug(!debug);
    }
    if (Input.GetKeyDown("r"))
    {
        glider.respawn();
    }
    if (Input.GetKeyDown("s"))
    {
        onSpawn();
    }
    if (Input.GetKeyDown("l"))
    {
        NextLevel();
    }
    if (Input.GetKeyDown("x"))
    {
        gameReset();
    }
    if (Input.GetKeyDown("g"))
    {
        godmode = !godmode;
        debugInfo.log("godmode", godmode ? "ensbled" : "disabled");
    }
    if (Input.GetKeyDown("w"))
    {
        onStiffBreeze();
    }
    if (Input.GetKeyDown("q"))
    {
        onDoldrums();
    }
    if (Input.GetKeyDown("p"))
    {
        gamePause(!pausing);
        debugInfo.log("physics", glider.physicsEnabled() ? "enabled" : "disabled");
    }
    if (!glider.physicsEnabled())
    {
        if (Input.GetKey("space")) glider.transform.position += glider.transform.forward * 10;
        if (Input.GetKey("up")) glider.transform.eulerAngles += new Vector3(+3, 0, 0);
        if (Input.GetKey("down")) glider.transform.eulerAngles += new Vector3(-3, 0, 0);
        if (Input.GetKey("left")) glider.transform.eulerAngles += new Vector3(0, -3, 0);
        if (Input.GetKey("right")) glider.transform.eulerAngles += new Vector3(0, +3, 0);
    }

}


/// <summary>
/// BUG BUG BUG
/// only update after levels have been loaded, never during load 
/// </summary>
/// <param name="reset"></param>
/// <param name="timeNormalized"></param>
void updateHUD(float timeNormalized = 0)
{
    if (state != STATE.RESETTING) //never update during loading levels, buggy
    {
        HUD.setGameTime(timeNormalized);
        debugInfo.log("time", gameTime);
        //the finish looks same in all directions 
        // so i can misuse that to get the angle towards the glider 
        levelControl.levelInfo().finish.transform.LookAt(glider.transform.position);
        //and then rotate it 180 to get the direction towards the finish
        compass.setBacon(levelControl.levelInfo().finish.transform.eulerAngles.y + 180 - glider.transform.eulerAngles.y);
        compass.setCompass(glider.transform.eulerAngles.y);

        Map.updateMap(glider.position, glider.transform.eulerAngles);

        debugInfo.log("compass angle", glider.transform.eulerAngles.y);
        debugInfo.log("speed", glider.speed);
        debugInfo.log("altitude", glider.altitude);
        debugInfo.log("alt change", glider.altChange);
    }

}

void Update()
{

    cheatkeys();

    if (state == STATE.PLAYING)
    {
        float normalizedGametime = BenjasMath.timer(ref gameTime, maxGameTime, pausing);
        updateHUD(normalizedGametime);
        if (normalizedGametime == 1)
        {
            onTimeout();
        }

        if (!pausing)
        {
            if (spawnByTime && BenjasMath.countdownToZero(ref timeToSpawn))
            {
                // switch off spawning by time after first object and switch on spawning when hitting a collider
                spawnByTime = false;
                spawnByCollider = true;
                //spawn first object
                onSpawn();
            }
            if (levelControl.isLastLevel())
            {
                if (Vector3.Distance(glider.transform.position, levelControl.levelInfo().finish.transform.position) < distanceFinishNear)
                {
                    onFinalFinishNear();
                }
            }
        }

    }

}



     */