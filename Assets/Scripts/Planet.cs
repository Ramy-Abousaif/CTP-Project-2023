using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Range(2, 256)]
    public int resolution = 10;
    public bool autoUpdate = true;

    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;

    [HideInInspector]
    public bool colourSettingsFoldout;

    ShapeGenerator shapeGenerator;

    private const int faceCount = 6;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    FaceMesh[] faces;

    private void Initialize()
    {
        shapeGenerator = new ShapeGenerator(shapeSettings);

        if(meshFilters == null || meshFilters.Length == 0)
            meshFilters = new MeshFilter[faceCount];

        faces = new FaceMesh[faceCount];

        Vector3[] directions = {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

        for (int i = 0; i < faceCount; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("face");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            faces[i] = new FaceMesh(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColours();
    }

    private void GenerateMesh()
    {
        foreach (var face in faces)
        {
            face.ConstructMesh();
        }
    }

    public void OnShapeSettingsChange()
    {
        if(autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColourSettingsChange()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColours();
        }
    }

    private void GenerateColours()
    {
        foreach (var m in meshFilters)
        {
            m.GetComponent<MeshRenderer>().sharedMaterial.color = colourSettings.planetColour;
        }
    }
}
