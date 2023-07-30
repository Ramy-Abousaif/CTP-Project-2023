using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu()]
public class ColourSettings : ScriptableObject
{
    public Shader shader;
    // OPTIONAL: Can be assigned manually to essentially have a material instance in the editor
    public Material planetMat;
    public BiomeColourSettings biomeColourSettings;
    public Gradient oceanColour;
    public Texture2D oceanMap;
    public Vector2 oceanMapTiling;
    public float waveSpeed;
    public float oceanStrength;
    public float oceanSmoothness;

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
