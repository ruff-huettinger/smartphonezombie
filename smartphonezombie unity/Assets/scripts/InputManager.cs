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

    public bool scrollContinouslyMode = true;
    public bool invertDirection = false;
    public float speedMinInMPS = 0;
    public float speedMaxInMPS = 0;
    public float smoothing0to1 = 0;
    public float resultingSpeedInMPS = 0;


    public static InputManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        float oldSpeed = resultingSpeedInMPS;
        // calculate speed
        if(scrollContinouslyMode)
        {
            resultingSpeedInMPS = Input.mouseScrollDelta.y * MeterPerScrollClick / Time.deltaTime;
            if (!invertDirection) resultingSpeedInMPS *= -1;
        }
        else
        {
            resultingSpeedInMPS = (Input.mouseScrollDelta.y * MeterPerSecondPerScrollClick);
            if (invertDirection) resultingSpeedInMPS *= -1;
            resultingSpeedInMPS = oldSpeed - resultingSpeedInMPS;
        }


        // clamp speed changes
        if (speedMaxInMPS > speedMinInMPS) resultingSpeedInMPS = Mathf.Clamp(resultingSpeedInMPS, speedMinInMPS, speedMaxInMPS); 

        // smooth speed changes
        Mathf.Clamp01(smoothing0to1);
        if (smoothing0to1 > 0) resultingSpeedInMPS = Mathf.Lerp(resultingSpeedInMPS, oldSpeed, smoothing0to1);

        // set game speed 
        // (can be taken out if not directly control game)
        if (game != null)
        {
            game.GAMEsetSpeed(resultingSpeedInMPS);
        }
    }
}

/*
using UnityEngine;
using System.Collections;
using Devices;
using System;

public class InputManager : MonoBehaviour {
{
    BmcmSensor sensor;

    float rawVal_left       = 0;
    float rawVal_right      = 0;
    float rawVal_ripcord    = 0;

    float normalizedVal_leftGrip = 0;
    float normalizedVal_rightGrip = 0;
    float normalizedVal_ripcord = 0;

    float resultLeftRightMinus1To1 = 0;
    float resultUpDownMinus1To1 = 0;

    float minRawValue_leftgrip = 0;
    float maxRawValue_leftgrip = 5.14f;

    float minRawValue_rightgrip = 0;
    float maxRawValue_rightgrip = 5.14f;

    float minRawVal_ripcord = 0;
    float maxRawVal_ripcord = 5.14f;

    private float deadzone = 0;
    private float threshold_pull = 0;
    private float threshold_control = 0;
    private float threshold_inactivity = 0;
    public bool showInputGUI = false;
    public bool enableKeyboard = false;
    public float inactivityTimeout = 30f;         //max time of inactivity until state changes to inactivity
    public float curInactivityTime;
    bool usingGrips = false;

    int port_ripcord = 1;
    int port_leftGrip = 2;
    int port_rightGrip = 3;

    static InputManager instance;
    private bool pulledGrips;
    private bool pulledLeftGrip;
    private bool pulledRightGrip;
    private bool pulledRipcord;
    private bool waitingForTime = false;

    public static InputManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;

        sensor = new BmcmSensor("usb-ad");
        sensor.Init();
        
        inactivityTimeout = (float)Configuration.GetInnerTextByTagName("inactivityTimeout", inactivityTimeout);
    }

    // Use this for initialization
    void Start()
    {
        port_ripcord    = (int)Configuration.GetInnerTextByTagName("ripcord_port", port_ripcord );
        port_leftGrip   = (int)Configuration.GetInnerTextByTagName("leftgrip_port", port_leftGrip );
        port_rightGrip  = (int)Configuration.GetInnerTextByTagName("rightgrip_port", port_rightGrip );
        Debug.Log("port_ripcord " + port_ripcord);
        Debug.Log("port_leftGrip " + port_leftGrip);
        Debug.Log("port_rightGrip " + port_rightGrip);

        minRawValue_leftgrip = (float)Configuration.GetInnerTextByTagName("minRawValue_leftgrip", minRawValue_leftgrip);
        minRawValue_rightgrip = (float)Configuration.GetInnerTextByTagName("minRawValue_rightgrip", minRawValue_rightgrip);

        maxRawValue_leftgrip = (float)Configuration.GetInnerTextByTagName("maxRawValue_leftgrip", maxRawValue_leftgrip);
        maxRawValue_rightgrip = (float)Configuration.GetInnerTextByTagName("maxRawValue_rightgrip", maxRawValue_rightgrip);

        minRawVal_ripcord = (float)Configuration.GetInnerTextByTagName("minRawVal_ripcord", minRawVal_ripcord);
        maxRawVal_ripcord = (float)Configuration.GetInnerTextByTagName("maxRawVal_ripcord", maxRawVal_ripcord);

        deadzone            = (float)Configuration.GetInnerTextByTagName("deadzone", deadzone);
        threshold_pull       = (float)Configuration.GetInnerTextByTagName("threshold_pull", threshold_pull);
        threshold_control   = (float)Configuration.GetInnerTextByTagName("threshold_control", threshold_control);
        threshold_inactivity = (float)Configuration.GetInnerTextByTagName("threshold_inactivity", threshold_inactivity);

        enableKeyboard = Configuration.GetInnerTextByTagName("debug", false);

        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            enableKeyboard = !enableKeyboard;
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            showInputGUI = !showInputGUI;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (sensor.IsValid() && !enableKeyboard)
        {
            rawVal_ripcord = sensor.GetAnalogIn(port_ripcord);
            rawVal_left = sensor.GetAnalogIn(port_leftGrip);
            rawVal_right = sensor.GetAnalogIn(port_rightGrip);

            if (Input.GetKey(KeyCode.C))
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    minRawValue_leftgrip = rawVal_left;
                    minRawValue_rightgrip = rawVal_right;

                    minRawVal_ripcord = rawVal_ripcord;
                    Configuration.SaveValueInConfig(minRawValue_leftgrip.ToString(), "minRawValue_leftgrip");
                    Configuration.SaveValueInConfig(minRawValue_rightgrip.ToString(), "minRawValue_rightgrip");
                    Configuration.SaveValueInConfig(minRawVal_ripcord.ToString(), "minRawVal_ripcord");
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    maxRawValue_leftgrip = rawVal_left;
                    maxRawValue_rightgrip = rawVal_right;

                    Configuration.SaveValueInConfig(maxRawValue_leftgrip.ToString(), "maxRawValue_leftgrip");
                    Configuration.SaveValueInConfig(maxRawValue_rightgrip.ToString(), "maxRawValue_rightgrip");
                }
                else if (Input.GetKeyDown(KeyCode.P))
                {
                    maxRawVal_ripcord = rawVal_ripcord;
                    Configuration.SaveValueInConfig(maxRawVal_ripcord.ToString(), "maxRawVal_ripcord");
                }
            }

            //normalized Values
            normalizedVal_leftGrip = Remap(rawVal_left, minRawValue_leftgrip, maxRawValue_leftgrip, -1, 1);
            normalizedVal_rightGrip = Remap(rawVal_right, minRawValue_rightgrip, maxRawValue_rightgrip, -1, 1);
            normalizedVal_ripcord = Remap(rawVal_ripcord, minRawVal_ripcord, maxRawVal_ripcord, 0, 1);

            //Filter with Deadzone
            normalizedVal_leftGrip = FilterValueWithDeadzone(normalizedVal_leftGrip);
            normalizedVal_rightGrip = FilterValueWithDeadzone(normalizedVal_rightGrip);

            
            resultLeftRightMinus1To1 = (normalizedVal_rightGrip - normalizedVal_leftGrip) * 0.5f;
            resultUpDownMinus1To1 = (normalizedVal_leftGrip + normalizedVal_rightGrip) * 0.5f;
        }

        if (enableKeyboard || !sensor.IsValid() )
        {
            resultUpDownMinus1To1 = Input.GetAxis("Vertical");
            resultLeftRightMinus1To1  = Input.GetAxis("Horizontal");

            normalizedVal_ripcord = (Input.GetKeyDown(KeyCode.Return))? 1 : 0;
        }

        // pull left / right grip
        var changedGripDirection = false;
        if (normalizedVal_leftGrip > threshold_control && !pulledLeftGrip)
        {
            pulledLeftGrip = true;

            changedGripDirection = true;
        }
        else if (normalizedVal_leftGrip < threshold_control && pulledLeftGrip)
        {
            pulledLeftGrip = false;

            changedGripDirection = true;
        }

        if (normalizedVal_rightGrip > threshold_control && !pulledRightGrip)
        {
            pulledRightGrip = true;

            changedGripDirection = true;
        }
        else if (normalizedVal_rightGrip < threshold_control && pulledRightGrip)
        {
            pulledRightGrip = false;

            changedGripDirection = true;
        }

        if (changedGripDirection)
        {
            changedGripDirection = false;

            string leftDirection = (pulledLeftGrip) ? "down" : "up";
            string rightDirection = (pulledRightGrip) ? "down" : "up";

            GameManager.CallChangedGrips("left=" + leftDirection + ";right=" + rightDirection);
        }

        // pull the grips
        if (resultUpDownMinus1To1 > threshold_control && !pulledGrips)
        {
            pulledGrips = true;
            //GameManager.CallPulledGrips();
        }
        else if (resultUpDownMinus1To1 < threshold_control && pulledGrips)
        {
            pulledGrips = false;
        }

        // pull the Ripcord
        if (normalizedVal_ripcord > threshold_pull && !pulledRipcord)
        {
            pulledRipcord = true;
            GameManager.CallPulledRipcord();
        }
        else if (normalizedVal_ripcord < threshold_pull && pulledRipcord && !waitingForTime)
        {
            waitingForTime = true;
            StartCoroutine(WaitForTimeToReleaseRipcord());
        }

        //Inactivity
        if (usingGrips)
        {
            if (normalizedVal_leftGrip < threshold_inactivity && normalizedVal_rightGrip < threshold_inactivity)
            {
                usingGrips = false;
            }
        }
        else if (!usingGrips)
        {
            if (normalizedVal_leftGrip > threshold_inactivity && normalizedVal_rightGrip > threshold_inactivity)
            {
                curInactivityTime = 0;
                usingGrips = true;
            }

            curInactivityTime += Time.deltaTime;

            if (curInactivityTime > inactivityTimeout)
            {
                curInactivityTime = 0;
                GameManager.CallInactivity();
            }
        }
    }

    IEnumerator WaitForTimeToReleaseRipcord()
    {
        yield return new WaitForSeconds(0.5f);
        pulledRipcord = false;
        waitingForTime = false;
    }

    void OnGUI()
    {
        if (showInputGUI)
        {
            int yPos = 0, lineHeight = 20;
            int x = 20;
            int y = 400;

            GUI.Box(new Rect(10, y, 200, 300), "");
            GUI.Label(new Rect(x, y + (++yPos) * lineHeight, 500, 500), "USB-AD is " + sensor.IsValid());
            GUI.Label(new Rect(x, y + (++yPos) * lineHeight, 500, 500), "raw Value left " + port_leftGrip + ": " + rawVal_left);
            GUI.Label(new Rect(x, y + (++yPos) * lineHeight, 500, 500), "norm Value left: " + normalizedVal_leftGrip);
            GUI.Label(new Rect(x, y + (++yPos) * lineHeight, 500, 500), "raw Value right " + port_rightGrip + ": " + rawVal_right);
            GUI.Label(new Rect(x, y + (++yPos) * lineHeight, 500, 500), "norm Value right: " + normalizedVal_rightGrip);
            GUI.Label(new Rect(x, y + (++yPos) * lineHeight, 500, 500), "raw Value ripcord " + port_ripcord + " : " + rawVal_ripcord);
            GUI.Label(new Rect(x, y + (++yPos) * lineHeight, 500, 500), "norm Value ripcord: " + normalizedVal_ripcord);
            if (enableKeyboard)
            {
                GUI.Label(new Rect(x, y + (++yPos) * lineHeight, 500, 500), "keyboard Vertical " + resultUpDownMinus1To1);
                GUI.Label(new Rect(x, y + (++yPos) * lineHeight, 500, 500), "keyboard Horizontal" + resultLeftRightMinus1To1);
            }
        }
    }

    float FilterValueWithDeadzone(float curVal)
    {
        if (curVal > deadzone)
        {

            curVal = (curVal - deadzone) / (1.0f - deadzone);
        }
        else if (curVal < -deadzone)
        {
            curVal = (curVal + deadzone) / (1.0f - deadzone);
        }
        else
        {
            curVal = 0;
        }

        return curVal;
    }

    float Remap(float _val, float _minIn, float _maxIn, float _minOut, float _maxOut)
    {
        float newVal = _minOut + (_maxOut - _minOut) * (_val - _minIn) / (_maxIn - _minIn);
        return Mathf.Clamp(newVal, _minOut, _maxOut);
    }

    // move to left or right
    public float GetResultLeftRightMinus1To1()
    {
        return resultLeftRightMinus1To1;
    }

    // move to up or down
    public float GetResultUpDownMinus1To1()
    {
        return resultUpDownMinus1To1;
    }
}
*/
