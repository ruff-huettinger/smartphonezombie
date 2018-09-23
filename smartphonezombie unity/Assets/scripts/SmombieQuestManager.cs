using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class SmombieQuestManager : MonoBehaviour
{

    public SmombieQuest[] quests;
    public SmombieSpawnPoint[] spawns;
    public int fotoQuestsMax = 1;
    public int streetQuestsMax = 99;
    public int crossingQuestsMax = 99;
    public int carrierQuestsMax = 1;
    public int houseQuestsMax = 99;
    public int spawnsSold = 0;

    [SerializeField]
    class questList
    {
        List<SmombieQuest> quests = new List<SmombieQuest>();
        int questsUsed = 0;
        int questsMax = 0;

        public questList(int maximumAvailable)
        {
            questsMax = maximumAvailable;
            questsUsed = 0;
        }

        public void Add(SmombieQuest newQuest)
        {
            quests.Add(newQuest);
        }

        public bool spawn(SmombieSpawnPoint spawn)
        {

            if (questsMax > questsUsed && quests.Count > 0)
            {
                //spawn a random quest:
                int q = Random.Range(0, quests.Count - 1);
                quests[q].spawnAt(spawn);
                quests.RemoveAt(q);
                questsUsed++;
                return true;
            }
            return false;
        }


    }

    // Use this for initialization
    void Awake()
    {
        quests = GetComponentsInChildren<SmombieQuest>();

        spawns = GetComponentsInChildren<SmombieSpawnPoint>();

       // Reset();
    }

    public void setupAudio(string path)
    {
       
        foreach (SmombieQuest quest in quests) quest.setupAudio(path);
    }

    public void Reset()
    {
        Debug.Log("resetting quests");
        distributeQuests();
        foreach (SmombieQuest quest in quests)
        {
            quest.onCrash = onQuestFail;
            quest.onPass = onQuestPass;
            quest.onEnter = onQuestEnter;
        }
    }

    /// <summary>
    /// this will be called on a quest fail and manage the reaction
    /// grab the finale texts here too
    /// </summary>
    /// <param name="quest"></param>
    public void onQuestFail(SmombieQuest quest)
    {
        Debug.Log("fail detected by quest " + quest.name);
        switch (quest.reactionOnFail)
        {
            case SmombieQuest.REACTION_FAIL.DELAY:
                SmombieGame.GetInstance().GAMEdelay();
                break;

            case SmombieQuest.REACTION_FAIL.DOG:
                SmombieGame.GetInstance().GAMEdog();
                break;

            case SmombieQuest.REACTION_FAIL.WET:
                SmombieGame.GetInstance().GAMEwet();
                break;

            case SmombieQuest.REACTION_FAIL.FINALE_CRASH:
                SmombieGame.GetInstance().GAMEfinaleCrash();
                break;

            case SmombieQuest.REACTION_FAIL.FINALE_DRAWING:
                SmombieGame.GetInstance().GAMEfinaleDrawing();
                break;

        }
    }

    /// <summary>
    /// this will be called when a quest is getting activated, especially interesting for photo quests
    /// </summary>
    /// <param name="quest"></param>
    public void onQuestEnter(SmombieQuest quest)
    {
        Debug.Log("enter detected by quest " + quest.name);
        if (quest.questtype == SmombieQuest.QUESTTYPE.FOTO)
        {
            Debug.LogError("handle foto quest here");
        }
    }

    /// <summary>
    /// called on quest pass, call finale texts here too
    /// </summary>
    /// <param name="quest"></param>
    public void onQuestPass(SmombieQuest quest)
    {
        Debug.Log("pass detected by quest " + quest.name);
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void distributeQuests()
    {
        List<SmombieSpawnPoint> spawnsAvailable;
        questList fotoQuests = new questList(fotoQuestsMax);
        questList streetQuests = new questList(streetQuestsMax);
        questList crossingQuests = new questList(crossingQuestsMax);
        questList carrierQuests = new questList(carrierQuestsMax);
        questList houseQuests = new questList(houseQuestsMax);
        foreach (SmombieQuest quest in quests)
        {
            // sort all quests into questlists
            quest.gameObject.SetActive(false);

            if (quest.questtype == SmombieQuest.QUESTTYPE.CROSSING)
            {
                crossingQuests.Add(quest);
            }
            else if (quest.questtype == SmombieQuest.QUESTTYPE.CARRIER)
            {
                carrierQuests.Add(quest);
            }
            else if (quest.questtype == SmombieQuest.QUESTTYPE.STREET)
            {
                streetQuests.Add(quest);
            }
            else if (quest.questtype == SmombieQuest.QUESTTYPE.HOUSE)
            {
                houseQuests.Add(quest);
            }
            else if (quest.questtype == SmombieQuest.QUESTTYPE.FOTO)
            {
                fotoQuests.Add(quest);
            }

        }

        spawnsAvailable = spawns.ToList<SmombieSpawnPoint>();
        spawnsSold = 0;

        // loop one time for each spawnpoint even if it is picked in random order
        int loopcount = spawns.Length;
        for (int j = 0; j < loopcount; j++)
        {
            //fetch a random spawn point:
            int s = Random.Range(0, spawnsAvailable.Count - 1);
            bool spawnSold = false;

            //try to sell it to random quests in order of importance of questtype
            if (!spawnSold && spawnsAvailable[s].acceptsCarrierQuest)
            {
                // if the spawn function works we're done here, if not, we will ask the next spawn type
                spawnSold = carrierQuests.spawn(spawnsAvailable[s]);
            }

            if (!spawnSold && spawnsAvailable[s].acceptsCrossingQuest)
            {
                // if the spawn function works we're done here, if not, we will ask the next spawn type
                spawnSold = crossingQuests.spawn(spawnsAvailable[s]);
            }

            if (!spawnSold && spawnsAvailable[s].acceptsHouseQuest)
            {
                // if the spawn function works we're done here, if not, we will ask the next spawn type
                spawnSold = houseQuests.spawn(spawnsAvailable[s]);
            }

            if (!spawnSold && spawnsAvailable[s].acceptsStreetQuest)
            {
                // if the spawn function works we're done here, if not, we will ask the next spawn type
                spawnSold = streetQuests.spawn(spawnsAvailable[s]);
            }

            if (!spawnSold && spawnsAvailable[s].acceptsFotoQuest)
            {
                // if the spawn function works we're done here, if not we will delete the spawnpoint anyway to not pick it again
                spawnSold = fotoQuests.spawn(spawnsAvailable[s]);
            }

            if (spawnSold)
                spawnsSold++;

            spawnsAvailable[s].gameObject.name = "spawn point " + j + (spawnSold ? " sold" : "");
            spawnsAvailable.RemoveAt(s);
        }

        Debug.Log("quests distributed");
        /* distributeQuestType(ref fotoQuests);
         distributeQuestType(ref houseQuests);
         distributeQuestType(ref carrierQuests);
         distributeQuestType(ref streetQuests);*/
    }
}





    /*

     
	//public List<questacle> questacles = new List<questacle>();
	[Header("questacles")]


	public staticquestacle[] streetQuestLib = new staticquestacle[12] ; // all available questacles in the street
    public staticquestacle[] carrierquestacleLib = new staticquestacle[12];   // all available Quests on carriers
    public staticquestacle[] fotoQuestLib = new staticquestacle[3]; // all available foto Quests
    public Transform[] carrierSpawnPointLib;
    public Transform[] streetSpawnPointLib;
    public Transform[] fotoSpawnpointLib;
    public staticquestacle fotoQuestInGame;
    public List<staticquestacle> QuestsInGame;

	public delegate void Delegate();
    public Delegate onSetupFinished;
	public Delegate onLevelAwake;


    

	[System.Serializable]
	public class staticquestacle
	{
		public SmombieStaticQuests Object;
		public int maxAppearances=0;

		public void distribute()
		{
			if (Object!=null)
			maxAppearances = Object.distribute(maxAppearances);
		}

	}


	[CustomPropertyDrawer(typeof(staticquestacle))]
	public class staticquestacleDrawer : PropertyDrawer 
	{
	    // Draw the property inside the given rect
	    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
	    {
	        // Using BeginProperty / EndProperty on the parent property means that
	        // prefab override logic works on the entire property.
	        EditorGUI.BeginProperty(position, label, property);
	        // Draw label
	        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
	        // Don't make child fields be indented
	        int indent = EditorGUI.indentLevel;
	        EditorGUI.indentLevel = 0;
	        // draw
			position.x = 100;
			position.x += labeledField(position.x , position.y,0,Mathf.Max(50,200),position.height,"",property.FindPropertyRelative ("Object"));
			position.x += labeledField(position.x, position.y,40,25,position.height,"count",property.FindPropertyRelative ("maxAppearances"));
	        // Set indent back to what it was
	        EditorGUI.indentLevel = indent;
	        EditorGUI.EndProperty();
	    }

	}

	public static float labeledField(float x,float y,float wLabel, float wContent, float h, string label, SerializedProperty prop)
	    {
				
				EditorGUI.LabelField(new Rect(x, y, wLabel, h),label);
				EditorGUI.PropertyField(new Rect(x+wLabel,y,wContent,h), prop,GUIContent.none, true);
				return wLabel+wContent;
	    }


	public void orderquestacles(staticquestacle[] Array)
	{
		availableMobilquestacles.Clear();
		for(int i = 0;i<Array.Length;i++)
		{
			if (Array[i].Object!=null && Array[i].maxAppearances>0)
			{
				for (int j=0;j<Array[i].maxAppearances;j++)
				{
					availableMobilquestacles.Add(Array[i].Object.GetComponent<ParagliderFlyingquestacle>());
                    //Debug.Log("█ available onject added:" + availableMobilquestacles[availableMobilquestacles.Count-1]);
                }
			}
		}
	}

	public ParagliderFlyingquestacle spawnFlyingObject()
	{
        if (availableMobilquestacles.Count > 0)
        {
            int i = UnityEngine.Random.Range(0, availableMobilquestacles.Count);
            Debug.Log("level "+SceneManager.GetActiveScene().name+" - fetching index " + i + " out of " + availableMobilquestacles.Count + "available objetcs");
            GameObject copy = Instantiate(availableMobilquestacles[i].gameObject);
            copy.transform.parent = terrain.transform;
            copy.SetActive(true);
            ParagliderFlyingquestacle output = copy.GetComponent<ParagliderFlyingquestacle>();
            availableMobilquestacles.RemoveAt(i);
            Debug.Log(i + "of" + availableMobilquestacles.Count + " available objects is a " + copy.name + " with script " + output + "is ready to be spawned");
            output.spawn();
            return output;
        }
        return null;
	}





	public void distributeStatquest()
	{
		foreach( staticquestacle quest in streetQuests)
		{
			quest.distribute();
		}
	}



	public void makeLevelPreview()
	{
		//put the level preview (with cam) to the point where the paraglider will appear
		if(startPoint == null)
		Debug.LogError(level+"has no start point, put gameobject in from inspector");
		if(levelPreview == null)
		return;
		levelPreview.transform.position=startPoint.transform.position;
		levelPreview.transform.rotation=startPoint.transform.rotation;
		previewTexture = levelPreview.getPreviewTexture();
	}

	// Use this for initialization
	public ParagliderLevel Setup () {
		Debug.Log(">>>>>>>>>>> setting up Level ("+gameObject.name+") <<<<<<<<<<<<<<<");

		if (terrain != null)
        {
			terrain.SetActive(true);
        }
		waitForDelayedSetup = true;
        return this;
	}

	//wait for all scripts to run and settle down
	public int delayFrames = 4; 
	bool waitForDelayedSetup = false;

	public void delayedSetup()
	{
		waitForDelayedSetup=false;
		distributeStatquest();
		orderMobilequestacles(flyingquestacles);
		if (terrain != null)
        {
			makeLevelPreview();
            terrain.SetActive(false);
            onSetupFinished();
        }
	}

	bool waitForWake = false;

	public void wake()
	{
		Debug.Log("public void wake(RenderTexture previewTexture)");
		terrain.SetActive(true);
		waitForWake = true;
		delayFrames = 2;
	}

	public void delayedAwake()
	{
		if(levelPreview!= null)
		{
			levelPreview.gameObject.SetActive(false);
		}

		onLevelAwake();
	}

	// Update is called once per frame
	void Update () 
	{
		if (waitForDelayedSetup && delayFrames-- <0)
		{
			delayedSetup();
		}
		else if (waitForWake && delayFrames-- <0) 
		{
			waitForWake = false;
			delayedAwake();
		}
	}
    /**/

