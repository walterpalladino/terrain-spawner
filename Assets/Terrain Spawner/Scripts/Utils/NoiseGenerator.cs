using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator
{
    /// octaves : Number of iterations (the more there is, the more detailed the terrain will be)
    /// persistance : The higher it is, the rougher the terrain will be (this value should be between 0 and 1 excluded)
    /// lacunarity : The higher it is, the more "feature" the terrain will have (should be strictly positive)
    public static float GetNoiseAt(int x, int z, int octaves, float persistance, float lacunarity)
    {
        float perlinValue = 0f;
        float amplitude = 1f;
        float frequency = 1f;

        for (int i = 0; i < octaves; i++)
        {
            // Get the perlin value at that octave and add it to the sum
            perlinValue += Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude;

            // Decrease the amplitude and the frequency
            amplitude *= persistance;
            frequency *= lacunarity;
        }

        // Return the noise value
        return Mathf.Clamp(perlinValue, 0.0f, 1.0f);
    }
}
