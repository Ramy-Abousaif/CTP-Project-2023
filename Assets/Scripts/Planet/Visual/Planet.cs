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
    private void Initialize()
    {
        shapeGenerator.UpdateShapeSettings(shapeSettings);
        colourGenerator.UpdateColourSettings(colourSettings);

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

                meshObj.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].mesh = new Mesh();
            }
            meshFilters[i].GetComponent<MeshRenderer>().material = colourSettings.planetMat;

            faces[i] = new FaceMesh(shapeGenerator, meshFilters[i].mesh, resolution, directions[i]);
        }
    }

    public void Awake()
    {
        GeneratePlanet();
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

        colourGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
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

    public void OnAtmosphereSettingsChange()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateAtmosphere();
        }
    }

    private void GenerateColours()
    {
        colourGenerator.UpdateColours();
    }

    private void GenerateAtmosphere()
    {

    }
}
