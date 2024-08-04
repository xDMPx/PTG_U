using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    public uint size = 500;
    public float scale = 110.3f;
    public float offsetX = 0;
    public float offsetY = 0;

    public bool autoUpdateInEditor = false;
    public bool applyEaseFunction = false;
    public Material noisePlaneMaterial;

    private MeshFilter meshFilter;
    private GameObject noisePlane;
    private bool update = false;

    // Start is called before the first frame update
    void Start()
    {
        if (noisePlane == null)
            noisePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        meshFilter = gameObject.GetComponent<MeshFilter>();
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
        if (size < 10) size = 10;
        if (scale < 0.1f) scale = 0.1f;
        if (offsetX < 0) offsetX = 0;
        if (offsetY < 0) offsetY = 0;
        update = true;
    }

    public void GenerateMap()
    {
        if (noisePlane == null) Start();
        float[,] heightMap = generatePerlinNoiseMap(size, offsetX, offsetY, scale, applyEaseFunction);
        GenerateNoiseTexture(heightMap);
    }

    void GenerateNoiseTexture(float[,] noiseMap)
    {
        int mapWidth = noiseMap.GetLength(0);
        int mapHeight = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(mapWidth, mapHeight);

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
    }

    public static float[,] generatePerlinNoiseMap(uint size, float offsetX, float offsetY, float scale, bool applyEaseFunction)
    {
        float[,] heightMap = new float[size, size];
        for (float y = 0; y < size; y++)
        {
            for (float x = 0; x < size; x++)
            {
                float pnX = (offsetX + x) / scale;
                float pnY = (offsetY + y) / scale;
                heightMap[(int)x, (int)y] = Mathf.PerlinNoise(pnX, pnY);
                if (applyEaseFunction)
                    heightMap[(int)x, (int)y] = easeFunction(heightMap[(int)x, (int)y]);
            }
        }

        return heightMap;
    }

    //https://mrl.cs.nyu.edu/~perlin/noise/
    static float easeFunction(float t)
    {   // 6t^5 - 15t^4 + 10t^3
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

}
