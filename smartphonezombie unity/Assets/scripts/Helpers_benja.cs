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

    public static AudioLoader_benja[] findAudioClips(string path, string file)
    {
        Debug.Log("searching for " + path +" ::: "+ file);
        FileInfo[] fileInfo = findAllFiles(path, file);
        Debug.Log("fileInfo " + (fileInfo != null));
        AudioLoader_benja[] clips = new AudioLoader_benja[fileInfo.Length];
        Debug.Log(clips.Length);
        for (int i = 0; i < fileInfo.Length; i++)
        {
            Debug.Log(i);
            Debug.Log("loading audio file" + fileInfo[i].Name);
            clips[i] = new AudioLoader_benja();
            clips[i].loadAudioClip(path,fileInfo[i].Name);
            
        }

            return clips;
    }



}
