﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solid {
    public List<Face> faces;
    public Mesh mesh;
    public int lastMoveID;


    public Solid(Mesh m, Matrix4x4 t, Vector3 translate)
    {
        mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        ArrayList points = new ArrayList();
        List<int> pointMap = new List<int>();
        faces = new List<Face>();

        foreach (Vector3 v in m.vertices)
        {
            int match = -1;
            Vector3 tv = (Vector3)(t * v) + translate;
            for (int i = 0; i < points.Count; ++i)
            {
                Point p = (Point)points[i];
                if (Vector3.Distance(p.position, tv) <= .0001f)
                {
                    match = i;
                    break;
                }
            }
            if (match == -1)
            {
                verts.Add(tv);
                points.Add(new Point(tv));
                pointMap.Add(points.Count-1);
            } else
            {
                pointMap.Add(match);
            }
        }

        for (int tri = 0; tri < m.triangles.Length; tri += 3)
        {
            tris.Add(m.triangles[tri]);
            tris.Add(m.triangles[tri+1]);
            tris.Add(m.triangles[tri+2]);

            Point p1 = (Point)points[pointMap[m.triangles[tri]]];
            Point p2 = (Point)points[pointMap[m.triangles[tri+1]]];
            Point p3 = (Point)points[pointMap[m.triangles[tri+2]]];

            List<Edge> edges = new List<Edge>();
            edges.Add(new Edge(p1, p2));
            edges.Add(new Edge(p2, p3));
            edges.Add(new Edge(p3, p1));

            faces.Add(new Face(edges));
        }

        mesh.SetVertices(verts);
        mesh.SetTriangles(tris.ToArray(), 0);

        DCGBase.solids.Add(this);
    }


    public void updateMesh()
    {

    }
}
