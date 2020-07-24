using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MoveUnit : MonoBehaviour {
    private NavMeshAgent _agent;
    private RaycastHit _hitInfo = new RaycastHit();
    
    // Start is called before the first frame update
    void Start() {
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update() {
        // if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift)) {
        //     var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     if (Physics.Raycast(ray.origin, ray.direction, out _hitInfo)) {
        //         _agent.destination = _hitInfo.point;
        //     }
        // }
    }

    public void moveToPos(Vector3 position) {
        _agent.destination = position;
    }
}
