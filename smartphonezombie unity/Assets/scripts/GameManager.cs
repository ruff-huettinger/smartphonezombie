using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    UDPListener udpListener;
    UDPSender udpSender;
    private int senderPort;
    private int listenerPort;

    // Use this for initialization
    void Start () {
        senderPort = (int)Configuration.GetInnerTextByTagName("senderPort", 5555);
        listenerPort = (int)Configuration.GetInnerTextByTagName("listenerPort", 4444);

        udpListener = new UDPListener();
        udpListener.MessageReceived += OnMessage;

        udpListener.Start(listenerPort);
    }

    public void OnMessage(object sender, string e)
    {
        if (e.IndexOf("mousewheel=") > -1)
        {
            e = e.Replace("mousewheel=", "");
            double delta = Convert.ToDouble(e);
            InputManager.GetInstance().mouseWheel = (float)delta;
        }
        else
        {
            InputManager.GetInstance().mouseWheel = 0;
        }

        if (e == "screensaver" || e == "intro")
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => SmombieGame.GetInstance().GAMEreset() );
        }
        if (e == "startgame")
        {
            if (SmombieGame.GetInstance().state != SmombieGame.STATE.ATSTART)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => SmombieGame.GetInstance().GAMEstart() );
            }
        }
        else if (e == "startplay")
        {
            if (SmombieGame.GetInstance().state != SmombieGame.STATE.PLAYING)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => SmombieGame.GetInstance().GAMEstartPlaying());
            }
        }
    }
}
