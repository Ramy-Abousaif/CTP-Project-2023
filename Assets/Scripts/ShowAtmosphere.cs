using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAtmosphere : MonoBehaviour
{
    public Material atmospherePostProcessMat;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, atmospherePostProcessMat);
    }
}
