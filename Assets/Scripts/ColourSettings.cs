using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ColourSettings : ScriptableObject
{
    public Gradient gradient;
    public Shader shader;
    [HideInInspector]
    public Material planetMat;
}
