﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap;
public struct GestureEventArgs
{

}
public delegate void GestureEventHandler();

public class handController: Controller {
    public Leap.Controller trackedController;
    private LeapServiceProvider leapProvider;
    public bool pinchHeld = false;
    public bool grabHeld = false;

    public event GestureEventHandler pinchMade;
    public event GestureEventHandler pinchGone;
    public event GestureEventHandler grabMade;
    public event GestureEventHandler grabGone;
    // Use this for initialization
    void Start () {
        leapProvider = FindObjectOfType<LeapServiceProvider>();
        if (leapProvider == null)
            Debug.LogError("No Leap Provider found!");
        pinchMade += TriggerDown;
        pinchGone += TriggerUp;
        currentTool = tools[0];
    }
    public void OnPinchHeld()
    {
        if (pinchMade != null)
            pinchMade();
    }
    public void OnPinchGone()
    {
        if (pinchGone != null)
            pinchGone();
    }

    // Update is called once per frame
    void Update () {
        //Check to see if a pinch is being held. 
        if (checkPinch())
        {
            //Debug.Log("Pinch is Held");
            OnPinchHeld();
            currentTool.triggerInput = true;
        }
        else
        {
            //Debug.Log("Pinch is Gone");
            OnPinchGone();
            currentTool.triggerInput = false;
        }

    }
    protected void TriggerDown()
    {
        currentTool.TriggerDown();
    }

    protected void TriggerUp()
    {
        currentTool.TriggerUp();
    }
    public bool checkPinch()
    {

        Leap.Frame frame = leapProvider.CurrentFrame;
        Leap.Hand rhand;
        if(frame.Hands.Count > 0)
        {
            //Debug.Log("Catds");
            rhand = frame.Hands[0];
            if (rhand.IsRight)
            {
                foreach (Leap.Finger f in rhand.Fingers)
                {
                    if (f.Type == Leap.Finger.FingerType.TYPE_INDEX)
                    {
                        currentTool.setPos(f.TipPosition.ToVector3());
                        //Debug.Log(currentTool.getPos().ToString());
                    }
                }
                if (rhand.PinchStrength >= 1f) return true;
            }
            //If the first element in the array is not the right hand check
            //to see if therer is another hand out there, it must be the right.
            else if(frame.Hands.Count > 1)
            {
                rhand = frame.Hands[1];
                foreach (Leap.Finger f in rhand.Fingers)
                {
                    if (f.Type == Leap.Finger.FingerType.TYPE_INDEX)
                    {
                        currentTool.setPos(f.TipPosition.ToVector3());
                    }
                }
                if (rhand.PinchStrength >= 1f) return true;
            }
        }
        return false;
    }
}
