using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(DrawSelectionIndicator))]
public class SelectionManager : MonoBehaviour {
    public static List<Selectable> selectables = new List<Selectable>();
    public Color selectionColor;
    public GameObject selectionMesh;
    public Camera mainCamera;
    public LayerMask selectionLayers;
    
    private DrawSelectionIndicator _dsi;
    private bool _selectionStarted;
    private Vector3 _mousePosition1;
    private List<Selectable> _selectedObjects;

    // selection mesh
    private MeshFilter _selectionMeshFilter;
    private Rigidbody _selectionMeshRigidbody;
    private MeshCollider _selectionMeshCollider;

    private SelectionMeshVerticesCalc _smvc;
    private Bounds _cameraBounds;
    private Rect _selectionRect;
    private GameObject _pointer;
    
    
    private void Start() {
        _selectionStarted = false;
        _selectedObjects = new List<Selectable>();
        _dsi = GetComponent<DrawSelectionIndicator>();
        _selectionMeshFilter = selectionMesh.GetComponent<MeshFilter>();
        _pointer = new GameObject();
        _pointer.name = "pointer for unit selection";
        _smvc = new SelectionMeshVerticesCalc(mainCamera, _pointer);
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
            _selectedObjects.Clear();
            // get the rectangle of the player selection
            _selectionRect = _dsi.GetScreenSelectionRectangle(_mousePosition1, Input.mousePosition);
            // the selection mesh cannot be too thin as this causes an error with Unity ( the mesh is no longer considered convex )
            if (Vector3.Distance(_mousePosition1, Input.mousePosition) > 5 && _mousePosition1.x != Input.mousePosition.x && _mousePosition1.y != Input.mousePosition.y) {
                selectionMesh.SetActive(true);
                UpdateSelectionMeshValues();
            } else {
                Vector3 middle = Vector3.Lerp(_mousePosition1, Input.mousePosition, 0.5f);
                Ray ray = mainCamera.ScreenPointToRay(middle);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectionLayers)) {
                    Selectable selectable = hit.collider.gameObject.GetComponent<Selectable>();
                    if (selectables.Contains(selectable)) { // makes sure the selectable is enabled and can be added to the selected list
                        addToSelection(selectable);
                    }
                }
                // Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red);
            }
            for (int i = 0; i < selectables.Count; i++) {
                _cameraBounds = GetViewportBounds(_mousePosition1, Input.mousePosition);
                if (_cameraBounds.Contains(mainCamera.WorldToViewportPoint(selectables[i].transform.position))) {
                    addToSelection(selectables[i]);
                }
            }
        } else {
            selectionMesh.SetActive(false);
        }
    }

    // adds an element to the selection
    public void addToSelection(Selectable selectable) {
        if (_selectedObjects.Contains(selectable) == false) {
            _selectedObjects.Add(selectable);
        }
    }

    // adds the selectable to the selectable list
    public void addToSelectables(Selectable selectable) {
        if (selectables.Contains(selectable) == false) {
            selectables.Add(selectable);
        }
    }

    // removes the unit from the selectables and ensures that the unit is no longer selected either
    public void removeFromSelectables(Selectable selectable) {
        if (selectables.Contains(selectable)) {
            selectables.Remove(selectable);
        }
        if (_selectedObjects.Contains(selectable)) {
            _selectedObjects.Remove(selectable);
        }
    }
    
    // update the position of the vertices of the selection mesh according to the user's mouse position on screen
    private void UpdateSelectionMeshValues() {
        _smvc.calc(_selectionRect);
        _selectionMeshCollider.sharedMesh = null;
        // == vertices of the mesh ==
        Vector3[] vertices = new Vector3[] {
            // Bottom
            _smvc.p0, _smvc.p1, _smvc.p2, _smvc.p3,
            // Left
            _smvc.p7, _smvc.p4, _smvc.p0, _smvc.p3,
            // Front
            _smvc.p4, _smvc.p5, _smvc.p1, _smvc.p0,
            // Back
            _smvc.p6, _smvc.p7, _smvc.p3, _smvc.p2,
            // Right
            _smvc.p5, _smvc.p6, _smvc.p2, _smvc.p1,
            // Top
            _smvc.p7, _smvc.p6, _smvc.p5, _smvc.p4
        };
        _selectionMeshFilter.mesh.vertices = vertices;
        _selectionMeshCollider.sharedMesh = _selectionMeshFilter.mesh;
    }

    void OnGUI() {
        if (_selectionStarted) {
            // draw the player's selection rectangle
            _dsi.DrawScreenRectBorder(_selectionRect, 2, selectionColor);
        }
        // Draw selection edges
        if (_selectedObjects.Count > 0) {
            for (int i = 0; i < _selectedObjects.Count; i++) {
                _dsi.DrawIndicator(mainCamera, _selectedObjects[i].GetObjectBounds());
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
