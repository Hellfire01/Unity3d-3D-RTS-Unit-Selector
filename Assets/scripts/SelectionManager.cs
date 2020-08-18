using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(DrawSelectionIndicator))]
public class SelectionManager : MonoBehaviour {
    [Tooltip("this array containes all of the Selectables selected by the user")]
    public static List<Selectable> selectables = new List<Selectable>();
    [Tooltip("This is the color that is used for the user's selection")]
    public Color selectionColor;
    [Tooltip("Reference to the main camera. Needed to avoid calling Camera.main each frame")]
    public Camera mainCamera;
    [Tooltip("The selection layer of the Selectable game objects. If multiple are selected, only the last one will be used")]
    public LayerMask selectionLayer;
    
    
    private bool _selectionStarted;

    // mouse related
    private Vector3 _mousePosition1;
    private Vector3 _previousMousePosition;
    
    // draw related
    private DrawSelectionIndicator _dsi;
    private SelectionMeshGetVertices _smvc;
    
    // user selection
    private Bounds _cameraBounds;
    private Rect _selectionRect;
    
    // selection lists
    private List<Selectable> _selectedObjectsByClickDrag;
    private List<Selectable> _selectedObjectsByMeshCollider;
    private List<Selectable> _selectedObjects;

    // selection mesh
    private GameObject _selectionMesh;
    private MeshFilter _selectionMeshFilter;
    private Rigidbody _selectionMeshRigidbody;
    private MeshCollider _selectionMeshCollider;

    private GameObject _pointer;

    private void Start() {
        _selectionStarted = false;
        _dsi = GetComponent<DrawSelectionIndicator>();
        _pointer = new GameObject();
        _pointer.name = "pointer for unit selection";
        // selection arrays
        _selectedObjectsByClickDrag = new List<Selectable>();
        _selectedObjectsByMeshCollider = new List<Selectable>();
        _selectedObjects = new List<Selectable>();
        // mouse position
        _mousePosition1 = Input.mousePosition;
        _previousMousePosition = Input.mousePosition;
        // selection mesh
        _selectionMesh = new GameObject();
        _selectionMesh.name = "SelectionMesh";
        _selectionMesh.layer = (int)Mathf.Log(selectionLayer.value, 2);
        SelectionMeshCollider smc = _selectionMesh.AddComponent<SelectionMeshCollider>();
        smc.selectionManager = this;
        _selectionMeshFilter = _selectionMesh.AddComponent<MeshFilter>();
        _smvc = new SelectionMeshGetVertices(mainCamera, _pointer);
        CreatePrimitiveMesh.GenerateBoxMesh(_selectionMeshFilter);
        _selectionMeshRigidbody = _selectionMesh.AddComponent<Rigidbody>();
        _selectionMeshRigidbody.useGravity = false;
        _selectionMeshCollider = _selectionMesh.AddComponent<MeshCollider>();
        _selectionMeshCollider.convex = true;
        _selectionMeshCollider.isTrigger = true;
        _selectionMeshCollider.sharedMesh = _selectionMeshFilter.mesh;
    }

    private void FixedUpdate() {
        // the list is emptied before the triggers are called again
        if (_selectionStarted) {
            _selectedObjectsByMeshCollider.Clear();
        }
    }

    void Update() {
        // Begin selection
        if (Input.GetMouseButtonDown(0)) {
            _selectionStarted = true;
            _mousePosition1 = Input.mousePosition;
            _previousMousePosition = Input.mousePosition;
        }
        // End selection
        if (Input.GetMouseButtonUp(0)) {
            _selectionStarted = false;
        }
        if (_selectionStarted) {
            _selectedObjects.Clear();
            _selectedObjectsByClickDrag.Clear();
            _selectionRect = _dsi.GetScreenSelectionRectangle(_mousePosition1, Input.mousePosition);
            calculateSelectablesByMeshOrRaycast();
            _cameraBounds = GetViewportBounds(_mousePosition1, Input.mousePosition);
            for (int i = 0; i < selectables.Count; i++) {
                if (_cameraBounds.Contains(mainCamera.WorldToViewportPoint(selectables[i].transform.position))) {
                    addAsSelectedByClickDrag(selectables[i]);
                }
            }
            _selectedObjects = new List<Selectable>(_selectedObjectsByClickDrag);
            for (int i = 0; i < _selectedObjectsByMeshCollider.Count; i++) {
                if (_selectedObjects.Contains(_selectedObjectsByMeshCollider[i]) == false) {
                    _selectedObjects.Add(_selectedObjectsByMeshCollider[i]);
                }
            }
            _previousMousePosition = Input.mousePosition;
        } else {
            _selectionMesh.SetActive(false);
        }
    }

    void OnGUI() {
        if (_selectionStarted) {
            // draw the player's selection rectangle
            _dsi.DrawScreenRectBorder(_selectionRect, 2, selectionColor);
        }
        // Draw selection edges
        for (int i = 0; i < _selectedObjects.Count; i++) {
            _dsi.DrawIndicator(mainCamera, _selectedObjects[i].GetObjectBounds());
        }
    }

    #region add and remove from selection

    // adds an element to the selection
    public void addAsSelectedByClickDrag(Selectable selectable) {
        if (selectables.Contains(selectable) && _selectedObjectsByClickDrag.Contains(selectable) == false) {
            _selectedObjectsByClickDrag.Add(selectable);
        }
    }
    
    // adds an element to the selection
    public void addAsSelectedByUserByMeshCollider(Selectable selectable) {
        if (selectables.Contains(selectable) && _selectedObjectsByMeshCollider.Contains(selectable) == false) {
            _selectedObjectsByMeshCollider.Add(selectable);
        }
    }

    // adds the selectable to the selectable list
    public void addAsSelectable(Selectable selectable) {
        if (selectables.Contains(selectable) == false) {
            selectables.Add(selectable);
        }
    }

    // removes the unit from the selectables and ensures that the unit is no longer selected either
    public void removeFromSelectables(Selectable selectable) {
        selectables.Remove(selectable);
        _selectedObjectsByClickDrag.Remove(selectable);
        _selectedObjectsByMeshCollider.Remove(selectable);
    }
    
    #endregion

    #region calculate selectables by raycast or mesh collision

    private void calculateSelectablesByMeshOrRaycast() {
        // should the user's selection be too small or thin, the shape of the mesh is no lon ger convex and this causes
        //     an error. In this instance, a raycast is used instead. This allows a normal selection without the user
        //     noticing the difference
        // Note : normally comparing floats directly is a bad practice due to precision loss but in this case, the mesh
        //     has an error only if the values are exactly the same ( the cube becomes a plane ). This is a very very rare case
        if (Vector3.Distance(_mousePosition1, Input.mousePosition) > 5
            && _mousePosition1.x != Input.mousePosition.x
            && _mousePosition1.y != Input.mousePosition.y) {
            // do not recalculate mesh if the Input.mousePosition did not change
            if (Input.mousePosition != _previousMousePosition) {
                _selectionMesh.SetActive(true);
                UpdateSelectionMeshValues();
            }
        } else {
            // the raycast is set at the center of the user's selection
            Vector3 middle = Vector3.Lerp(_mousePosition1, Input.mousePosition, 0.5f);
            Ray ray = mainCamera.ScreenPointToRay(middle);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectionLayer)) {
                Selectable selectable = hit.collider.gameObject.GetComponent<Selectable>();
                addAsSelectedByClickDrag(selectable);
            }
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

    #endregion
    
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
