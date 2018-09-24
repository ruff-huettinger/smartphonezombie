using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Helpers_benja : MonoBehaviour {

	// Use this for initialization
	void Start () {

        test = findAllFilePaths(Application.streamingAssetsPath + "/Sounds", "0401.S.1100*.wav");
    }

    public string[] test;

    public static FileInfo[] findAllFiles(string path, string filename = "*.*", bool includeSubdirs = false)
    {
        Debug.Log("findAllFiles " + path + " ::: " + filename);
        DirectoryInfo levelDirectoryPath = new DirectoryInfo(path);
            return levelDirectoryPath.GetFiles(filename, includeSubdirs? SearchOption.AllDirectories: SearchOption.TopDirectoryOnly);

    }

    public static string[] findAllFilePaths(string path, string filename = "*.*", bool includeSubdirs = false)
    {

        FileInfo[] fileInfo = findAllFiles(path, filename, includeSubdirs);
        string[] filelist = new string[fileInfo.Length];
        for (int i=0;i<fileInfo.Length; i++)
        {
            filelist[i] = fileInfo[i].FullName;
        }
        return filelist;
    }


    /// <summary>
    /// NOT WORKING, NOT WORKING NOT WORKING
    /// cannot load audio from dynamic audioloader because wont start coroutine
    /// </summary>
    /// <param name="path"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    public static AudioLoader_benja[] findAudioClips(string path, string file,GameObject ScriptContainer)
    {

        

        FileInfo[] fileInfo = findAllFiles(path, file);
        Debug.Log(fileInfo.Length+" audio files found, searching for " + path + " [file] " + file);
        AudioLoader_benja[] clips = new AudioLoader_benja[fileInfo.Length];
        for (int i = 0; i < fileInfo.Length; i++)
        {
            Debug.Log(i + " loading audio file" + fileInfo[i].Name);
            clips[i] = ScriptContainer.AddComponent<AudioLoader_benja>();
            clips[i].loadAudioClip(path,fileInfo[i].Name);
            
        }

            return clips;
    }



}
