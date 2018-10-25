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

        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
        }

        SmombieGame.GetInstance().callback += OnMessageFromSmombieGame;

        udpListener = new UDPListener();
        udpListener.MessageReceived += OnMessageFromUDP;

        udpListener.Start(listenerPort);
    }

    public void OnMessageFromUDP(object sender, string e)
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

        if (e == "reset" || e == "screensaver" || e == "intro")
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
        else if (e == "hidecamera")
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => SmartphoneCamera.GetInstance().showView(false) );

        }
        else if (e == "showcamera")
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => SmartphoneCamera.GetInstance().showView(true));
        }
    }

    public void OnMessageFromSmombieGame(object sender, string e)
    {
        if(e == "fotoenter")
        {
            SendMessage("cameraapp=visible");
            SmartphoneCamera.GetInstance().showView(true);
        }
        else if(e == "fotoexit")
        {
            SendMessage("cameraapp=hidden");
            SmartphoneCamera.GetInstance().showView(false);
        }
        else
        {
            SendMessage("event=" + e);
        }
    }
    void SendMessage(string txt)
    {
        UDPSender.SendUDPStringASCII("127.0.0.1", senderPort, txt);

    }
}
