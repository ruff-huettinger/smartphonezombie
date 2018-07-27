using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


public class SmombieQuestManager : MonoBehaviour {


 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /*

     
	//public List<questacle> questacles = new List<questacle>();
	[Header("questacles")]


	public staticquestacle[] streetQuestLib = new staticquestacle[12] ; // all available questacles in the street
    public staticquestacle[] cornerquestacleLib = new staticquestacle[12];   // all available Quests on corners
    public staticquestacle[] fotoQuestLib = new staticquestacle[3]; // all available foto Quests
    public Transform[] cornerSpawnPointLib;
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
}
