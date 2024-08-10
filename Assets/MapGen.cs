using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{

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
    public struct NoiseMapConfig
    {
        public uint size;
        public float scale;
        public float offsetX;
        public float offsetY;

        public NoiseMapConfig(uint size, float scale, float offsetX, float offsetY)
        {
            this.size = size;
            this.scale = scale;
            this.offsetX = offsetX;
            this.offsetY = offsetY;

        }
    }

    public NoiseMapConfig noiseMapConfig = new NoiseMapConfig(500, 110.3f, 0, 0);
    public FBmParams fBmParams = new FBmParams(1, 1, 0.5f, 2.0f, 6);

    public float meshHight = 100f;

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
    public Material noisePlaneMaterial;

    private GameObject noisePlane;
    private bool update = false;

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
        if (noisePlane == null) Start();

        float[,] heightMap = generatePerlinNoiseMap(
                noiseMapConfig,
                fBmParams,
                applyEaseFunction,
                applyCurve,
                curve);

        gameObject.GetComponent<MeshFilter>().mesh = generateMeshfromNoiseMap(heightMap, meshHight);
        GenerateNoiseTexture(heightMap);
    }

    void GenerateNoiseTexture(float[,] noiseMap)
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

        noisePlane.GetComponent<Renderer>().sharedMaterial = noisePlaneMaterial;
        noisePlane.GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
        noisePlane.transform.localScale = new Vector3(mapWidth / 10.0f, 1, mapHeight / 10.0f);
        noisePlane.transform.position = gameObject.transform.position;

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

        gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
        gameObject.GetComponent<Renderer>().sharedMaterial.SetTexture("_colorMap", colorTexture);
        gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_meshHeight", meshHight);
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
                    heightMap[x, y] = Mathf.PerlinNoise(pnX, pnY);
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
            total += amplitude * Mathf.PerlinNoise(x * frequency, y * frequency);
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

    public static Mesh generateMeshfromNoiseMap(float[,] noiseMap, float meshHight)
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Vector3[] vertices = new Vector3[width * height];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                vertices[y * width + x] = new Vector3(x - width / 2, noiseMap[x, y] * meshHight, y - height / 2);

        mesh.vertices = vertices;

        int triangles_num = ((width * height) - (width + height - 1)) * 2;
        int[] triangles = new int[triangles_num * 3];

        for (int i = 0, triangle = 0; i < vertices.Length - width; i++)
        {
            if (i % width == width - 1)
                continue;
            triangles[triangle] = i;
            triangles[triangle + 1] = i + width + 1;
            triangles[triangle + 2] = i + 1;
            triangles[triangle + 3] = i;
            triangles[triangle + 4] = i + width;
            triangles[triangle + 5] = i + width + 1;
            triangle += 6;
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = vertices.Length - 1; i >= 0; i--)
            uvs[vertices.Length - 1 - i] = new Vector2((vertices[i].x + width / 2) / width, (vertices[i].z + height / 2) / height);

        mesh.uv = uvs;

        return mesh;
    }

}
