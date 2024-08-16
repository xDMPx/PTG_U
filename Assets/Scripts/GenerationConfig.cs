using System;
using UnityEngine;

[Serializable]
public enum ColoringShader
{
    HIGHT,
    TEXTURE,
}

[Serializable]
public struct NoiseMapConfig
{
    public uint size;
    public float scale;
    public bool offsetBySize;
    public float offsetX;
    public float offsetY;

    public NoiseMapConfig(uint size, float scale, bool offsetBySize, float offsetX, float offsetY)
    {
        this.size = size;
        this.scale = scale;
        this.offsetBySize = offsetBySize;
        this.offsetX = offsetX;
        this.offsetY = offsetY;

    }
}

[Serializable]
public struct FBmParams
{
    public float amplitude;
    public float frequency;
    public float persistence;
    public float lacunarity;
    public uint octaveCount;

    public FBmParams(
            float amplitude,
            float frequency,
            float persistence,
            float lacunarity,
            uint octaveCount)
    {
        this.amplitude = amplitude;
        this.frequency = frequency;
        this.persistence = persistence;
        this.lacunarity = lacunarity;
        this.octaveCount = octaveCount;
    }
}

[Serializable]
public struct ColorThreshold
{
    public Color color;
    public float threshold;

    public ColorThreshold(Color color, float threshold)
    {
        this.color = color;
        this.threshold = threshold;
    }
}
