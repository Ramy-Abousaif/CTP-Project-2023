using Palmmedia.ReportGenerator.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourGenerator
{
    ColourSettings colourSettings;
    Texture2D texture;
    const int textureResolution = 50;
    INoiseFilter zoneNoiseFilter;

    public void UpdateColourSettings(ColourSettings colourSettings)
    {
        this.colourSettings = colourSettings;

        if(colourSettings.planetMat == null)
            colourSettings.planetMat = new Material(colourSettings.shader);

        if (texture == null || texture.height != colourSettings.zoneColourSettings.zones.Length)
        {
            texture = new Texture2D(textureResolution * 2, colourSettings.zoneColourSettings.zones.Length, TextureFormat.RGBA32, false);
            texture.wrapMode = TextureWrapMode.Clamp;
        }

        zoneNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(colourSettings.zoneColourSettings.noiseSettings);
    }

    public void UpdateElevation(MinMax elevationMinMax)
    {
        colourSettings.planetMat.SetVector("_ElevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
        //Debug.Log("Current elevation: " + elevationMinMax.Min + " " + elevationMinMax.Max);
        //Debug.Log("Data being fed into shader: " + colourSettings.planetMat.GetVector("_ElevationMinMax"));
    }

    public float LatitudinalZoneProgressFromPoint(Vector3 pointOnUnitSphere)
    {
        float blendRange = colourSettings.zoneColourSettings.blend / 2f + 0.001f;
        float heightProgress = (pointOnUnitSphere.y + 1) / 2f;
        heightProgress += (zoneNoiseFilter.Evaluate(pointOnUnitSphere) - colourSettings.zoneColourSettings.noiseOffset) * colourSettings.zoneColourSettings.noiseScale;
        float zoneIndex = 0;
        int numZones = colourSettings.zoneColourSettings.zones.Length;

        for (int i = 0; i < numZones; i++)
        {
            float distance = heightProgress - colourSettings.zoneColourSettings.zones[i].startPos;
            float weight = Mathf.InverseLerp(-blendRange, blendRange, distance);
            zoneIndex *= (1 - weight);
            zoneIndex += i * weight;
        }

        return zoneIndex / Mathf.Max(1, numZones - 1);
    }

    public void UpdateColours()
    {
        Color[] colours = new Color[texture.width * texture.height];
        int index = 0;
        foreach (var zone in colourSettings.zoneColourSettings.zones)
        {
            for (int i = 0; i < textureResolution * 2; i++)
            {
                Color gradientColour;
                if(i < textureResolution)
                    gradientColour = colourSettings.oceanColour.Evaluate(i / (textureResolution - 1f));
                else
                    gradientColour = zone.gradient.Evaluate((i - textureResolution) / (textureResolution - 1f));

                colours[index] = gradientColour;
                index++;
            }
        }
        texture.SetPixels(colours);
        texture.Apply();

        colourSettings.planetMat.SetTexture("_MainTex", texture);

        if (colourSettings.oceanMap != null)
            colourSettings.planetMat.SetTexture("_OceanNormalMap", colourSettings.oceanMap);
        else
            colourSettings.planetMat.SetTexture("_OceanNormalMap", new Texture2D(512, 512, TextureFormat.RGBA32, true));

        colourSettings.planetMat.SetFloat("_OceanSmoothness", colourSettings.oceanSmoothness);
        colourSettings.planetMat.SetFloat("_OceanNormalStrength", colourSettings.oceanStrength);
        colourSettings.planetMat.SetFloat("_OceanScrollSpeed", colourSettings.waveSpeed);
        colourSettings.planetMat.SetTextureScale("_OceanNormalMap", colourSettings.oceanMapTiling);
    }
}
