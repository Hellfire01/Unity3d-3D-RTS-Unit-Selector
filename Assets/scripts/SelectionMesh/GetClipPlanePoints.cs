using System;
using UnityEngine;

public static class GetClipPlanePoints {
    public class ClipPlanePoints {
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
        float height = Mathf.Tan(halfFOV) * distance;
        float width = height * mainCamera.aspect;

        Vector3 pcfd = pos + cameraTransform.forward * distance;
        Vector3 crw = cameraTransform.right * width;
        Vector3 cth = cameraTransform.up * height;
        
        // lower right
        clipPlanePoints.LowerRight = pcfd;
        clipPlanePoints.LowerRight += crw;
        clipPlanePoints.LowerRight -= cth;
        // lower left
        clipPlanePoints.LowerLeft = pcfd;
        clipPlanePoints.LowerLeft -= crw;
        clipPlanePoints.LowerLeft -= cth;
        // upper right
        clipPlanePoints.UpperRight = pcfd;
        clipPlanePoints.UpperRight += crw;
        clipPlanePoints.UpperRight += cth;
        // upper left
        clipPlanePoints.UpperLeft = pcfd;
        clipPlanePoints.UpperLeft -= crw;
        clipPlanePoints.UpperLeft += cth;

        return clipPlanePoints;
    }
}
