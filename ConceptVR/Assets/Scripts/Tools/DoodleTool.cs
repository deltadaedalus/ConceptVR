﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DoodleTool : Tool {
    LineRenderer currLineRend; //Line Renderer
    public Material material;
    Doodle doodle;
    public GameObject doodPrefab;
    int frameCount = 0;
    float width;
    void Start () {
        width = .5f;
	}
	
	// Update is called once per frame
	new void Update () {
        //if(transform.Find("CurrentDoodleFrame").)
        if (triggerInput && currLineRend != null && doodle != null && controllerPosition != new Vector3(0,0,0))
        {
            if (frameCount == 0)
            {
                doodle.CmdUpdateLineRenderer(controllerPosition);
                frameCount = 3;
            }
            else
                frameCount--;

        }
        
	}
    public override bool TriggerDown()
    {
        GameObject go = Instantiate(ItemBase.itemBase.DoodlePrefab,controllerPosition,new Quaternion(0,0,0,0));
        ItemBase.Spawn(go);
        doodle = go.GetComponent<Doodle>();
        currLineRend = go.GetComponent<LineRenderer>();
        return true;
    }
    public override bool TriggerUp()
    {
        if(doodle == false)
        {
            Debug.Log("Wut");
        }
        doodle.isFinished = true;
        return true;
    }
}
