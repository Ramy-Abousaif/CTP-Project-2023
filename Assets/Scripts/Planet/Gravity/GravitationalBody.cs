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
    public string bodyName = "Unnamed";
    Transform meshHolder;

    public Vector3 velocity { get; private set; }
    public float mass { get; private set; }
    Rigidbody rb;

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

        currentRotationAngle += CalculateRotationalPeriod() * Time.deltaTime;

        // Apply the rotation using the northPole vector as the axis and currentRotationAngle in radians
        transform.rotation = Quaternion.AngleAxis(currentRotationAngle, northPole);
    }

    public void UpdateVelocity(GravitationalBody[] allBodies, float timeStep)
    {
        foreach (var otherBody in allBodies)
        {
            if (otherBody != this)
            {
                float sqrDst = (otherBody.rb.position - rb.position).sqrMagnitude;
                Vector3 forceDir = (otherBody.rb.position - rb.position).normalized;

                Vector3 acceleration = forceDir * 0.0001f * otherBody.mass / sqrDst;
                velocity += acceleration * timeStep;
            }
        }
    }

    public void UpdateVelocity(Vector3 acceleration, float timeStep)
    {
        velocity += acceleration * timeStep;
    }

    public void UpdatePosition(float timeStep)
    {
        rb.MovePosition(rb.position + velocity * timeStep);
    }

    private float CalculateRotationalPeriod()
    {
        return 360 / CalculateRotationalVelocity();
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
        gameObject.name = bodyName;
    }

    public Rigidbody Rigidbody
    {
        get
        {
            return rb;
        }
    }

    public Vector3 Position
    {
        get
        {
            return rb.position;
        }
    }
}
