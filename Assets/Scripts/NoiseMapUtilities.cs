using UnityEngine;

public static class NoiseMapUtilities
{

    public static Texture2D GenerateNoiseTexture(float[,] noiseMap)
    {
        int mapWidth = noiseMap.GetLength(0);
        int mapHeight = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(mapWidth, mapHeight, TextureFormat.RGBA32, -1, true);

        Color[] colorMap = new Color[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                colorMap[y * mapWidth + x] =
                    Color.Lerp(Color.black, Color.white, noiseMap[mapWidth - x - 1, mapHeight - y - 1]);
            }
        }
        texture.SetPixels(colorMap);
        texture.Apply();

        return texture;
    }

    public static Texture2D GenerateColorMap(ColorThreshold[] colors)
    {
        Texture2D colorTexture = new Texture2D(colors.Length, 2, TextureFormat.RGBA32, -1, true);
        Color[] color_data = new Color[2 * colors.Length];
        int i = 0;
        for (; i < colors.Length; i++)
        {
            color_data[i] = colors[i].color;
        }
        for (; i < 2 * colors.Length; i++)
        {
            color_data[i] = new Color(colors[i - colors.Length].threshold, colors[i - colors.Length].threshold, colors[i - colors.Length].threshold, 0);
        }

        colorTexture.SetPixels(color_data);
        colorTexture.Apply();


        return colorTexture;
    }

    public static float[,] GeneratePerlinNoiseMap(
            NoiseSource noiseSource,
            NoiseMapConfig noiseMapConfig,
            FBmParams fBmParams,
            bool applyEaseFunction,
            bool applyCurve,
            AnimationCurve curve
            )
    {
        uint size = noiseMapConfig.size;
        float scale = noiseMapConfig.scale;
        float offsetX = noiseMapConfig.offsetX;
        float offsetY = noiseMapConfig.offsetY;
        float improvedNoiseZ = noiseMapConfig.improvedNoiseZ;
        if (noiseMapConfig.offsetBySize)
        {
            offsetX *= size;
            offsetY *= size;
        }

        float[,] heightMap = new float[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float pnX = (offsetX + (float)x) / scale;
                float pnY = (offsetY + (float)y) / scale;
                if (fBmParams.octaveCount > 1)
                    heightMap[x, y] = CalculateFBM(pnX, pnY, noiseSource, fBmParams, improvedNoiseZ);
                else if (noiseSource == NoiseSource.ImprovedNoise)
                    heightMap[x, y] = PerlinNoiseGenerator.NormalizedPerlinNoise(pnX, pnY, improvedNoiseZ);
                else
                    heightMap[x, y] = Mathf.PerlinNoise(pnX, pnY);
                if (applyEaseFunction)
                    heightMap[x, y] = EaseFunction(heightMap[x, y]);
                if (applyCurve)
                    heightMap[x, y] = curve.Evaluate(heightMap[x, y]);

            }
        }

        return heightMap;
    }

    public static float CalculateFBM(float x, float y, NoiseSource noiseSource, FBmParams fBmParams, float improvedNoiseZ = 1)
    {
        float total = 0.0f;
        float sumOfAmplitudes = 0.0f;
        float amplitude = fBmParams.amplitude;
        float frequency = fBmParams.frequency;

        for (int i = 0; i < fBmParams.octaveCount; i++)
        {
            if (noiseSource == NoiseSource.ImprovedNoise)
                total += amplitude * PerlinNoiseGenerator.NormalizedPerlinNoise(x * frequency, y * frequency, improvedNoiseZ);
            else
                total += amplitude * Mathf.PerlinNoise(x * frequency, y * frequency);
            sumOfAmplitudes += amplitude;
            amplitude *= fBmParams.persistence;
            frequency *= fBmParams.lacunarity;
        }
        return total / sumOfAmplitudes;
    }

    //https://mrl.cs.nyu.edu/~perlin/noise/
    static float EaseFunction(float t)
    {   // 6t^5 - 15t^4 + 10t^3
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

}
