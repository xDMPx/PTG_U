using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    public NoiseMapConfig noiseMapConfig = new NoiseMapConfig(500, 110.3f, false, 0, 0);
    public FBmParams fBmParams = new FBmParams(1, 1, 0.5f, 2.0f, 6);

    public float meshHeight = 100f;
    public uint meshLOD = 0;

    public bool autoUpdateInEditor = false;
    public bool applyEaseFunction = false;
    public bool applyCurve = false;

    public ColorThreshold[] colors = new ColorThreshold[4] {
        new ColorThreshold(Color.blue, 0.1f),
        new ColorThreshold(Color.yellow, 0.2f),
        new ColorThreshold(Color.green, 0.5f),
        new ColorThreshold(Color.gray, 1.0f)
    };

    public AnimationCurve curve;
    public bool showNoisePlane = true;
    public Material noisePlaneMaterial;

    private GameObject noisePlane;
    private Texture2D noiseTexture;
    private bool update = false;

    public ColoringShader cshader = ColoringShader.TEXTURE;
    public Material textureShaderMaterial;
    public Material heightShaderMaterial;

    // Start is called before the first frame update
    void Start()
    {
        if (noisePlane == null)
            noisePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {
        if (update)
        {
            update = false;
            GenerateMap();
        }
    }

    void OnValidate()
    {

        if (noiseMapConfig.size < 10) noiseMapConfig.size = 10;
        if (noiseMapConfig.scale < 0.1f) noiseMapConfig.scale = 0.1f;
        if (noiseMapConfig.offsetX < 0) noiseMapConfig.offsetX = 0;
        if (noiseMapConfig.offsetY < 0) noiseMapConfig.offsetY = 0;
        if (fBmParams.amplitude < 1) fBmParams.amplitude = 1;
        if (fBmParams.frequency < 1) fBmParams.frequency = 1;
        if (colors.Length < 1)
        {
            Array.Resize(ref colors, 1);
        }
        update = true;
    }

    public void GenerateMap()
    {
        if (noisePlane == null && showNoisePlane == true) Start();
        if (noisePlane != null && showNoisePlane == false) DestroyImmediate(noisePlane);

        float[,] noiseMap = generatePerlinNoiseMap(
                noiseMapConfig,
                fBmParams,
                applyEaseFunction,
                applyCurve,
                curve);

        gameObject.GetComponent<MeshFilter>().mesh = NoiseBasedMesh.generateMeshfromNoiseMap(noiseMap, meshHeight, meshLOD);

        if (cshader == ColoringShader.TEXTURE || showNoisePlane)
        {
            noiseTexture = GenerateNoiseTexture(noiseMap);
            if (showNoisePlane)
            {
                noisePlane.GetComponent<Renderer>().sharedMaterial = noisePlaneMaterial;
                noisePlane.GetComponent<Renderer>().sharedMaterial.mainTexture = noiseTexture;
                int mapWidth = noiseMap.GetLength(0);
                int mapHeight = noiseMap.GetLength(1);
                noisePlane.transform.localScale = new Vector3(mapWidth / 10.0f, 1, mapHeight / 10.0f);
                noisePlane.transform.position = gameObject.transform.position;
            }
        }

        if (cshader == ColoringShader.TEXTURE)
        {
            gameObject.GetComponent<Renderer>().sharedMaterial = textureShaderMaterial;
            gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = noiseTexture;
            gameObject.GetComponent<Renderer>().sharedMaterial.SetTexture("_colorMap", generateColorMap());
        }
        else if (cshader == ColoringShader.HIGHT)
        {
            gameObject.GetComponent<Renderer>().sharedMaterial = heightShaderMaterial;
            gameObject.GetComponent<Renderer>().sharedMaterial.SetTexture("_colorMap", generateColorMap());
            gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_meshHeight", meshHeight);
        }
    }

    Texture2D GenerateNoiseTexture(float[,] noiseMap)
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

    Texture2D generateColorMap()
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

    public static float[,] generatePerlinNoiseMap(
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
                    heightMap[x, y] = calculateFBM(pnX, pnY, fBmParams);
                else
                    heightMap[x, y] = PerlinNoiseGenerator.NormalizedPerlinNoise(pnX, pnY);
                if (applyEaseFunction)
                    heightMap[x, y] = easeFunction(heightMap[x, y]);
                if (applyCurve)
                    heightMap[x, y] = curve.Evaluate(heightMap[x, y]);

            }
        }

        return heightMap;
    }

    private static float calculateFBM(float x, float y, FBmParams fBmParams)
    {
        float total = 0.0f;
        float sumOfAmplitudes = 0.0f;
        float amplitude = fBmParams.amplitude;
        float frequency = fBmParams.frequency;

        for (int i = 0; i < fBmParams.octaveCount; i++)
        {
            total += amplitude * PerlinNoiseGenerator.NormalizedPerlinNoise(x * frequency, y * frequency);
            sumOfAmplitudes += amplitude;
            amplitude *= fBmParams.persistence;
            frequency *= fBmParams.lacunarity;
        }
        return total / sumOfAmplitudes;
    }

    //https://mrl.cs.nyu.edu/~perlin/noise/
    static float easeFunction(float t)
    {   // 6t^5 - 15t^4 + 10t^3
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

}
