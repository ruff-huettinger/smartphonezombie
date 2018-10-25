using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLoader_benja : MonoBehaviour
{
    

    [Header("Audio Stuff")]
    public AudioClip audioClip;
    public string filePath = "/Sounds";
    public string fileName ="";

    public bool loadAudio = false;
    public bool testAudio = false;
    
    private void Update()
    {
        if(loadAudio)
        {
            loadAudio = false;

            loadAudioClip(Application.streamingAssetsPath + "/" + filePath, fileName);
            StartCoroutine(LoadAudio());
        }
        if (testAudio)
        {
            testAudio = false;
            AudioSource source = GetComponent<AudioSource>();
            source.clip = audioClip;
            source.Play();     
        }
    }
    public void loadAudioClip(string path,string audioFileName)
    {
        filePath = "file://" + path + "/";
        fileName = audioFileName;
        Debug.Log("♬ " + "loading clip " + filePath + "  ::  "+fileName);
        StartCoroutine(LoadAudio());
    }

    private IEnumerator LoadAudio()
    {
        WWW request = GetAudioFromFile(filePath, fileName);
        yield return request;

        audioClip = request.GetAudioClip();
        audioClip.name = fileName;
    }

    private WWW GetAudioFromFile(string path, string filename)
    {
        string audioToLoad = string.Format(path + "{0}", filename);
        WWW request = new WWW(audioToLoad);
        return request;
    }
}