using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CustomAtmosphereEffect : PostProcessingEffect
{
    public Shader atmosphereShader;

    List<AtmosphereEffectContainer> effectHolders;
    List<float> sortDistances;

    List<Material> postProcessingMaterials;
    bool active = true;

    public override void Render(RenderTexture source, RenderTexture destination)
    {
        List<Material> materials = GetMaterials();
        CustomPostProcessing.RenderMaterials(source, destination, materials);
    }

    void Init()
    {
        if (effectHolders == null || effectHolders.Count == 0 || !Application.isPlaying)
        {
            var planets = FindObjectsOfType<Planet>();
            effectHolders = new List<AtmosphereEffectContainer>(planets.Length);
            for (int i = 0; i < planets.Length; i++)
            {
                effectHolders.Add(new AtmosphereEffectContainer(planets[i]));
            }
        }

        if (postProcessingMaterials == null)
            postProcessingMaterials = new List<Material>();

        if (sortDistances == null)
            sortDistances = new List<float>();

        sortDistances.Clear();
        postProcessingMaterials.Clear();
    }

    public List<Material> GetMaterials()
    {
        if (!active)
            return null;

        Init();

        if (effectHolders.Count > 0)
        {
            Camera cam = Camera.current;
            Vector3 camPos = cam.transform.position;

            SortFarToNear(camPos);

            for (int i = 0; i < effectHolders.Count; i++)
            {
                AtmosphereEffectContainer effectHolder = effectHolders[i];

                if (effectHolder.atmosphereEffect != null)
                {
                    effectHolder.atmosphereEffect.UpdateAtmosphere(effectHolder.planet);
                    postProcessingMaterials.Add(effectHolder.atmosphereEffect.GetMat());
                }
            }
        }

        return postProcessingMaterials;
    }

    void SortFarToNear(Vector3 viewPos)
    {
        for (int i = 0; i < effectHolders.Count; i++)
        {
            float dstToSurface = effectHolders[i].DstFromSurface(viewPos);
            sortDistances.Add(dstToSurface);
        }

        for (int i = 0; i < effectHolders.Count - 1; i++)
        {
            for (int j = i + 1; j > 0; j--)
            {
                if (sortDistances[j - 1] < sortDistances[j])
                {
                    float tempDst = sortDistances[j - 1];
                    var temp = effectHolders[j - 1];
                    sortDistances[j - 1] = sortDistances[j];
                    sortDistances[j] = tempDst;
                    effectHolders[j - 1] = effectHolders[j];
                    effectHolders[j] = temp;
                }
            }
        }
    }

    public class AtmosphereEffectContainer
    {
        public Planet planet;
        public AtmosphereEffect atmosphereEffect;

        public AtmosphereEffectContainer(Planet planet)
        {
            this.planet = planet;
            if (planet.atmosphereSettings)
                atmosphereEffect = new AtmosphereEffect();
        }

        public float DstFromSurface(Vector3 viewPos)
        {
            return Mathf.Max(0, (planet.transform.position - viewPos).magnitude - planet.shapeSettings.planetRadius);
        }
    }
}
