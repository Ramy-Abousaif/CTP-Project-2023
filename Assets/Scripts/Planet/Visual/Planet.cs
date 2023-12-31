using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CelestialType
{
    PLANET,
    SATELLITE,
    STAR
};

public class Planet : MonoBehaviour
{
    [Range(2, 256)]
    public int resolution = 10;
    public bool autoUpdate = true;

    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;
    public AtmosphereSettings atmosphereSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;

    [HideInInspector]
    public bool colourSettingsFoldout;

    [HideInInspector]
    public bool atmosphereSettingsFoldout;

    ShapeGenerator shapeGenerator = new ShapeGenerator();
    ColourGenerator colourGenerator = new ColourGenerator();

    private const int faceCount = 6;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    FaceMesh[] faces;

    // Method gets called on initialization and creates the shape generator and the mesh filters
    private void Setup()
    {
        shapeGenerator.UpdateShapeSettings(shapeSettings);
        colourGenerator.UpdateColourSettings(colourSettings);

        if(meshFilters == null || meshFilters.Length == 0)
            meshFilters = new MeshFilter[faceCount];

        this.faces = new FaceMesh[faceCount];

        Vector3[] directions = {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

        for (int i = 0; i < faceCount; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("face");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].mesh = new Mesh();
            }
            this.meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.planetMat;

            this.faces[i] = new FaceMesh(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }

    public void Awake()
    {
        GeneratePlanet();
    }

    public void GeneratePlanet()
    {
        if(shapeSettings == null || colourSettings == null || atmosphereSettings == null)
        {
            Debug.LogWarning("Planet settings not set correctly, missing a setting");
            return;
        }

        Setup();
        GenerateMesh();
        GenerateColours();
        GenerateAtmosphere();
    }

    private void GenerateMesh()
    {
        foreach (var face in faces)
        {
            face.ConstructMesh();
        }

        colourGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    public void OnShapeSettingsChange()
    {
        if(autoUpdate)
        {
            Setup();
            GenerateMesh();
        }
    }

    public void OnColourSettingsChange()
    {
        if (autoUpdate)
        {
            Setup();
            GenerateColours();
        }
    }

    public void OnAtmosphereSettingsChange()
    {
        if (autoUpdate)
        {
            Setup();
            GenerateAtmosphere();
        }
    }

    private void GenerateColours()
    {
        colourGenerator.UpdateColours();
        foreach (var face in faces)
        {
            face.UpdateUVs(colourGenerator);
        }
    }

    private void GenerateAtmosphere()
    {
        
    }
}
