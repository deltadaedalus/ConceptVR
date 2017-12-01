﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTool : Tool {
    public Material destMat;

    float delDist = 0.07f;

	// Use this for initialization
	void Start () {
	}

    private void Update()
    {
        if (triggerInput)
        {
            Point nPoint;
            while (true)
            {
                nPoint = DCGBase.NearestPoint(controllerPosition, delDist);
                if (nPoint == null)
                    break;
                else
                    nPoint.Remove();
            }
        }
    }

    private void OnRenderObject()
    {
        if (triggerInput)
        {
            destMat.SetPass(0);
            Graphics.DrawMeshNow(GeometryUtil.icoSphere4, Matrix4x4.TRS(controllerPosition, Quaternion.identity, new Vector3(delDist, delDist, delDist)));
        }
    }
}