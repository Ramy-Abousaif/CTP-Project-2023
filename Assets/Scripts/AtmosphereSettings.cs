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
    public Vector3 wavelengths = new Vector3(680, 550, 440);
    public float scatterStrength = 1f;

    void OnValidate()
    {
        isUpdated = false;
    }

    public void SetProperties(Material mat, float _planetRadius)
    {
        if (!isUpdated || !Application.isPlaying)
        {
            float atmosphereRadius = (1 + scale) * _planetRadius;

            float scatterR = Mathf.Pow(400 / wavelengths.x, 4) * scatterStrength;
            float scatterG = Mathf.Pow(400 / wavelengths.y, 4) * scatterStrength;
            float scatterB = Mathf.Pow(400 / wavelengths.z, 4) * scatterStrength;

            mat.SetInt("numInScatteringPoints", inScatteringPoints);
            mat.SetInt("numOpticalDepthPoints", opticalDepthPoints);
            mat.SetFloat("atmosphereRadius", atmosphereRadius);
            mat.SetVector("scatteringCoefficients", new Vector3(scatterR, scatterG, scatterB));
            mat.SetFloat("planetRadius", _planetRadius);
            mat.SetFloat("densityFalloff", density);


            isUpdated = true;
        }
    }
}
