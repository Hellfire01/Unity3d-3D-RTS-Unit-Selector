using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraFarClipPlaneControl : MonoBehaviour {
    public LayerMask terrainOnly;
    public Camera mainCamera;
    public float distanceToGround = 0;
    
    private void Update() {
        float extend = 50f;
        RaycastHit hit;

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, Mathf.Infinity)) {
            Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * 1000f, Color.green);
            distanceToGround = Vector3.Distance(mainCamera.transform.position, hit.point);
            mainCamera.farClipPlane = distanceToGround * 4;
        } else {
            Debug.DrawRay(mainCamera.transform.position, Camera.main.transform.forward * 1000f, Color.red);
        }
    }
}
