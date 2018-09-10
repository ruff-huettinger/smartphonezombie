using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class recordAndPlayPath_Benja : MonoBehaviour {

    public GameObject trackedObject;
    public GameObject followingObject;


    public float keyframeDistance = 0.01f;
    public bool recordKeyframes = false;
    private bool isRecording = false;
    public bool debug = false;

    public float pathDistance = 0f;
    public bool playKeyframes = false;
    public float speedInMPerS = 0f;

    

    public List<GameObject> keyframes;


        public GameObject Keyframe(Transform trafo,string keyframeName)
        {
        GameObject go = new GameObject(keyframeName);
        go.transform.position = trafo.position;
        go.transform.rotation = trafo.rotation;
        go.transform.parent = this.transform;
        return go;
        }

        public GameObject Keyframe(Transform trafo)
        {
         return Keyframe(trafo, "keyframe");
        }

        public GameObject Keyframe(GameObject frame)
        {
         return Keyframe(frame.transform);
        }

        public GameObject Keyframe(GameObject frame, string keyframeName)
        {
            frame = Keyframe(frame.transform, keyframeName);
        return frame;
        }



    void eraseKeyframes()
    {
        //destroy existing keyframes as gameobjects and as list element
        foreach (GameObject frame in keyframes)
        {
            
            if(frame != null)
                Destroy(frame);
        }
        keyframes.Clear();
    }

    string keyframeName(float distanceOnPath)
    {
        return "keyframe " + distanceOnPath.ToString("F3") + "m";
    }

    bool keyframeByinterpolation(GameObject lastFrame, GameObject nextFrame, float distance)
        {
            
            float maxdist = Vector3.Distance(lastFrame.transform.position, nextFrame.transform.position);
            if (maxdist < distance)
            return false;
            pathDistance += keyframeDistance;
            GameObject keyframe = new GameObject(keyframeName(pathDistance));
            float t = Mathf.InverseLerp(0, maxdist, distance);
            keyframe.transform.position = Vector3.Lerp(lastFrame.transform.position, nextFrame.transform.position, t);
            keyframe.transform.rotation = Quaternion.Lerp(lastFrame.transform.rotation, nextFrame.transform.rotation, t);
            keyframe.transform.parent = this.transform;
            keyframes.Add(keyframe);
            return true;
        }



    // Use this for initialization
    void Start ()
    {

	}
	
    public void startRecording()
    {
        Debug.Log("recording started");
        isRecording = true;
        recordKeyframes = true;
        pathDistance = 0f;
        betterDecreaseSpeedByFactor = 0;
        eraseKeyframes();
        keyframes.Add(Keyframe(trackedObject,keyframeName(0)));
    }

    public int betterDecreaseSpeedByFactor=0;

    void keepRecording()
    {
        int steps = 0;
        while (keyframeByinterpolation(keyframes[keyframes.Count - 1], trackedObject, keyframeDistance)) steps++;

        betterDecreaseSpeedByFactor = Mathf.Max(betterDecreaseSpeedByFactor, steps);
    }

    public void stopRecording()
    {
        Debug.Log("recording stoped, please save this game object as prefab or aply changes if it already exists");
        pathDistance += Vector3.Distance(keyframes[keyframes.Count - 1].transform.position, trackedObject.transform.position);
        keyframes.Add(Keyframe(trackedObject, keyframeName( pathDistance)));
        isRecording = false;
        recordKeyframes = false;
    }



    public void drawpath()
    {
        for(int i=0; i<keyframes.Count; i++)
        {
            Debug.DrawLine(keyframes[i].transform.position, keyframes[i].transform.position + keyframes[i].transform.forward);
        }
    }



    // Update is called once per frame
    public float playheadPositionInM = 0;
    public float playheadPosition01()
    {
        return playheadPositionInM / pathDistance;
    }

    /// <summary>
    /// starts playing the keyframes with the current speed
    /// if fromStart will start playing from first frame
    /// </summary>
    /// <param name="fromStart"></param>
    public void play(bool fromStart = false)
    {
        if(fromStart)
        {
            returnToStart();
        }
        playKeyframes = keepPlaying();
    }

    /// <summary>
    /// keeps playing alive
    /// </summary>
    /// <returns></returns>
    bool keepPlaying()
    {
        bool returnValue = true;
        playheadPositionInM += Time.deltaTime * speedInMPerS;
        playheadPositionInM = Mathf.Clamp(playheadPositionInM, 0, pathDistance);
        if (playheadPositionInM >= pathDistance)
        {
            playheadPositionInM = pathDistance;
            stopPlaying();
            returnValue = false;
        }
        int i = Mathf.FloorToInt(playheadPositionInM / keyframeDistance);
        float a =(float) i * keyframeDistance;
        //makes sure last frame (which is most likely closer than keyframe distance) wont overshoot
        float b = Mathf.Min(a + keyframeDistance, pathDistance); 
        float t = Mathf.InverseLerp(a, b, playheadPositionInM   );
        followingObject.transform.position = Vector3.Lerp(keyframes[i].transform.position, keyframes[i + 1].transform.position, t);
        followingObject.transform.rotation = Quaternion.Lerp(keyframes[i].transform.rotation, keyframes[i + 1].transform.rotation, t);
        return returnValue;
    }

    /// <summary>
    /// stops playing hard
    /// if returnToStart will set the playhead back on start and speed to 0
    /// </summary>
    /// <param name="returnToStart"></param>
    public void stopPlaying(bool returnToStart = false)
    {
        playKeyframes = false;
        if(returnToStart)
        {
            this.returnToStart();
            keepPlaying();
        }
        
    }


    /// <summary>
    /// sets the playhead back on start and sets speed to 0
    /// </summary>
    public void returnToStart()
    {
        playheadPositionInM = 0;
        speedInMPerS = 0;
    }

    void Update ()
    {
        if (debug) 
        {
            drawpath();
        }
        if (playKeyframes)
        {
            playKeyframes = keepPlaying();
        }


        else if (recordKeyframes)
        {
            if (!isRecording)
            {
                startRecording();
            }
            else
            {
                keepRecording();
            }
        }
        else if (isRecording)
        {
            stopRecording();
        }	
	}
}
