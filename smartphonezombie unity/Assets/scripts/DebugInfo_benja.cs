using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugInfo_benja : MonoBehaviour {

    public UnityEngine.UI.Text textField;
    public delegate void boolDelegate(bool theBool);
    public boolDelegate onDebugChange;

	// Use this for initialization
	void Start () {
	    	
	}

    public bool debugging = true;

    public List<string> names;
    public List<string> values;
    public bool doUpdate = true;

    public void log(string name, float value)
    {
        
        log(name, (Mathf.Round(value*100)/100).ToString());
    }

    public void log(string name, string value, bool forceUpdate = false)
    {
        
        if (debugging || forceUpdate)
        {
            int index = names.IndexOf(name);
            if (index < 0)
                index = addLine(name);
            values[index] = value;
            doUpdate = true;
        }
    }

    int addLine(string name)
    {
        names.Add(name);
        values.Add("");
        return names.Count - 1;
    }
    private void updateText()
    {
        textField.text = "Debug Info _______________________";
        for (int i = 0; i < names.Count; i++)
        {
            textField.text += "\n" + names[i] + " : " + values[i];
        }
      
    }
    // Update is called once per frame
    void Update () {
     
        if (doUpdate && debugging)
        {
            doUpdate = false;
            updateText();
        }
        textField.enabled = debugging;
    }



    public void setDebugState(bool debug)
    {
        debugging = debug;
        onDebugChange(debug);
    }

}
