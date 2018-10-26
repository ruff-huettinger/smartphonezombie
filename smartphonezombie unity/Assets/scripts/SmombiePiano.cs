using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmombiePiano : MonoBehaviour {

    public bool dropPiano = false;
    public bool test = false;
    //public string audioFilename;
    public AudioClip audioClip;
    //public string audioPath;
    public float pianodropLength = 2;
    float timer = 0;
    public float heightAbove = 3;
    public float heightFloor = 0;

    //AudioLoader_benja loader;
    AudioSource audio;


    // Use this for initialization
    public void Start()
    {
   //      loader = gameObject.AddComponent<AudioLoader_benja>();
     //   loader.loadAudioClip(Application.streamingAssetsPath +"/"+ audioPath, audioFilename);
        audio = gameObject.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.loop = false;
        audio.clip = audioClip;

    }


    public void PIANOstart()
    {
        Reset();
        audio.Play();
        dropPiano = true;
    }


    public void Reset()
    {
        audio.Stop();
        //audio.clip = loader.audioClip;
        setPos(heightAbove);
        dropPiano = false;
    }

    void setPos(float height)
    {
        Vector3 temp = transform.localPosition;
        temp.y = height;
        transform.localPosition = temp;
    }



    public float t;
	// Update is called once per frame
	void Update () {

        if(test)
        { test = false;
            PIANOstart(); }
		if(dropPiano)
        {
            t = BenjasMath.timer(ref timer, pianodropLength);

            setPos(Mathf.Lerp(heightAbove, heightFloor, t));
            if (t >= 1)
            {
                dropPiano = false;
                timer = 0;
            }
        } 
	}
}
