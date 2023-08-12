using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalBody : MonoBehaviour
{
    public CelestialType type;

    public float radius;
    public float surfaceGravity;
    public Vector3 initialVelocity;
    public float initialAngularMomentum;

    public Vector3 velocity { get; private set; }
    public float mass { get; private set; }
    [HideInInspector]
    public Rigidbody rb;

    public Vector3 northPole = Vector3.up;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = mass;

        if(transform.GetComponent<Planet>() != null)
            radius = transform.GetComponent<Planet>().shapeSettings.planetRadius;

        velocity = initialVelocity;
    }

    float currentRotationAngle;

    private void Update()
    {
        if (type == CelestialType.STAR)
            return;

        currentRotationAngle += CalculateRotationalVelocity() * Time.deltaTime * GravitySimManager.instance.myTimeScale;

        // Apply the rotation using the northPole vector as the axis and currentRotationAngle in radians
        transform.rotation = Quaternion.AngleAxis(currentRotationAngle, northPole);
    }

    public void UpdatePos(float _timeStep)
    {
        rb.MovePosition(rb.position + velocity * _timeStep * GravitySimManager.instance.myTimeScale);
    }

    public void UpdateVel(Vector3 _accel, float _timeStep)
    {
        velocity += _timeStep * GravitySimManager.instance.myTimeScale * _accel;
    }


    // Calculate the rotational period of the planet in seconds
    public float GetRotationalPeriod()
    {
        return CalculateRotationalVelocity() / 360f;
    }

    private float CalculateRotationalVelocity()
    {
        // Calculate the moment of inertia of the planet
        float momentOfInertia = 0.4f * radius * radius; // Assuming a solid sphere with uniform density (simplified)

        // Calculate the rotational velocity vector in radians per second
        float rotationalVelocity = initialAngularMomentum / momentOfInertia;

        //Debug.Log("Rotational Velocity: " + rotationalVelocity + " radians per second");

        return rotationalVelocity;
    }

    void OnValidate()
    {
        mass = surfaceGravity * radius * radius / 0.0001f;
    }
}

class GravitationalBodyContainer
{
    public float mass;
    public Vector3 pos;
    public Vector3 velocity;

    public GravitationalBodyContainer(GravitationalBody body)
    {
        mass = body.mass;
        pos = body.transform.position;
        velocity = body.initialVelocity;
    }
}