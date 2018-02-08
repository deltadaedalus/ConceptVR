﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using System.Linq;

public class FreeFormTool : Tool {
    #region Member Berries
    private Vector3 startPos;
	private Vector3 endPos;
	private List<Vector3> rightPoints;
	private List<Vector3> leftPoints;
    private List<Vector3> finalPoints;
    private HandsUtil util;
    private int frameCount = 0;
    private LineRenderer rightFreeFormLine;
    private LineRenderer freeFormLine;
    private List<Point> backFacePoints;
    private List<Point> frontFacePoints;
    private LeapTrackedController leapControl;
    private List<Face> faces;
    #endregion

    // Use this for initialization
    new void Start () {
		rightPoints = new List<Vector3> ();
		leftPoints = new List<Vector3> ();
        util = new HandsUtil();
        finalPoints = new List<Vector3>();
        backFacePoints = new List<Point>();
        frontFacePoints = new List<Point>();
    }
	
	// Update is called once per frame
	new void Update () {
        if (formInput)
        {
            if(Hands.Right == null || Hands.Left == null)
            {
                Destroy(freeFormLine.gameObject);
                Destroy(rightFreeFormLine.gameObject);
                leapControl.freeFormFailureHandler();
            }
            if (frameCount >=17)
            {
                Debug.Log(frameCount);
                //Add points to the line renderer and the point lists
                Vector3 rightPos = Hands.Right.PalmPosition.ToVector3();
                Vector3 leftPos = Hands.Left.PalmPosition.ToVector3();
                if (rightPos.y == 0f || leftPos.y == 0f)
                    return;
                #region Line Renderer adding
                freeFormLine.positionCount++;
                freeFormLine.SetPosition(freeFormLine.positionCount-1, leftPos);
                rightFreeFormLine.positionCount++;
                rightFreeFormLine.SetPosition(rightFreeFormLine.positionCount-1, rightPos);
                rightPoints.Add(rightPos);
                leftPoints.Add(leftPos);
                #endregion
                frameCount = 0;
            }
            else frameCount++;
        }
	}
    public override void FreeForm(LeapTrackedController ltc)
    {
        leapControl = ltc;
        Vector3 rightPos = Hands.Right.PalmPosition.ToVector3();
        Vector3 leftPos = Hands.Left.PalmPosition.ToVector3();
        #region Line Renderer Initialize
        GameObject go = new GameObject();
        go.transform.position = rightPos;
        rightFreeFormLine = go.AddComponent<LineRenderer>();
        rightFreeFormLine.startWidth = .01f;
        rightFreeFormLine.endWidth = .01f;
        GameObject gol = new GameObject();
        gol.transform.position = leftPos;
        freeFormLine = gol.AddComponent<LineRenderer>();
        freeFormLine.startWidth = .01f;
        freeFormLine.endWidth = .01f;
        freeFormLine.positionCount++;
        rightFreeFormLine.SetPosition(0, rightPos);
        freeFormLine.SetPosition(0, leftPos);
        #endregion
        rightPoints.Add(rightPos);
        leftPoints.Add(leftPos);
    }
    public override void FreeFormEnd()
    {
        #region Initial and end Point removal
        rightPoints.RemoveAt(0);
        leftPoints.RemoveAt(0);
        rightPoints.RemoveAt(rightPoints.Count-1);
        leftPoints.RemoveAt(leftPoints.Count-1);
        #endregion
        Bezerp();
        generateFreeFormSolidCubic();
        #region Clean up
        Destroy(freeFormLine.gameObject);
        Destroy(rightFreeFormLine.gameObject);
        rightPoints.Clear();
        leftPoints.Clear();
        finalPoints.Clear();
        backFacePoints.Clear();
        frontFacePoints.Clear();
        #endregion

    }
    /*  Bezerp
     *  Input - none
     *  Output - Start & end Bezier curves
     *  Creates a minor curve connecting the start and end positions of a free form.
     */
    public void Bezerp()
    {
        #region Start Curve Bezier
        Vector3 virtL = leftPoints[1] + (leftPoints[1] - leftPoints[2]);
        Vector3 virtR = rightPoints[0] + (rightPoints[0] - rightPoints[1]);
        Vector3[] startVerts = { leftPoints[0], virtL, virtR, rightPoints[0] };
        rightPoints.Insert(0,GeometryUtil.Bezerp(startVerts,.75f));
        rightPoints.Insert(0, GeometryUtil.Bezerp(startVerts, .5f));
        rightPoints.Insert(0, GeometryUtil.Bezerp(startVerts, .25f));
        #endregion

        #region End Curve Bezier
        virtL = leftPoints[leftPoints.Count-1] + (leftPoints[leftPoints.Count - 1] - leftPoints[leftPoints.Count - 2]);
        virtR = rightPoints[rightPoints.Count - 1] + (rightPoints[rightPoints.Count - 1] - rightPoints[rightPoints.Count - 2]);
        Vector3[] endVerts = { leftPoints[leftPoints.Count - 1], virtL, virtR, rightPoints[rightPoints.Count - 1] };

        rightPoints.Add(GeometryUtil.Bezerp(endVerts, .75f));
        rightPoints.Add(GeometryUtil.Bezerp(endVerts, .5f));
        rightPoints.Add(GeometryUtil.Bezerp(endVerts, .25f));
        #endregion
        finalPoints.AddRange(rightPoints);
        finalPoints.AddRange(Enumerable.Reverse(leftPoints));

    }
    /*  generateFreeFormSolidCubic
     *  Input - none
     *  Output - Generated Solid
     *  Takes the drawn line made by the user, and turns it into a cubic solid
     */
    public void generateFreeFormSolidCubic()
    {
        Vector3 midPoint = findCenter(finalPoints);
        Vector3 backMid = normalize(finalPoints)/4;
        Debug.Log("mid " + midPoint);
        Vector3 backDiff = midPoint - backMid;
        Debug.Log("midb " + backMid);
        Debug.Log("backF " + backDiff);
        Vector3 frontMid = midPoint + backDiff;
        #region Back Face Generation
        foreach (Vector3 v in finalPoints)
            backFacePoints.Add(new Point(v - backMid));
        List<Edge> backEdges = new List<Edge>();
        backEdges.Add(new Edge(backFacePoints[backFacePoints.Count - 1], backFacePoints[0]));
        for(int p =0; p< backFacePoints.Count - 1; ++p)
        {
            backEdges.Add(new Edge(backFacePoints[p], backFacePoints[p+1]));
        }
        new Face(backEdges);
        #endregion

        #region Front Face Generation
        foreach (Vector3 v in finalPoints)
            frontFacePoints.Add(new Point(v));
        List<Edge> frontEdges = new List<Edge>();
        frontEdges.Add(new Edge(frontFacePoints[frontFacePoints.Count - 1], frontFacePoints[0]));
        for (int p = 0; p < frontFacePoints.Count - 1; ++p)
        {
            frontEdges.Add(new Edge(frontFacePoints[p], frontFacePoints[p + 1]));
        }
        new Face(frontEdges);
        #endregion

        #region Side Edges and Face Generation
        List<Edge> sideEdges = new List<Edge>();
        for (int i = 0; i < frontFacePoints.Count ; ++i)
        {
            sideEdges.Add(new Edge(frontFacePoints[i], backFacePoints[i]));
        }
        List<Edge> tempList = new List<Edge>();
        for (int i =0; i < sideEdges.Count; ++i)
        {
            if (i == sideEdges.Count - 1)
            {
                tempList = new List<Edge>() { sideEdges[0], frontEdges[i-1], sideEdges[i], backEdges[i-1] };
            }
            else
            {
                tempList = new List<Edge>() {  frontEdges[i], backEdges[i],sideEdges[i+1], sideEdges[i] };
            }
            new Face(tempList);
                
        }
        //new Solid(faces);
        #endregion
        
    }
    public Vector3 findCenter(List<Vector3> vecs)
    {
        float x = 0f;
        float y = 0f;
        float z = 0f;
        foreach(Vector3 v in vecs)
        {
            x += v.x;
            y += v.y;
            z += v.z;
        }
        return new Vector3(x / vecs.Count, y / vecs.Count, z / vecs.Count);
    }
    public Vector3 normalize(List<Vector3> vecs)
    {
        Vector3 sum = new Vector3();
        Vector3 diffA = vecs[1] - vecs[0];
        Vector3 diffB = new Vector3();
        for(int i = 1; i< vecs.Count-1; ++i)
        {
            diffB = vecs[i+1] - vecs[i];
            sum += Vector3.Cross(diffA, diffB).normalized;
            diffA = diffB;
        }
        sum += Vector3.Cross(diffA,(vecs[vecs.Count-1]-vecs[0])).normalized;
        return sum.normalized;

    }
}
