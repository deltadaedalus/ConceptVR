﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoodleTool : Tool {
    private LineRenderer currLineRend;//Line Renderer
    private int numClicks = 0;
    public Material material;
    private List<Color> colors = new List<Color>(new Color[] { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta });
    private int colorIndex;
    void Start () {
        colorIndex = 0;
	}
	
	// Update is called once per frame
	new void Update () {
        if (triggerInput && currLineRend != null)
        {
            currLineRend.positionCount = numClicks + 1;
            currLineRend.SetPosition(numClicks, controllerPosition);
            numClicks++;
        }
	}
    public override void TriggerDown()
    {
        GameObject go = new GameObject();
        currLineRend = go.AddComponent<LineRenderer>();
        currLineRend.material = material;
        numClicks = 0;
        //Makes a thinner line
        currLineRend.startWidth = .01f;
        currLineRend.endWidth = .01f;
    }
    public Color changeColor()
    {
        if (colors.Count - 1 == colorIndex)
            colorIndex = -1;
        material.SetColor("_SpecColor", colors[++colorIndex]);
        return colors[colorIndex];
    }
}
