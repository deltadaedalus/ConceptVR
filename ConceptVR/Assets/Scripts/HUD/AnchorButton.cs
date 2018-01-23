﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;

public delegate void ButtonPressEventHandler();
public class AnchorButton : ToggleButton {
	public delegate void ButtonPressEventHandler();
	public event ButtonPressEventHandler Anchor;
	public bool active;

	// Use this for initialization
	void Start () {
		active = false;
		base.Start ();
	}
	
	void OnPress(){
		if (active) {
			active = false;
		}
		active = true;
		Anchor ();
	}
}