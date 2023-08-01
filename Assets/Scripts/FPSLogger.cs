using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class FPSLogger : MonoBehaviour
{
    private float updateInterval = 1f;
    private float accum = 0f;
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
        accum += Time.timeScale / Time.deltaTime;
        frames++;

        if (timeLeft <= 0.0)
        {
            float fps = accum / frames;
            fpsData.Add(fps);

            timeLeft = updateInterval;
            accum = 0f;
            frames = 0;
        }
    }

    // Export the FPS data to a CSV file
    private void OnApplicationQuit()
    {
        string path = Application.dataPath + "/FPSData.csv";

        using (StreamWriter writer = new StreamWriter(path, false))
        {
            foreach (float fps in fpsData)
            {
                writer.WriteLine(fps);
            }
        }
    }
}