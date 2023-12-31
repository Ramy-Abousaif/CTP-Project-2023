using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    ShapeSettings shapeSettings;
    INoiseFilter[] noiseFilters;
    public MinMax elevationMinMax;

    public void UpdateShapeSettings(ShapeSettings shapeSettings)
    {
        this.shapeSettings = shapeSettings;
        noiseFilters = new INoiseFilter[shapeSettings.noiseLayers.Length];
        for (int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(shapeSettings.noiseLayers[i].noiseSettings);
        }
        elevationMinMax = new MinMax();
    }

    public float CalculateUnscaledElevation(Vector3 pointOnUnitSphere)
    {
        float output = 0;
        float firstLayerVal = 0;

        if(noiseFilters.Length > 0) 
        {
            firstLayerVal = noiseFilters[0].Evaluate(pointOnUnitSphere);

            if (shapeSettings.noiseLayers[0].enabled)
                output = firstLayerVal;
        }

        for (int i = 1; i < noiseFilters.Length; i++)
        {
            if (shapeSettings.noiseLayers[i].enabled)
            {
                float mask = (shapeSettings.noiseLayers[i].useFirstLayerAsMask) ? firstLayerVal : 1;
                output += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
            }
        }
        elevationMinMax.AddValue(output);
        return  output;
    }

    public float GetScaledElevation(float unscaledElevation)
    {
        float elevation = Mathf.Max(0, unscaledElevation);
        elevation = shapeSettings.planetRadius * (1 + elevation);
        return elevation;
    }
}
