﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : DCGElement {
    public List<Edge> edges;
    public List<DCGElement> elements;   //elements this point is a part of
    public Vector3 position;

    public Point(Vector3 position)
    {
        this.position = position;
        edges = new List<Edge>();
        elements = new List<DCGElement>();
        DCGBase.points.Add(this);
    }

    public override void Render()
    {
        Graphics.DrawMeshNow(GeometryUtil.icoSphere2, Matrix4x4.TRS(this.position, Quaternion.identity, new Vector3(.007f, .007f, .007f)));
    }

    public override void Remove()
    {
        foreach (Edge e in edges)
            if (e.points[0] == this || e.points[e.points.Count] == this)
                e.Remove();
            else
                e.points.Remove(this);
        DCGBase.points.Remove(this);
    }

    public void setPosition(Vector3 value)
    {
        position = value;
        int moveID = DCGBase.nextMoveID();
        lastMoveID = moveID;

        foreach (Edge e in edges) if (e.lastMoveID != moveID)
            {
                e.lastMoveID = moveID;
                if (e.smooth)
                    e.updateMesh();
                foreach (Face f in e.faces) if (f.lastMoveID != moveID)
                    {
                        f.lastMoveID = moveID;
                        f.updateMesh();
                        foreach (Solid s in f.solids) if (s.lastMoveID != moveID)
                            {
                                s.lastMoveID = moveID;
                            }
                    }
            }

        foreach (DCGElement e in elements) if (e.lastMoveID != moveID)
            {
                e.lastMoveID = moveID;
                e.Update();
            }
    }
}
