using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AtmosphereSettings : ScriptableObject
{
    public Shader shader;

    private bool isUpdated;

    [Range(0, 1)]
    public float scale = 0.5f;

    public float density = 0.25f;
    public int opticalDepthPoints = 10;
    public int inScatteringPoints = 10;

    void OnValidate()
    {
        isUpdated = false;
    }

    public void SetProperties(Material mat, float _planetRadius)
    {
        if (!isUpdated || !Application.isPlaying)
        {
            float atmosphereRadius = (1 + scale) * _planetRadius;

            mat.SetInt("numInScatteringPoints", inScatteringPoints);
            mat.SetInt("numOpticalDepthPoints", opticalDepthPoints);
            mat.SetFloat("atmosphereRadius", atmosphereRadius);
            mat.SetFloat("planetRadius", _planetRadius);
            mat.SetFloat("densityFalloff", density);


            isUpdated = true;
        }
    }
}
