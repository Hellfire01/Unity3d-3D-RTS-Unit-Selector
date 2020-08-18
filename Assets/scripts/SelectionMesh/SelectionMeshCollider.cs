using UnityEngine;

public class SelectionMeshCollider : MonoBehaviour {
    public SelectionManager selectionManager;

    // WARNING : the collision matrix MUST be set in order to get the selection layer to only collide with your selection
    //    layer. This ensures that anything colliding with the collider mesh is selectable by the user and has a
    //    Selectable component attached
    // This is important in order to avoid light performance issues on big scenes with lots of colliders
    
    private void OnTriggerEnter(Collider other) {
        selectionManager.addAsSelectedByUserByMeshCollider(other.gameObject.GetComponent<Selectable>());
    }

    private void OnTriggerStay(Collider other) {
        selectionManager.addAsSelectedByUserByMeshCollider(other.gameObject.GetComponent<Selectable>());
    }
}
