using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySimManager : MonoBehaviour
{
    public static GravitySimManager instance;
    public GravitationalBody[] bodies;

    [HideInInspector]
    public float myTimeScale = 1.0f;
    private float timeScaleIncrement = 0.25f;
    private float minTimeScale = 0.0f;
    private float maxTimeScale = 2.0f;
    private float timeStep = 0.01f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        bodies = FindObjectsOfType<GravitationalBody>();
        Time.fixedDeltaTime = timeStep;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            myTimeScale = Mathf.Clamp(myTimeScale - timeScaleIncrement, minTimeScale, maxTimeScale);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            myTimeScale = Mathf.Clamp(myTimeScale + timeScaleIncrement, minTimeScale, maxTimeScale);
    }

    void FixedUpdate()
    {
        for (int i = 0; i < bodies.Length; i++)
        {
            Vector3 acceleration = GetAccelerationAtCurrentPosition(bodies[i].rb.position, bodies[i]);
            bodies[i].UpdateVel(acceleration, timeStep);
        }

        for (int i = 0; i < bodies.Length; i++)
        {
            bodies[i].UpdatePos(timeStep);
        }

    }

    public Vector3 GetAccelerationAtCurrentPosition(Vector3 currentPos, GravitationalBody excluded = null)
    {
        var accel = Vector3.zero;

        foreach (var body in bodies)
        {
            if (body != excluded)
            {
                var sqrDist = (body.rb.position - currentPos).sqrMagnitude;
                var dir = (body.rb.position - currentPos).normalized;
                accel += 0.0001f * dir * body.mass / sqrDist;
            }
        }

        return accel;
    }
}
