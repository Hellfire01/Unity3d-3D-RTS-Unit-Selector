using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour {
    public Renderer[] renderers; // Assign all child Mesh Renderers
    public GameObject parent;
    
    public Bounds GetObjectBounds() {
        Bounds totalBounds = new Bounds();

        for (int i = 0; i < renderers.Length; i++) {
            if (totalBounds.center == Vector3.zero) {
                totalBounds = renderers[i].bounds;
            } else {
                totalBounds.Encapsulate(renderers[i].bounds);
            }
        }
        return totalBounds;
    }

    void OnEnable() {
        // Add this object to the selectable object list
        if (!SelectionManager.selectables.Contains(this)) {
            SelectionManager.selectables.Add(this);
        }
    }

    void OnDisable() {
        // Remove this object from the selectable object list
        if (SelectionManager.selectables.Contains(this)) {
            SelectionManager.selectables.Remove(this);
        }
    }
}
