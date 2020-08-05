using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DrawSelectionIndicator))]
public class SelectionManager : MonoBehaviour {
    public static List<Selectable> selectables = new List<Selectable>();
    public Color selectionColor;
    public GameObject selectionMesh;
    public Camera mainCamera;

    private DrawSelectionIndicator _dsi;
    private bool _selectionStarted;
    private Vector3 _mousePosition1;
    private List<int> _selectedObjectsIndex;
    private MeshFilter _selectionBoxMeshFilter;
    private Bounds _cameraBounds;
    private Rect _selectionRect;
    private GameObject _pointer;

    private void Start() {
        _selectionStarted = false;
        _selectedObjectsIndex = new List<int>();
        _dsi = GetComponent<DrawSelectionIndicator>();
        _selectionBoxMeshFilter = selectionMesh.GetComponent<MeshFilter>();
        CreatePrimitiveMesh.GenerateBoxMesh(_selectionBoxMeshFilter);
        _pointer = new GameObject();
        _pointer.name = "pointer for unit selection";
        _pointer.transform.parent = transform.parent;
    }
    
    // Update is called once per frame
    void Update() {
        // Begin selection
        if (Input.GetMouseButtonDown(0)) {
            _selectionStarted = true;
            _mousePosition1 = Input.mousePosition;
        }
        // End selection
        if (Input.GetMouseButtonUp(0)) {
            _selectionStarted = false;
        }
        // Detect which Objects are inside selection rectangle
        if (_selectionStarted) {
            _selectedObjectsIndex.Clear();
            // get the rectangle of the player selection
            _selectionRect = _dsi.GetScreenSelectionRectangle(_mousePosition1, Input.mousePosition);
            UpdateSelectionMeshValues();
            for (int i = 0; i < selectables.Count; i++) {
                _cameraBounds = GetViewportBounds(_mousePosition1, Input.mousePosition);
                if (_cameraBounds.Contains(mainCamera.WorldToViewportPoint(selectables[i].transform.position))) {
                    _selectedObjectsIndex.Add(i);
                }
            }
        }
    }

    // gets the ratios of the selection's rectangle ( ratios go from 0 to 1 )
    // the ratios are then projected on the clipping planes in order to get the selection Mesh
    private void UpdateSelectionMeshValues() {
        // == get the ratios of the 4 points of the selection rectangle ==
        // top right
        Vector2 p0Ratio = new Vector2(_selectionRect.xMax / Screen.width,
                                      _selectionRect.yMin / Screen.height);
        // top left
        Vector2 p1Ratio = new Vector2(_selectionRect.xMin / Screen.width,
                                      _selectionRect.yMin / Screen.height);
        // bottom left
        Vector2 p2Ratio = new Vector2(_selectionRect.xMin / Screen.width,
                                      _selectionRect.yMax / Screen.height);
        // bottom right
        Vector2 p3Ratio = new Vector2(_selectionRect.xMax / Screen.width,
                                      _selectionRect.yMax / Screen.height);

        // == apply selection rectangle on near clip plane ==
        GetClipPlanePoints.ClipPlanePoints nearClipPlanePoints = GetClipPlanePoints.getClipPlanePoints(mainCamera, mainCamera.nearClipPlane);
        float nearPlaneWidth = Vector3.Distance(nearClipPlanePoints.LowerLeft, nearClipPlanePoints.LowerRight);
        float nearPlaneHeight = Vector3.Distance(nearClipPlanePoints.UpperRight, nearClipPlanePoints.LowerRight);
        _pointer.transform.eulerAngles = mainCamera.transform.eulerAngles;
        // p0
        _pointer.transform.position = nearClipPlanePoints.LowerLeft;
        _pointer.transform.Translate(nearPlaneWidth * p0Ratio.x, nearPlaneHeight * p0Ratio.y, 0);
        Vector3 p0 = _pointer.transform.position;
        // p1
        _pointer.transform.position = nearClipPlanePoints.LowerLeft;
        _pointer.transform.Translate(nearPlaneWidth * p1Ratio.x, nearPlaneHeight * p1Ratio.y, 0);
        Vector3 p1 = _pointer.transform.position;
        // p2
        _pointer.transform.position = nearClipPlanePoints.LowerLeft;
        _pointer.transform.Translate(nearPlaneWidth * p2Ratio.x, nearPlaneHeight * p2Ratio.y, 0);
        Vector3 p2 = _pointer.transform.position;
        // p3
        _pointer.transform.position = nearClipPlanePoints.LowerLeft;
        _pointer.transform.Translate(nearPlaneWidth * p3Ratio.x, nearPlaneHeight * p3Ratio.y, 0);
        Vector3 p3 = _pointer.transform.position;
        // == apply selection rectangle on far clip plane ==
        GetClipPlanePoints.ClipPlanePoints farClipPlanePoints = GetClipPlanePoints.getClipPlanePoints(mainCamera, mainCamera.farClipPlane);
        float farPlaneWidth = Vector3.Distance(farClipPlanePoints.LowerLeft, farClipPlanePoints.LowerRight);
        float farPlaneHeight = Vector3.Distance(farClipPlanePoints.UpperRight, farClipPlanePoints.LowerRight);
        _pointer.transform.eulerAngles = mainCamera.transform.eulerAngles;
        // p4
        _pointer.transform.position = farClipPlanePoints.LowerLeft;
        _pointer.transform.Translate(farPlaneWidth * p0Ratio.x, farPlaneHeight * p0Ratio.y, 0);
        Vector3 p4 = _pointer.transform.position;
        // p5
        _pointer.transform.position = farClipPlanePoints.LowerLeft;
        _pointer.transform.Translate(farPlaneWidth * p1Ratio.x, farPlaneHeight * p1Ratio.y, 0);
        Vector3 p5 = _pointer.transform.position;
        // p6
        _pointer.transform.position = farClipPlanePoints.LowerLeft;
        _pointer.transform.Translate(farPlaneWidth * p2Ratio.x, farPlaneHeight * p2Ratio.y, 0);
        Vector3 p6 = _pointer.transform.position;
        // p7
        _pointer.transform.position = farClipPlanePoints.LowerLeft;
        _pointer.transform.Translate(farPlaneWidth * p3Ratio.x, farPlaneHeight * p3Ratio.y, 0);
        Vector3 p7 = _pointer.transform.position;
        // == vertices of the mesh ==
        Vector3[] vertices = new Vector3[] {
            // Bottom
            p0, p1, p2, p3,
            // Left
            p7, p4, p0, p3,
            // Front
            p4, p5, p1, p0,
            // Back
            p6, p7, p3, p2,
            // Right
            p5, p6, p2, p1,
            // Top
            p7, p6, p5, p4
        };
        _selectionBoxMeshFilter.mesh.vertices = vertices;
    }
    
    private void UpdateSelectionMesh() {
        Vector2 p0Ratio = new Vector2();
    }
    
    private void OnDrawGizmos() {
        // for (int i = 0; i < selectables.Count; i++) {
        //     Gizmos.color = Color.red;
        //     Bounds bounds = selectables[i].GetObjectBounds();
        //     Gizmos.DrawCube(bounds.center, bounds.size);
        // }
        // Camera camera = Camera.main;
        // Gizmos.color = Color.red;
        // Gizmos.DrawCube(camera.WorldToViewportPoint(_cameraBounds.center), camera.WorldToViewportPoint(_cameraBounds.size));
    }

    void OnGUI() {
        if (_selectionStarted) {
            // draw the player's selection rectangle
            _dsi.DrawScreenRectBorder(_selectionRect, 2, selectionColor);
        }
        // Draw selection edges
        if (_selectedObjectsIndex.Count > 0) {
            for (int i = 0; i < _selectedObjectsIndex.Count; i++) {
                _dsi.DrawIndicator(mainCamera, selectables[_selectedObjectsIndex[i]].GetObjectBounds());
            }
        }
    }

    // Defines a volume such as seen by the camera and delimited by the player
    public Bounds GetViewportBounds(Vector3 screenPosition1, Vector3 screenPosition2) {
        Vector3 v1 = mainCamera.ScreenToViewportPoint(screenPosition1);
        Vector3 v2 = mainCamera.ScreenToViewportPoint(screenPosition2);
        Vector3 min = Vector3.Min(v1, v2);
        Vector3 max = Vector3.Max(v1, v2);
        min.z = mainCamera.nearClipPlane;
        max.z = mainCamera.farClipPlane;
        Bounds bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }
}
