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
    // selection mesh
    private MeshFilter _selectionMeshFilter;
    private Rigidbody _selectionMeshRigidbody;
    private MeshCollider _selectionMeshCollider;
    
    private Bounds _cameraBounds;
    private Rect _selectionRect;
    private GameObject _pointer;

    private void Start() {
        _selectionStarted = false;
        _selectedObjectsIndex = new List<int>();
        _dsi = GetComponent<DrawSelectionIndicator>();
        _selectionMeshFilter = selectionMesh.GetComponent<MeshFilter>();
        _pointer = new GameObject();
        _pointer.name = "pointer for unit selection";
        // selection mesh
        CreatePrimitiveMesh.GenerateBoxMesh(_selectionMeshFilter);
        _selectionMeshRigidbody = selectionMesh.AddComponent<Rigidbody>();
        _selectionMeshRigidbody.useGravity = false;
        _selectionMeshCollider = selectionMesh.AddComponent<MeshCollider>();
        _selectionMeshCollider.convex = true;
        _selectionMeshCollider.isTrigger = true;
        _selectionMeshCollider.sharedMesh = _selectionMeshFilter.mesh;
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

    private void UpdateSelectionMeshValues() {
        _selectionMeshCollider.sharedMesh = null;
        SelectionMeshVerticesCalc smvc = new SelectionMeshVerticesCalc(mainCamera, _selectionRect);
        // == vertices of the mesh ==
        Vector3[] vertices = new Vector3[] {
            // Bottom
            smvc.p0, smvc.p1, smvc.p2, smvc.p3,
            // Left
            smvc.p7, smvc.p4, smvc.p0, smvc.p3,
            // Front
            smvc.p4, smvc.p5, smvc.p1, smvc.p0,
            // Back
            smvc.p6, smvc.p7, smvc.p3, smvc.p2,
            // Right
            smvc.p5, smvc.p6, smvc.p2, smvc.p1,
            // Top
            smvc.p7, smvc.p6, smvc.p5, smvc.p4
        };
        _selectionMeshFilter.mesh.vertices = vertices;
        _selectionMeshCollider.sharedMesh = _selectionMeshFilter.mesh;
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
