using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class OrbitLines : MonoBehaviour
{

    public int stepCount = 1000;
    public float timeStep = 0.1f;
    public bool isRelativeToABody;

    [ConditionalHide("relativeToBody", 1)]
    public GravitationalBody gravitationalBody;

    void Update()
    {
        if (!Application.isPlaying)
            DrawTrajectories();
    }

    Vector3 GetAccelerationAtStep(GravitationalBodyContainer[] containers, int stepIndex)
    {
        var accel = Vector3.zero;

        for (int i = 0; i < containers.Length; i++)
        {
            if (stepIndex == i)
                continue;

            var sqrDist = (containers[i].pos - containers[stepIndex].pos).sqrMagnitude;
            var dir = (containers[i].pos - containers[stepIndex].pos).normalized;
            accel += 0.0001f * dir * containers[i].mass / sqrDist;
        }
        return accel;
    }

    void DrawTrajectories()
    {
        GravitationalBody[] bodies = FindObjectsOfType<GravitationalBody>();

        Vector3 relBodyInitialPos = Vector3.zero;
        int relBodyIndex = 0;
        GravitationalBodyContainer[] containers = new GravitationalBodyContainer[bodies.Length];
        Vector3[][] linePositions = new Vector3[bodies.Length][];

        for (int i = 0; i < containers.Length; i++)
        {
            containers[i] = new GravitationalBodyContainer(bodies[i]);
            linePositions[i] = new Vector3[stepCount];

            if (bodies[i] == gravitationalBody && isRelativeToABody)
            {
                relBodyIndex = i;
                relBodyInitialPos = containers[i].pos;
            }
        }

        for (int step = 0; step < stepCount; step++)
        {
            Vector3 relBodyPos = (isRelativeToABody) ? containers[relBodyIndex].pos : Vector3.zero;

            for (int i = 0; i < containers.Length; i++)
            {
                containers[i].velocity += GetAccelerationAtStep(containers, i) * timeStep;
            }

            for (int i = 0; i < containers.Length; i++)
            {
                Vector3 newPos = containers[i].pos + containers[i].velocity * timeStep;
                containers[i].pos = newPos;
                if (isRelativeToABody)
                {
                    var referenceFrameOffset = relBodyPos - relBodyInitialPos;
                    newPos -= referenceFrameOffset;
                }

                if (i == relBodyIndex && isRelativeToABody)
                    newPos = relBodyInitialPos;

                linePositions[i][step] = newPos;
            }
        }

        for (int bodyIndex = 0; bodyIndex < containers.Length; bodyIndex++)
        {
            Color lineColour = Color.white;

            if(bodies[bodyIndex].gameObject.GetComponent<Planet>() != null)
                lineColour = bodies[bodyIndex].gameObject.GetComponent<Planet>().colourSettings.oceanColour.Evaluate(0);

            for (int i = 0; i < linePositions[bodyIndex].Length - 1; i++)
            {
                Debug.DrawLine(linePositions[bodyIndex][i], linePositions[bodyIndex][i + 1], lineColour);
            }

        }
    }
}