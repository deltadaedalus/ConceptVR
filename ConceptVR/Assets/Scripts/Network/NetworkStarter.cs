﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkStarter : MonoBehaviour {
    public bool startHost;
    private NetworkManager netManager;
    private string netAddress;
    private string netPort;
    private bool restart = false;
	// Use this for initialization
	void Start () {
        netManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        netManager.StartHost();
        restart = false;
        netManager.GetComponent<NetworkManagerHUD>().showGUI = false;

    }
	
	// Update is called once per frame
	void Update () {
        if (restart) return;
        if (!netManager.isNetworkActive)
        {
            netManager.GetComponent<NetworkManagerHUD>().showGUI = false;
            netManager.networkAddress = "localhost";
            netManager.networkPort = 53535;

            netManager.StartHost();
        }
    }
    public void connectToHost()
    {
        if (netAddress == null || netPort == null)
            return;
        netManager.StopHost();
        netManager.networkAddress = netAddress;
        netManager.networkPort = int.Parse(netPort);
        netManager.StartClient();
        netManager.GetComponent<NetworkManagerHUD>().showGUI = true;
        startHost = false;
        netAddress = null;
        netPort = null;
    }
    public void setNetAddress(string addr)
    {
        netAddress = addr;
    }
    public void setPort(string port)
    {
        netPort = port;
    }
    public void OnFailedToConnect()
    {
        Debug.Log("Failed to connect to Host");
        startHost = true;
    }
    public void Restart()
    {
        restart = true;
        netManager.StopHost();
    }
}
