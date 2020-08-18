using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DrawSelectionIndicator : MonoBehaviour {
    public Texture topLeftBorder;
    public Texture bottomLeftBorder;
    public Texture topRightBorder;
    public Texture bottomRightBorder;
    public int textureOffset;
    public int textureSize;
    Texture2D _borderTexture;

    private void Start() {
        _borderTexture = new Texture2D(1, 1);
        _borderTexture.SetPixel(0, 0, Color.white);
        _borderTexture.Apply();
    }

    // draws the player's selection on screen
    public void DrawScreenRectBorder(Rect rect, float thickness, Color color) {
        GUI.color = color;
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, thickness), _borderTexture);
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, thickness, rect.height), _borderTexture);
        GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), _borderTexture);
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), _borderTexture);
        GUI.color = Color.white;
    }

    // gets the player's selection rectangle
    // the rectangle is defined by the position at witch the user did a clic and the current position of the mouse
    public Rect GetScreenSelectionRectangle(Vector3 screenPosition1, Vector3 screenPosition2) {
        // Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        // Calculate corners
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        // Create Rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    // displays the box around a selected GameObject given the 4 textures used in the corners
    // the selection will be drawn correctly no matter the orientation of the unit
    public void DrawIndicator(Camera camera, Bounds bounds) {
        Vector3 boundPoint1 = bounds.min;
        Vector3 boundPoint2 = bounds.max;
        Vector3 boundPoint3 = new Vector3(boundPoint1.x, boundPoint1.y, boundPoint2.z);
        Vector3 boundPoint4 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint1.z);
        Vector3 boundPoint5 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint1.z);
        Vector3 boundPoint6 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint2.z);
        Vector3 boundPoint7 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint2.z);
        Vector3 boundPoint8 = new Vector3(boundPoint2.x, boundPoint2.y, boundPoint1.z);
        Vector2[] screenPoints = new Vector2[8];
        screenPoints[0] = camera.WorldToScreenPoint(boundPoint1);
        screenPoints[1] = camera.WorldToScreenPoint(boundPoint2);
        screenPoints[2] = camera.WorldToScreenPoint(boundPoint3);
        screenPoints[3] = camera.WorldToScreenPoint(boundPoint4);
        screenPoints[4] = camera.WorldToScreenPoint(boundPoint5);
        screenPoints[5] = camera.WorldToScreenPoint(boundPoint6);
        screenPoints[6] = camera.WorldToScreenPoint(boundPoint7);
        screenPoints[7] = camera.WorldToScreenPoint(boundPoint8);
        Vector2 topLeftPosition = Vector2.zero;
        Vector2 topRightPosition = Vector2.zero;
        Vector2 bottomLeftPosition = Vector2.zero;
        Vector2 bottomRightPosition = Vector2.zero;

        for (int a = 0; a < screenPoints.Length; a++) {
            // Top left texture
            if (topLeftPosition.x == 0 || topLeftPosition.x > screenPoints[a].x) {
                topLeftPosition.x = screenPoints[a].x;
            }
            if (topLeftPosition.y == 0 || topLeftPosition.y > Screen.height - screenPoints[a].y) {
                topLeftPosition.y = Screen.height - screenPoints[a].y;
            }
            // Top right texture
            if (topRightPosition.x == 0 || topRightPosition.x < screenPoints[a].x) {
                topRightPosition.x = screenPoints[a].x;
            }
            if (topRightPosition.y == 0 || topRightPosition.y > Screen.height - screenPoints[a].y) {
                topRightPosition.y = Screen.height - screenPoints[a].y;
            }
            // Bottom left texture
            if (bottomLeftPosition.x == 0 || bottomLeftPosition.x > screenPoints[a].x) {
                bottomLeftPosition.x = screenPoints[a].x;
            }
            if (bottomLeftPosition.y == 0 || bottomLeftPosition.y < Screen.height - screenPoints[a].y) {
                bottomLeftPosition.y = Screen.height - screenPoints[a].y;
            }
            // Bottom right texture
            if (bottomRightPosition.x == 0 || bottomRightPosition.x < screenPoints[a].x) {
                bottomRightPosition.x = screenPoints[a].x;
            }
            if (bottomRightPosition.y == 0 || bottomRightPosition.y < Screen.height - screenPoints[a].y) {
                bottomRightPosition.y = Screen.height - screenPoints[a].y;
            }
        }
        GUI.DrawTexture(new Rect(topLeftPosition.x - textureOffset - textureSize, topLeftPosition.y - textureOffset - textureSize, textureSize, textureSize), topLeftBorder);
        GUI.DrawTexture(new Rect(topRightPosition.x + textureOffset, topRightPosition.y - textureOffset - textureSize, textureSize, textureSize), topRightBorder);
        GUI.DrawTexture(new Rect(bottomLeftPosition.x - textureOffset - textureSize, bottomLeftPosition.y + textureOffset, textureSize, textureSize), bottomLeftBorder);
        GUI.DrawTexture(new Rect(bottomRightPosition.x + textureOffset, bottomRightPosition.y + textureOffset, textureSize, textureSize), bottomRightBorder);
    }    
}
