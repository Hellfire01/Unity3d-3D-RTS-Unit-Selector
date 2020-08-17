using System;
using UnityEngine;

public class SelectionMeshCollider : MonoBehaviour {
    public SelectionManager selectionManager;
    
    void Start() {
        
    }

    private void Update() {
        
    }

    private void OnTriggerEnter(Collider other) {
        selectionManager.addAsSelectedByUserByMeshCollider(other.gameObject.GetComponent<Selectable>());
    }

    private void OnTriggerStay(Collider other) {
        selectionManager.addAsSelectedByUserByMeshCollider(other.gameObject.GetComponent<Selectable>());
    }
}
