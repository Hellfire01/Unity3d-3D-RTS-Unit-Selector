using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DrawSelectionIndicator))]
public class SelectionManager : MonoBehaviour {
    private DrawSelectionIndicator _dsi;
    private bool _selectionStarted;
    private Vector3 _mousePosition1;
    private List<int> _selectedObjectsIndex;
    private MeshFilter _selectionBoxMeshFilter;
    private Bounds _cameraBounds;
    
    public static List<Selectable> selectables = new List<Selectable>();
    public Color selectionColor;
    public GameObject selectionMesh;
    public Camera mainCamera;
    
    private void Start() {
        _selectionStarted = false;
        _selectedObjectsIndex = new List<int>();
        _dsi = GetComponent<DrawSelectionIndicator>();
        _selectionBoxMeshFilter = selectionMesh.GetComponent<MeshFilter>();
        CreateBoxMesh.GenerateBoxMesh(_selectionBoxMeshFilter);
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
            for (int i = 0; i < selectables.Count; i++) {
                _cameraBounds = GetViewportBounds(_mousePosition1, Input.mousePosition);
                // Debug.Log(_cameraBounds.ToString() + " / " + selectables[i].GetObjectBounds().ToString());
                if (_cameraBounds.Contains(mainCamera.WorldToViewportPoint(selectables[i].transform.position))) {
                    _selectedObjectsIndex.Add(i);
                }
            }
        }
        Debug.Log(Input.mousePosition.ToString());
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
            // get the rectangle of the player selection
            Rect rect = _dsi.GetScreenSelectionRectangle(_mousePosition1, Input.mousePosition);
            // draw the player's selection rectangle
            _dsi.DrawScreenRectBorder(rect, 2, selectionColor);
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
