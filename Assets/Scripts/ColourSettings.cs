using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu()]
public class ColourSettings : ScriptableObject
{
    public Shader shader;
    // OPTIONAL: Can be assigned manually to essentially have a material instance in the editor
    public bool exposeMat = false;
    [ConditionalHide("exposeMat", 1)]
    public Material planetMat;
    public ZoneColourSettings zoneColourSettings;
    public Gradient oceanColour;
    public Texture2D oceanMap;
    public Vector2 oceanMapTiling = new Vector2(1, 1);
    public float waveSpeed;
    public float oceanStrength;
    public float oceanSmoothness;

    [System.Serializable]
    public class ZoneColourSettings
    {
        public Zone[] zones;
        public NoiseSettings noiseSettings;
        public float noiseOffset;
        public float noiseScale;
        [Range(0.0f, 1.0f)]
        public float blend;

        [System.Serializable]
        public class Zone
        {
            public Gradient gradient;
            [Range(0, 1)]
            public float startPos;
        }
    }
}
