using Palmmedia.ReportGenerator.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourGenerator
{
    ColourSettings colourSettings;
    Texture2D texture;
    const int textureResolution = 50;
    INoiseFilter biomeNoiseFilter;

    public void UpdateColourSettings(ColourSettings colourSettings)
    {
        this.colourSettings = colourSettings;

        if(colourSettings.planetMat == null)
            colourSettings.planetMat = new Material(colourSettings.shader);

        if (texture == null || texture.height != colourSettings.biomeColourSettings.biomes.Length)
        {
            texture = new Texture2D(textureResolution * 2, colourSettings.biomeColourSettings.biomes.Length, TextureFormat.RGBA32, false);
            texture.wrapMode = TextureWrapMode.Clamp;
        }

        biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(colourSettings.biomeColourSettings.noiseSettings);
    }

    public void UpdateElevation(MinMax elevationMinMax)
    {
        colourSettings.planetMat.SetVector("_ElevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
        //Debug.Log("Current elevation: " + elevationMinMax.Min + " " + elevationMinMax.Max);
        //Debug.Log("Data being fed into shader: " + colourSettings.planetMat.GetVector("_ElevationMinMax"));
    }

    public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
    {
        float heightPercent = (pointOnUnitSphere.y + 1) / 2f;
        heightPercent += (biomeNoiseFilter.Evaluate(pointOnUnitSphere) - colourSettings.biomeColourSettings.noiseOffset) * colourSettings.biomeColourSettings.noiseScale;
        float biomeIndex = 0;
        int numBiomes = colourSettings.biomeColourSettings.biomes.Length;
        float blendRange = colourSettings.biomeColourSettings.blend / 2f + 0.001f;

        for (int i = 0; i < numBiomes; i++)
        {
            float dst = heightPercent - colourSettings.biomeColourSettings.biomes[i].startHeight;
            float weight = Mathf.InverseLerp(-blendRange, blendRange, dst);
            biomeIndex *= (1 - weight);
            biomeIndex += i * weight;
        }

        return biomeIndex / Mathf.Max(1, numBiomes - 1);
    }

    public void UpdateColours()
    {
        Color[] colours = new Color[texture.width * texture.height];
        int index = 0;
        foreach (var biome in colourSettings.biomeColourSettings.biomes)
        {
            for (int i = 0; i < textureResolution * 2; i++)
            {
                Color gradientColour;
                if(i < textureResolution)
                {
                    gradientColour = colourSettings.oceanColour.Evaluate(i / (textureResolution - 1f));
                }
                else
                {
                    gradientColour = biome.gradient.Evaluate((i - textureResolution) / (textureResolution - 1f));
                }
                Color tintColour = biome.tint;
                colours[index] = gradientColour * (1 - biome.tintPercent) + tintColour * biome.tintPercent;
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
