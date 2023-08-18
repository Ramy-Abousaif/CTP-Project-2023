static const float PI = 3.14159265359;
static const float TAU = PI * 2;
static const float maxFloat = 3.402823466e+38;

float invLerp(float from, float to, float value)
{
    return (value - from) / (to - from);
}

float2 raySphere(float3 centre, float radius, float3 rayOrigin, float3 rayDir)
{
    float3 offset = rayOrigin - centre;
    float a = 1;
    float b = 2 * dot(offset, rayDir);
    float c = dot(offset, offset) - radius * radius;
    float discriminant = b * b - 4 * a * c;

    if (discriminant > 0)
    {
        float s = sqrt(discriminant);
        float distToSphereFar = (-b + s) / (2 * a);
        float distToSphereNear = max(0, (-b - s) / (2 * a));

        if (distToSphereFar >= 0)
            return float2(distToSphereNear, distToSphereFar - distToSphereNear);
    }
    
    return float2(maxFloat, 0);
}