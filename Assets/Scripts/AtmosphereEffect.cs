using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereEffect
{
    protected Material mat;
    private Light light;

    public void UpdateAtmosphere(Planet planet)
    {
        if (planet.atmosphereSettings.shader == null)
            return;

        Shader shader = planet.atmosphereSettings.shader;

        if (light == null)
            light = GameObject.FindObjectOfType<Sun>().GetComponentInChildren<Light>();

        if (mat == null || mat.shader != shader)
            mat = new Material(shader);

        planet.atmosphereSettings.SetProperties(mat, planet.shapeSettings.planetRadius);
        mat.SetVector("centre", planet.transform.position);

        if (light)
        {
            Vector3 sunDir = (light.transform.position - planet.transform.position).normalized;
            mat.SetVector("sunDir", sunDir);
        }
    }

    public Material GetMat()
    {
        return mat;
    }
}