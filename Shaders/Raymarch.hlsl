// Keep your Hash and Noise3D functions as they are
float Hash(float3 p)
{
    p = frac(p * 0.3183099 + float3(0.71, 0.113, 0.419));
    p *= 17.0;
    return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
}

float Noise3D(float3 p)
{
    float3 i = floor(p);
    float3 f = frac(p);
    float3 u = f * f * (3.0 - 2.0 * f); // Smoothstep

    float n000 = Hash(i + float3(0, 0, 0));
    float n100 = Hash(i + float3(1, 0, 0));
    float n010 = Hash(i + float3(0, 1, 0));
    float n110 = Hash(i + float3(1, 1, 0));
    float n001 = Hash(i + float3(0, 0, 1));
    float n101 = Hash(i + float3(1, 0, 1));
    float n011 = Hash(i + float3(0, 1, 1));
    float n111 = Hash(i + float3(1, 1, 1));

    float nx00 = lerp(n000, n100, u.x);
    float nx10 = lerp(n010, n110, u.x);
    float nx01 = lerp(n001, n101, u.x);
    float nx11 = lerp(n011, n111, u.x);

    float nxy0 = lerp(nx00, nx10, u.y);
    float nxy1 = lerp(nx01, nx11, u.y);

    return lerp(nxy0, nxy1, u.z);
}

// Helper function for Ray-Sphere intersection
// Returns true if intersection occurs, outputs distances t0 (near) and t1 (far)
// RayOrigin: Starting point of the ray (Camera Position)
// RayDirection: Normalized direction of the ray
// Sphere: xyz = center, w = radius
bool IntersectSphere(float3 RayOrigin, float3 RayDirection, float4 Sphere, out float t0, out float t1)
{
    t0 = -1.0; // Use negative values to indicate no intersection initially
    t1 = -1.0;
    float3 oc = RayOrigin - Sphere.xyz; // Vector from sphere center to ray origin
    float b = dot(oc, RayDirection);
    float c = dot(oc, oc) - Sphere.w * Sphere.w;
    float discriminant = b * b - c;

    // If discriminant is negative, ray misses the sphere
    if (discriminant < 0.0)
    {
        return false;
    }

    float sqrtDiscriminant = sqrt(discriminant);
    t0 = -b - sqrtDiscriminant;
    t1 = -b + sqrtDiscriminant;

    return true;
}


// Main Volumetric Fog Function
void VolumetricFog_float(
    float3 WorldPos, // Input: World Position of the fragment
    float3 CameraPos, // Input: World Space Camera Position (_WorldSpaceCameraPos)
    float MaxSteps, // Input: Maximum number of steps
    float StepSize, // Input: Size of each step
    float DensityScale, // Input: Overall fog density multiplier
    float4 Sphere, // Input: Fog volume sphere (center xyz, radius w)
    float NoiseScale, // Input: Scale of the noise pattern
    float3 NoiseOffset, // Input: Offset for noise animation/variation
    float2 NoiseRemap, // Input: Remapping range for noise (min, max)
    float NoisePower, // Input: Power applied to remapped noise
    float EdgeFadeWidth, // Input: Width of the fade near the sphere edge (0-1, relative to radius)
    out float AccumulatedDensity // Output: Final calculated fog density
)
{
    AccumulatedDensity = 0;

    float3 viewVector = WorldPos - CameraPos;
    float viewDistance = length(viewVector);
    // Avoid division by zero for fragments very close to camera
    if (viewDistance < 0.0001) {
        return;
    }
    float3 viewDirection = viewVector / viewDistance;

    float tNear, tFar;
    bool intersects = IntersectSphere(CameraPos, viewDirection, Sphere, tNear, tFar);

    if (!intersects || tFar < 0.0 || tNear > viewDistance)
    {
        // Ray doesn't intersect the sphere in front of the camera
        // or the intersection is entirely behind the fragment
        return;
    }

    // Clamp intersection distances to valid range
    // Start marching from the camera or the sphere entry point, whichever is further
    float marchStartT = max(0.0, tNear);
    // Stop marching at the fragment position or the sphere exit point, whichever is closer
    float marchEndT = min(viewDistance, tFar);

    // Ensure we don't march backwards
    if (marchStartT >= marchEndT) {
         return;
    }

    float currentT = marchStartT;
    // Adjust starting point slightly to avoid immediate exit if starting exactly on the boundary
    // and to align steps better. You might need to experiment with this offset.
    // A small random offset can also help reduce banding artifacts.
    // currentT += rand(WorldPos.xy) * StepSize; // Requires a rand function
    currentT += 0.001; // Small fixed offset

    int stepsTaken = 0;
    float accumulatedDensity = 0;

    // Raymarch through the intersecting segment
    // Use MaxSteps as a safeguard
    // Using a variable step count based on distance is often better, but needs care
    // Fixed step count is simpler here
    float marchLength = marchEndT - currentT; // Use currentT after potential offset
    int numSteps = min(MaxSteps, ceil(marchLength / StepSize)); // Calculate steps needed

    if (numSteps <= 0) return; // No steps needed for this segment

    float actualStepSize = marchLength / numSteps; // Distribute steps evenly over the segment

    for (int i = 0; i < numSteps; ++i)
    {
        // Calculate sample position along the view ray
        float3 samplePos = CameraPos + viewDirection * currentT;

        // Calculate distance from sample point to sphere center for edge fade
        float sphereDist = distance(samplePos, Sphere.xyz);

        // Calculate edge fade factor (ensure fade width is reasonable)
        float fadeWidth = max(0.01, EdgeFadeWidth) * Sphere.w; // Avoid divide by zero
        float edgeDistance = Sphere.w - sphereDist;
        float edgeFactor = saturate(edgeDistance / fadeWidth);

        // Sample noise (using your existing method)
        float3 noiseSamplePos1 = samplePos * NoiseScale + NoiseOffset;
        float noise1 = Noise3D(noiseSamplePos1);
        float3 noiseSamplePos2 = samplePos * NoiseScale - NoiseOffset;
        float noise2 = Noise3D(noiseSamplePos2);
        float combinedNoise = noise1 * noise2; // Or maybe lerp/add? Experiment here.

        // Remap, Power and Falloff
        combinedNoise = lerp(NoiseRemap.x, NoiseRemap.y, combinedNoise);
        combinedNoise = pow(saturate(combinedNoise), NoisePower); // Saturate before power often safer
        combinedNoise *= edgeFactor; // Apply edge fade

        // Accumulate density, scaling by step size to approximate integration
        // The density value should represent density *per unit distance*
        accumulatedDensity += combinedNoise * actualStepSize;

        // Move to the next step position
        currentT += actualStepSize;

        // Optional: Break early if density gets very high (optimization)
        // if(accumulatedDensity * DensityScale > SomeThreshold) break;
    }

    // Apply final density scale
    AccumulatedDensity = accumulatedDensity * DensityScale;

    // Optional: Apply Beer's Law for extinction/scattering (more physically based)
    float transmittance = exp(-AccumulatedDensity * DensityScale);
    AccumulatedDensity = 1.0 - transmittance; // Or use this for fog color blending
}