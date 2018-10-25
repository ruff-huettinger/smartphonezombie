using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;


public class InputManager : MonoBehaviour {

    private static InputManager instance;

    [Header("Controlled Game Script (optional)")]
    public SmombieGame game;

    [Header("Mouse Wheel continous (keep scrolling to move)")]
    public float MeterPerScrollClick = 0.1f;
    [Header("Mouse Wheel like joystic (scroll to change speed")]
    public float MeterPerSecondPerScrollClick = 0.1f;
    [Header("Mouse Wheel other parameters")]
    public bool scrollContinouslyMode = true;
    public bool invertDirection = false;
    public float speedMinInMPS = 0;
    public float speedMaxInMPS = 0;
    public float smoothing0To1 = 0;
    public float speedInMPS = 0;


    public static InputManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        MeterPerScrollClick             = (float)Configuration.GetInnerTextByTagName("MeterPerScrollClick", MeterPerScrollClick);
        MeterPerSecondPerScrollClick    = (float)Configuration.GetInnerTextByTagName("MeterPerSecondPerScrollClick", MeterPerSecondPerScrollClick);

    }

    public void Reset()
    {
        speedInMPS = 0;
    }

    public float getSpeed()
    {
        return speedInMPS;
    }
    public float mouseWheel = 0;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            SmartphoneCamera.GetInstance().showView(false);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SmartphoneCamera.GetInstance().showView(true);
        }

        float oldSpeed = speedInMPS;

        if(Input.mouseScrollDelta.y != 0)
        {
            mouseWheel = Input.mouseScrollDelta.y;
        }

        // calculate speed
        if (scrollContinouslyMode)
        {
            //Debug.Log("Input.mouseScrollDelta.y " + Input.mouseScrollDelta.y);
            speedInMPS = mouseWheel * MeterPerScrollClick / Time.deltaTime;
            if (!invertDirection) speedInMPS *= -1;
        }
        else
        {
            speedInMPS = (mouseWheel * MeterPerSecondPerScrollClick);
            if (invertDirection) speedInMPS *= -1;
            speedInMPS = oldSpeed - speedInMPS;
        }

        mouseWheel = 0;


        // clamp speed changes
        if (speedMaxInMPS > speedMinInMPS) speedInMPS = Mathf.Clamp(speedInMPS, speedMinInMPS, speedMaxInMPS); 

        // smooth speed changes
        Mathf.Clamp01(smoothing0To1);
        if (smoothing0To1 > 0 && scrollContinouslyMode) speedInMPS = Mathf.Lerp(speedInMPS, oldSpeed, smoothing0To1);

        // set game speed 
        // (can be taken out if not directly control game)
        if (game != null)
        {
            game.GAMEsetSpeed(speedInMPS);
        }
    }
}


