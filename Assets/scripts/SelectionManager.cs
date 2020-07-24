using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DrawSelectionIndicator))]
public class SelectionManager : MonoBehaviour {
    private DrawSelectionIndicator _dsi;
    private bool _selectionStarted;
    private Vector3 _mousePosition1;
    private List<int> _selectedObjects;
    public static List<Selectable> selectables = new List<Selectable>();

    private void Start() {
        _selectionStarted = false;
        _selectedObjects = new List<int>();
        _dsi = GetComponent<DrawSelectionIndicator>();
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
            Camera camera = Camera.main;
            _selectedObjects.Clear();
            for (int i = 0; i < selectables.Count; i++) {
                Bounds viewportBounds = _dsi.GetViewportBounds(camera, _mousePosition1, Input.mousePosition);
                if (viewportBounds.Contains(camera.WorldToViewportPoint(selectables[i].transform.position))) {
                    _selectedObjects.Add(i);
                }
            }
        }
    }
    
    void OnGUI() {
        if (_selectionStarted) {
            Rect rect = _dsi.GetScreenSelectionRectangle(_mousePosition1, Input.mousePosition);
            _dsi.DrawScreenRectBorder(rect, 2, Color.cyan);
        }

        // Draw selection edges
        if (_selectedObjects.Count > 0) {
            Camera camera = Camera.main;
            for (int i = 0; i < _selectedObjects.Count; i++) {
                Debug.Log(i + " / " + _selectedObjects[i].ToString());
                _dsi.DrawIndicator(camera, selectables[_selectedObjects[i]].GetObjectBounds());
            }
        }
    }
}
