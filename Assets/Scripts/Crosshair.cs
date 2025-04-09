using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public float crosshairSize = 5f;
    public float lineThickness = 2f;
    public Color crosshairColor = Color.green;

    void OnGUI()
    {
        float centerX = Screen.width / 2;
        float centerY = Screen.height / 2;

        GUI.color = crosshairColor;

        GUI.DrawTexture(new Rect(centerX - crosshairSize, centerY - lineThickness / 2, crosshairSize * 2, lineThickness), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(centerX - lineThickness / 2, centerY - crosshairSize, lineThickness, crosshairSize * 2), Texture2D.whiteTexture);   
    }
}
