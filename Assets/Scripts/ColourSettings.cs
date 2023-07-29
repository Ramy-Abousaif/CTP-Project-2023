using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu()]
public class ColourSettings : ScriptableObject
{
    public Shader shader;
    [HideInInspector]
    public Material planetMat;
    public BiomeColourSettings biomeColourSettings;
    public Gradient oceanColour;

    [System.Serializable]
    public class BiomeColourSettings
    {
        public Biome[] biomes;
        public NoiseSettings noiseSettings;
        public float noiseOffset;
        public float noiseScale;
        [Range(0.0f, 1.0f)]
        public float blend;

        [System.Serializable]
        public class Biome
        {
            public Gradient gradient;
            public Color tint;
            [Range(0, 1)]
            public float startHeight;
            [Range(0, 1)]
            public float tintPercent;
        }
    }
}
