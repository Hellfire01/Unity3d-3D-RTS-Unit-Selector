using System;
using UnityEngine;

public static class GetClipPlanePoints {
    public struct ClipPlanePoints {
        public Vector3 UpperLeft;
        public Vector3 UpperRight;
        public Vector3 LowerLeft;
        public Vector3 LowerRight;

        public String getString() {
            return ("(" + UpperLeft.x + "," + UpperLeft.y + "), " +
                    "(" + UpperRight.x + "," + UpperRight.y + "), " +
                    "(" + LowerLeft.x + "," + LowerLeft.y + "), " +
                    "(" + LowerRight.x + "," + LowerRight.y + ")");
        }
    }

    public static ClipPlanePoints getClipPlanePoints(Camera mainCamera, float distance) {
        ClipPlanePoints clipPlanePoints = new ClipPlanePoints();
        Transform cameraTransform = mainCamera.transform;
        Vector3 pos = cameraTransform.position;
        float halfFOV = (mainCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
        float aspect = mainCamera.aspect;
        float height = Mathf.Tan(halfFOV) * distance;
        float width = height * aspect;

        // lower right
        clipPlanePoints.LowerRight = pos + cameraTransform.forward * distance;
        clipPlanePoints.LowerRight += cameraTransform.right * width;
        clipPlanePoints.LowerRight -= cameraTransform.up * height;
        // lower left
        clipPlanePoints.LowerLeft = pos + cameraTransform.forward * distance;
        clipPlanePoints.LowerLeft -= cameraTransform.right * width;
        clipPlanePoints.LowerLeft -= cameraTransform.up * height;
        // upper right
        clipPlanePoints.UpperRight = pos + cameraTransform.forward * distance;
        clipPlanePoints.UpperRight += cameraTransform.right * width;
        clipPlanePoints.UpperRight += cameraTransform.up * height;
        // upper left
        clipPlanePoints.UpperLeft = pos + cameraTransform.forward * distance;
        clipPlanePoints.UpperLeft -= cameraTransform.right * width;
        clipPlanePoints.UpperLeft += cameraTransform.up * height;
        
        return clipPlanePoints;
    }
}
