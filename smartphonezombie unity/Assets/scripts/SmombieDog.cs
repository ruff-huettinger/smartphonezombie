using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmombieDog : MonoBehaviour {

    public bool showDog = false;
    public bool loopDog = false;
    //public string audioFilename;
    public AudioClip barkingClip;
    //public string audioPath;
    public float dogLoopLength = 2;
    float timer = 0;
    public Vector2 posDownL;
    public Vector2 posUp;
    public Vector2 posDownR;
    RectTransform trafo;
    Vector3 scale;
    //AudioLoader_benja loader;
    AudioSource barker;
    public Canvas canvas;
    

    // Use this for initialization
    public void Setup (string audioPath) {
       trafo = GetComponent<RectTransform>();
        scale = trafo.localScale;
         //loader = gameObject.AddComponent<AudioLoader_benja>();
        //loader.loadAudioClip(Application.streamingAssetsPath +"/"+ audioPath, audioFilename);
        barker = gameObject.AddComponent<AudioSource>();
        canvas = gameObject.GetComponentInParent<Canvas>();
	}

    bool mirrored = false;
    bool up = true;

    public void DOGstart()
    {
        Reset();
        barker.Play();
        showDog = true;
        loopDog = true;
    }

    public void DOGstop()
    {
        barker.Stop();
        showDog = false;
    }

    private void Reset()
    {
        barker.Stop();
        barker.loop = true;
        //barker.clip = loader.audioClip;
        barker.clip = barkingClip;
        mirrored = false;
        up = true;
        setPos(posDownL);
        showDog = false;
        loopDog = false;
    }

    void setPos(Vector2 pos)
    {
        trafo.localPosition = new Vector3(pos.x, pos.y, trafo.localPosition.z);
        if (mirrored)   trafo.localScale = new Vector3(-scale.x, scale.y, scale.z);
        else            trafo.localScale = scale;
    }



    public float t;
	// Update is called once per frame
	void Update () {
        if (showDog)
        { loopDog = true; }

		if(loopDog)
        {
            t = BenjasMath.timer(ref timer, dogLoopLength/2);
            
            if(!mirrored)
            { 
                if(up)  setPos(Vector2.Lerp(posDownL, posUp, BenjasMath.easeIn(t)));
                else    setPos(Vector2.Lerp(posUp, posDownR, BenjasMath.easeOut(t)));
            }
            else
            {
                if (up) setPos(Vector2.Lerp(posDownR, posUp, BenjasMath.easeIn(t)));
                else setPos(Vector2.Lerp(posUp, posDownL, BenjasMath.easeOut(t)));
            }
            if (t >= 1)
            {
                //quarter loop omplete
                if (!up)
                {
                    //is down, maybe stop
                    loopDog = showDog;
                    //mirror
                    mirrored = !mirrored;
                }
                timer = 0;
                up = !up;
            }
        } 
	}
}
