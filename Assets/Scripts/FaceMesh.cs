using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceMesh
{
    ShapeGenerator shapeGenerator;
    Mesh mesh;
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;

    public FaceMesh(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                int index = i + j * resolution;
                Vector2 percent = new Vector2(i, j) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[index] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);

                if(i != resolution - 1 && j != resolution - 1)
                {
                    triangles[triIndex] = index;
                    triangles[triIndex + 1] = index + resolution + 1;
                    triangles[triIndex + 2] = index + resolution;
                    triangles[triIndex + 3] = index;
                    triangles[triIndex + 4] = index + 1;
                    triangles[triIndex + 5] = index + resolution + 1;
                    triIndex += 6;
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}