using UnityEngine;

public class SelectionMeshCollider : MonoBehaviour {
    public SelectionManager selectionManager;

    private void OnTriggerEnter(Collider other) {
        selectionManager.addAsSelectedByUserByMeshCollider(other.gameObject.GetComponent<Selectable>());
    }

    private void OnTriggerStay(Collider other) {
        selectionManager.addAsSelectedByUserByMeshCollider(other.gameObject.GetComponent<Selectable>());
    }
}
