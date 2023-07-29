using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySimManager : MonoBehaviour
{
    public GravitationalBody[] bodies;
    public static GravitySimManager instance;

    void Awake()
    {
        bodies = FindObjectsOfType<GravitationalBody>();
        Time.fixedDeltaTime = 0.01f;
        //Debug.Log("Setting fixedDeltaTime to: " + 0.01f);
    }

    void FixedUpdate()
    {
        for (int i = 0; i < bodies.Length; i++)
        {
            Vector3 acceleration = CalculateAcceleration(bodies[i].Position, bodies[i]);
            bodies[i].UpdateVelocity(acceleration, 0.01f);
            //bodies[i].UpdateVelocity (bodies, Universe.physicsTimeStep);
        }

        for (int i = 0; i < bodies.Length; i++)
        {
            bodies[i].UpdatePosition(0.01f);
        }

    }

    public static Vector3 CalculateAcceleration(Vector3 point, GravitationalBody ignoreBody = null)
    {
        Vector3 acceleration = Vector3.zero;
        foreach (var body in Instance.bodies)
        {
            if (body != ignoreBody)
            {
                float sqrDst = (body.Position - point).sqrMagnitude;
                Vector3 forceDir = (body.Position - point).normalized;
                acceleration += forceDir * 0.0001f * body.mass / sqrDst;
            }
        }

        return acceleration;
    }

    public static GravitationalBody[] Bodies
    {
        get
        {
            return Instance.bodies;
        }
    }

    static GravitySimManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GravitySimManager>();
            }
            return instance;
        }
    }
}
