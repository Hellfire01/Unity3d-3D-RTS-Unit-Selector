using System.Collections.Generic;
using UnityEngine;

public class SelectionMeshVerticesCalc {
    public Vector2 p0Ratio;
    public Vector2 p1Ratio;
    public Vector2 p2Ratio;
    public Vector2 p3Ratio;

    public Vector3 p0;
    public Vector3 p1;
    public Vector3 p2;
    public Vector3 p3;
    public Vector3 p4;
    public Vector3 p5;
    public Vector3 p6;
    public Vector3 p7;

    private Camera _mainCamera;
    private Rect _selectionRect;
    private GameObject _pointer;
    
    public SelectionMeshVerticesCalc(Camera mainCamera, Rect selectionRect) {
        _mainCamera = mainCamera;
        _selectionRect = selectionRect;
        _pointer = new GameObject();
        _pointer.name = "pointer for pos of vertices of select mesh";
        getRatios();
        getVertices(out p0, out p1, out p2, out p3, _mainCamera.nearClipPlane + 0.1f);
        getVertices(out p4, out p5, out p6, out p7, _mainCamera.farClipPlane);
    }

    // gets the ratios of the selection's rectangle ( ratios go from 0 to 1 )
    private void getRatios() {
        // top right
        p0Ratio = new Vector2(_selectionRect.xMax / Screen.width,
                              1 - _selectionRect.yMin / Screen.height);
        // top left
        p1Ratio = new Vector2(_selectionRect.xMin / Screen.width,
                              1 - _selectionRect.yMin / Screen.height);
        // bottom left
        p2Ratio = new Vector2(_selectionRect.xMin / Screen.width,
                              1 - _selectionRect.yMax / Screen.height);
        // bottom right
        p3Ratio = new Vector2(_selectionRect.xMax / Screen.width,
                              1 - _selectionRect.yMax / Screen.height);
        // Debug.Log(p0Ratio + ", " + p1Ratio + ", " + p2Ratio + ", " + p3Ratio);
    }

    // apply selection rectangle on given clip plane
    private void getVertices(out Vector3 pA, out Vector3 pB, out Vector3 pC, out Vector3 pD, float distance) {
        distance = Mathf.Clamp(distance, 3f, 100f);
        GetClipPlanePoints.ClipPlanePoints ncpp = GetClipPlanePoints.getClipPlanePoints(_mainCamera, distance);
        float planeWidth = Vector3.Distance(ncpp.LowerLeft, ncpp.LowerRight);
        float planeHeight = Vector3.Distance(ncpp.UpperRight, ncpp.LowerRight);
        _pointer.transform.eulerAngles = _mainCamera.transform.eulerAngles;
        // p0 / p4 / top left
        _pointer.transform.position = ncpp.LowerLeft;
        _pointer.transform.Translate(planeWidth * p2Ratio.x, planeHeight * p2Ratio.y, 0);
        pA = _pointer.transform.position;
        // p1 / p5
        _pointer.transform.position = ncpp.LowerLeft;
        _pointer.transform.Translate(planeWidth * p3Ratio.x, planeHeight * p3Ratio.y, 0);
        pB = _pointer.transform.position;
        // p2 / p6
        _pointer.transform.position = ncpp.LowerLeft;
        _pointer.transform.Translate(planeWidth * p0Ratio.x, planeHeight * p0Ratio.y, 0);
        pC = _pointer.transform.position;
        // p3 / p7
        _pointer.transform.position = ncpp.LowerLeft;
        _pointer.transform.Translate(planeWidth * p1Ratio.x, planeHeight * p1Ratio.y, 0);
        pD = _pointer.transform.position;
    }
}
