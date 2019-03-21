using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class SmombieQuestManager : MonoBehaviour
{
    private bool neverUsed;
    [Header("test quests at certain spawn points")]
    public int testspawnQuestIndex = -1;
    public int atSpawnPointIndex = -1;
    [Header("lists dont touch this")]
    public SmombieQuest[] quests;
    public SmombieSpawnPoint[] spawns;
    public List<TriggerChecker> corners;
    [Header("adjustable preferences for quest distribution")]
    public int fotoQuestsMax = 1;
    public int streetQuestsMax = 99;
    public int crossingQuestsMax = 99;
    public int carrierQuestsMax = 1;
    public int houseQuestsMax = 99;
    [Header("info dont touch this")]
    public int spawnsSold = 0;
    public bool inFotoQuest = false;


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
                int q = Random.Range(0, quests.Count );
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
        testspawnQuestIndex = -1;
        quests = GetComponentsInChildren<SmombieQuest>();

        spawns = GetComponentsInChildren<SmombieSpawnPoint>();

        corners = GetComponentsInChildren<TriggerChecker>().ToList<TriggerChecker>();
        for (int i = 0; i < corners.Count; i++)
            if (corners[i].gameObject.name != "cornerTrigger")
            {
                corners.Remove(corners[i]);
                i--;
            }
            else
            {
                corners[i].onTrigger = onReachCorner;
            }

            inFotoQuest = false;
    // Reset();
}



    /*
    public void setupAudio(string path)
    {
       
        foreach (SmombieQuest quest in quests) quest.setupAudio(path);
    }
    */
    public void Reset()
    {
        Debug.Log("resetting quests");
        distributeQuests();
        foreach (SmombieQuest quest in quests)
        {
            quest.onCrash = onQuestFail;
            quest.onPass = onQuestPass;
            quest.onEnter = onQuestEnter;
            quest.getCrashSpeed = SmombieGame.GetInstance().GAMEgetSpeed;
        }
    }

    public void onReachCorner()
    {
        if (inFotoQuest)
        {
            inFotoQuest = false;
            SmombieGame.GetInstance().GAMEfotoExit();
        }
  
    }

    int nextTestQuest = -1;

    public void testQuest(bool nextQuest = true)
    {
        if (nextQuest) nextTestQuest++;
        if (nextTestQuest < 0 || nextTestQuest >= quests.Length) nextTestQuest = 0;
        testspawnQuestIndex = nextTestQuest;
        testspawn();


    }


    void testspawn()
    {
        testspawnQuestIndex = Mathf.Clamp(testspawnQuestIndex,0, quests.Length - 1);
        atSpawnPointIndex = Mathf.Clamp(atSpawnPointIndex, 0, spawns.Length - 1);
        foreach (SmombieQuest quest in quests) quest.gameObject.SetActive(false);
        quests[testspawnQuestIndex].spawnAt(spawns[atSpawnPointIndex]);
        testspawnQuestIndex = -1;
    }

    /// <summary>
    /// this will be called when a quest is getting activated, especially interesting for photo quests
    /// </summary>
    /// <param name="quest"></param>
    public void onQuestEnter(SmombieQuest quest)
    {
        Debug.Log("ENTER: " + quest.name);
        if (quest.questtype == SmombieQuest.QUESTTYPE.FOTO)
        {
            inFotoQuest = true;
            SmombieGame.GetInstance().GAMEfotoEnter();
        }
        SmombieGame.GetInstance().sendInfoForFinalResultCollection(quest.codeForFinaleText);
    }

    /// <summary>
    /// called on quest pass, call finale texts here too
    /// </summary>
    /// <param name="quest"></param>
    public void onQuestPass(SmombieQuest quest)
    {
        Debug.Log("PASS: " + quest.name);
        SmombieGame.GetInstance().sendInfoForFinalResultCollection(quest.codeForFinaleText, quest.finaleSnapshotFilePath);
    }

    /// <summary>
    /// this will be called on a quest fail and manage the reaction
    /// grab the finale texts here too
    /// </summary>
    /// <param name="quest"></param>
    public void onQuestFail(SmombieQuest quest)
    {
        if (SmombieGame.GetInstance().godModeNoCrashes) return;
        Debug.Log("CRASH: " + quest.name);
        switch (quest.reactionOnFail)
        {
            case SmombieQuest.REACTION_FAIL.DELAY:
                SmombieGame.GetInstance().GAMEdelay();
                delayedQuest = quest;
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
        SmombieGame.GetInstance().sendInfoForFinalResultCollection(quest.codeForFinaleText,quest.finaleSnapshotFilePath);
    }

    public SmombieQuest delayedQuest;

    public void onContinueAfterDelay()
    {
        if(delayedQuest!= null)
        {
            delayedQuest.handleContinue();
            delayedQuest = null;
        }
    }






    // Update is called once per frame
    void Update()
    {
        if (testspawnQuestIndex>=0) testspawn();
    }

    public void distributeQuests(bool useTestspawn = false)
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
        
        for (int j = 0; j < spawns.Length; j++)
        {
            //fetch a random spawn point:
            int s = Random.Range(0, spawnsAvailable.Count - 1);
            bool spawnSold = false;
            spawnsAvailable[s].Reset();
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