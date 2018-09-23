using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLoader_benja : MonoBehaviour
{
    

    [Header("Audio Stuff")]
    public AudioClip audioClip;
    public string filePath = "/Sounds";
    public string fileName = "mario.wav";

    public void loadAudioClip(string path,string audioFileName)
    {
        filePath = "file://" + path + "/";
        fileName = audioFileName;
        Debug.Log("A________________");
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