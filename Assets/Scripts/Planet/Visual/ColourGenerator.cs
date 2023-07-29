using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourGenerator
{
    ColourSettings colourSettings;
    Texture2D texture;
    const int textureResolution = 50;

    public void UpdateColourSettings(ColourSettings colourSettings)
    {
        this.colourSettings = colourSettings;

        if(colourSettings.planetMat == null)
            colourSettings.planetMat = new Material(colourSettings.shader);

        if (texture == null)
        {
            texture = new Texture2D(textureResolution, 1);
            texture.wrapMode = TextureWrapMode.Clamp;
        }
    }

    public void UpdateElevation(MinMax elevationMinMax)
    {
        colourSettings.planetMat.SetVector("_ElevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
        //Debug.Log("Current elevation: " + elevationMinMax.Min + " " + elevationMinMax.Max);
        //Debug.Log("Data being fed into shader: " + colourSettings.planetMat.GetVector("_ElevationMinMax"));
    }

    public void UpdateColours()
    {
        Color[] colours = new Color[textureResolution];
        for (int i = 0; i < textureResolution; i++) 
        {
            colours[i] = colourSettings.gradient.Evaluate(i / (textureResolution - 1f));
        }
        texture.SetPixels(colours);
        texture.Apply();
        colourSettings.planetMat.SetTexture("_MainTex", texture);
    }
}
