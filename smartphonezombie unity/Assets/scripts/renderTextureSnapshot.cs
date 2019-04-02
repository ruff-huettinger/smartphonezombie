using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class renderTextureSnapshot : MonoBehaviour {

    public bool takeSnapshot;
    public bool timestamp;
    public string fileName = "";
    private string subFolder = "../snapshots";
    public string filePath;
    public Camera theCam;

    public void Start()
    {
        if (theCam==null)
        {
            theCam = GetComponent<Camera>();
        }
        if (keepCamDisabled && theCam != null) theCam.enabled = false;
        if (fileName == "") fileName = this.name;
        if (!System.IO.Directory.Exists(subFolder + "/"))
            System.IO.Directory.CreateDirectory(subFolder + "/");
    }
    // Update is called once per frame
    void Update () {
        if (theCam != null)
        {
            if (takeSnapshot)
            {
                StartCoroutine(snapshot());
                takeSnapshot = false;
            }
            else theCam.enabled = !keepCamDisabled; 
        }
    }

    private bool lockFilePath = false;

    /// <summary>
    /// generate a snapshot at the end of the frame
    /// </summary>
    /// <param name="filename">plain filename of the png file</param>
    /// <returns>whole filepath</returns>
    public string takeSnapShot(string filename)
    {
        fileName = filename;
        return takeSnapShot();
    }

    /// <summary>
    /// generate a snapshot at the end of the frame
    /// </summary>
    /// <returns>whole filepath</returns>
    public string takeSnapShot()
    {
        takeSnapshot = true;
        return generateFilePath();
    }

    public bool keepCamDisabled = false;

    public void keepCamDisabledWhenUnused()
    {
        keepCamDisabled = true;
        if(theCam!=null) theCam.enabled = false;
    }

    private string generateFilePath(bool unlockFilePath = false)
    {
        if (!lockFilePath)
        {
            filePath = subFolder + "/" + fileName;
            if (timestamp) filePath += "_" + Time.frameCount.ToString();
            filePath += ".png";
        }
        lockFilePath = !unlockFilePath;
        return filePath;
    }

    // Take a shot immediately
    IEnumerator snapshot()
    {

        // We should only read the screen buffer after rendering is complete
        if (theCam != null)
        {
            theCam.enabled = true;
        }
            yield return new WaitForEndOfFrame();

        if (theCam != null)
        {
            theCam.enabled = true;
            RenderTexture.active = theCam.targetTexture;
            Texture2D tex = new Texture2D(theCam.targetTexture.width, theCam.targetTexture.height, TextureFormat.RGB24, true);
         
            tex.ReadPixels(new Rect(0, 0, theCam.targetTexture.width, theCam.targetTexture.height), 0, 0);
            tex.Apply();

            // Encode texture into PNG
            byte[] bytes = tex.EncodeToPNG();
            Destroy(tex);

            System.IO.File.WriteAllBytes(generateFilePath(), bytes);
        }
    }
}
