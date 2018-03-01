﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Item : NetworkBehaviour {
    [SyncVar(hook = "OnSelected")]
    public bool isSelected;
    [SyncVar]
    public bool isLocked;
    public bool destroyed = false;
    protected static HUDManager HUD;
    // Use this for initialization
    protected void Start() {
        HUD = GameObject.Find("Managers").GetComponent<HUDManager>();
        ItemBase.items.Add(this);
    }

    // Update is called once per frame
    void Update() {

    }
    #region Override Functions
    public virtual float Distance(Vector3 pos) { return -1f; }
    public virtual void CmdSelect() { }
    public virtual void CmdDeSelect() { if (destroyed) return; }
    public virtual void Push() { }
    public virtual void changeColor(Color color) { }
    public virtual Vector3 Position(Vector3 contPos) { return new Vector3(); }
    public virtual void SelectUtil() { }
    #endregion
    public static void Pop()
    {
        HUD.Pop();
    }
    public void OnDestroy()
    {
        ItemBase.items.Remove(this);
    }
    public void OnSelected(bool boolean) { this.SelectUtil(); }
}
