using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class PerformanceLogger : MonoBehaviour
{
    private float updateInterval = 1f;
    private float accumFPS = 0f;
    private int frames = 0;
    private float timeLeft;
    private List<float> fpsData = new List<float>();

    private void Start()
    {
        timeLeft = updateInterval;
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        accumFPS += 1f / Time.deltaTime; // Calculate frame rate as frames per second
        frames++;

        if (timeLeft <= 0.0)
        {
            float fps = accumFPS / frames;
            fpsData.Add(fps);

            timeLeft = updateInterval;
            accumFPS = 0f;
            frames = 0;
        }
    }

    private void OnApplicationQuit()
    {
        string path = Application.dataPath + "/PerformanceData.csv";

        using (StreamWriter writer = new StreamWriter(path, false))
        {
            writer.WriteLine("FPS,Memory Usage");

            foreach (float fps in fpsData)
            {
                float memoryUsage = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024); // Convert to MB

                writer.WriteLine($"{fps},{memoryUsage}");
            }
        }
    }
}