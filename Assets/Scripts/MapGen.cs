using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{

    public NoiseSource noiseSource = NoiseSource.Unity;
    public NoiseMapConfig noiseMapConfig = new NoiseMapConfig(500, 110.3f, false, 0, 0, 1);
    public FBmParams fBmParams = new FBmParams(1, 1, 0.5f, 2.0f, 6);

    public float meshHeight = 100f;
    public float waterPlaneThreshold = 0.1f;
    public uint meshLOD = 0;
    public bool useAvg = false;

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
    public bool showWaterPlane = true;
    public Material noisePlaneMaterial;
    public Material waterPlaneMaterial;

    private GameObject noisePlane;
    private GameObject waterPlane;
    private Texture2D noiseTexture;
    private bool update = false;

    public ColoringShader cshader = ColoringShader.TEXTURE;
    public Material textureShaderMaterial;
    public Material heightShaderMaterial;

    // Start is called before the first frame update
    void Start()
    {
        if (noisePlane == null)
        {
            noisePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            noisePlane.name = "noisePlane";
        }
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
        transform.Rotate(new Vector3(0, Time.deltaTime, 0));
        if (waterPlane != null)
        {
            waterPlane.transform.Rotate(new Vector3(0, Time.deltaTime, 0));
        }
        if (noisePlane != null)
        {
            noisePlane.transform.Rotate(new Vector3(0, Time.deltaTime, 0));
        }
    }

    void OnValidate()
    {
        if (noiseMapConfig.size < 10) noiseMapConfig.size = 10;
        if (noiseMapConfig.scale < 0.1f) noiseMapConfig.scale = 0.1f;
        if (noiseMapConfig.offsetX < 0) noiseMapConfig.offsetX = 0;
        if (noiseMapConfig.offsetY < 0) noiseMapConfig.offsetY = 0;
        if (noiseMapConfig.improvedNoiseZ < 0) noiseMapConfig.offsetY = 0;
        if (noiseMapConfig.improvedNoiseMapSizeMultiplier == 0) noiseMapConfig.improvedNoiseMapSizeMultiplier = 1;
        if (noiseMapConfig.improvedNoiseMapSizeMultiplier > 10) noiseMapConfig.improvedNoiseMapSizeMultiplier = 10;
        if (fBmParams.amplitude < 1) fBmParams.amplitude = 1;
        if (fBmParams.frequency < 1) fBmParams.frequency = 1;
        if (meshHeight < 0) meshHeight = 1;
        if (waterPlaneThreshold < 0.01) waterPlaneThreshold = 0.01f;
        if (waterPlaneThreshold > 1) waterPlaneThreshold = 1.0f;
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

        float[,] noiseMap = NoiseMapUtilities.GeneratePerlinNoiseMap(
                noiseSource,
                noiseMapConfig,
                fBmParams,
                applyEaseFunction,
                applyCurve,
                curve);

        if (!Application.isPlaying)
        {
            if (gameObject.GetComponent<MeshFilter>().sharedMesh != null)
            {
                DestroyImmediate(gameObject.GetComponent<MeshFilter>().sharedMesh);
            }
            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            gameObject.GetComponent<MeshFilter>().sharedMesh = NoiseBasedMesh.GenerateMeshfromNoiseMap(noiseMap, meshHeight, meshLOD, useAvg);
            //sw.Stop();
            //Debug.Log($"MeshGen 1000 avg cache: {sw.Elapsed.TotalSeconds / 1000}");

        }
        else
        {
            if (gameObject.GetComponent<MeshFilter>().mesh != null)
            {
                Destroy(gameObject.GetComponent<MeshFilter>().mesh);
            }
            gameObject.GetComponent<MeshFilter>().mesh = NoiseBasedMesh.GenerateMeshfromNoiseMap(noiseMap, meshHeight, meshLOD, useAvg);
        }

        if (cshader == ColoringShader.TEXTURE || showNoisePlane)
        {
            noiseTexture = NoiseMapUtilities.GenerateNoiseTexture(noiseMap);
            if (showNoisePlane)
            {
                if (!Application.isPlaying)
                {
                    noisePlane.GetComponent<Renderer>().sharedMaterial = noisePlaneMaterial;
                    noisePlane.GetComponent<Renderer>().sharedMaterial.mainTexture = noiseTexture;
                }
                else
                {
                    noisePlane.GetComponent<Renderer>().material = noisePlaneMaterial;
                    noisePlane.GetComponent<Renderer>().material.mainTexture = noiseTexture;
                }
                int mapWidth = noiseMap.GetLength(0);
                int mapHeight = noiseMap.GetLength(1);
                noisePlane.transform.localScale = new Vector3(mapWidth / 10.0f, 1, mapHeight / 10.0f);
                noisePlane.transform.position = gameObject.transform.position;
            }
        }

        if (cshader == ColoringShader.TEXTURE)
        {
            if (!Application.isPlaying)
            {
                gameObject.GetComponent<Renderer>().sharedMaterial = textureShaderMaterial;
                gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = noiseTexture;
                gameObject.GetComponent<Renderer>().sharedMaterial.SetTexture("_colorMap", NoiseMapUtilities.GenerateColorMap(colors));
            }
            else
            {
                gameObject.GetComponent<Renderer>().material = textureShaderMaterial;
                gameObject.GetComponent<Renderer>().material.mainTexture = noiseTexture;
                gameObject.GetComponent<Renderer>().material.SetTexture("_colorMap", NoiseMapUtilities.GenerateColorMap(colors));
            }
        }
        else if (cshader == ColoringShader.HEIGHT)
        {
            if (!Application.isPlaying)
            {
                gameObject.GetComponent<Renderer>().sharedMaterial = heightShaderMaterial;
                gameObject.GetComponent<Renderer>().sharedMaterial.SetTexture("_colorMap", NoiseMapUtilities.GenerateColorMap(colors));
                gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_meshHeight", meshHeight);
            }
            else
            {
                gameObject.GetComponent<Renderer>().material = heightShaderMaterial;
                gameObject.GetComponent<Renderer>().material.SetTexture("_colorMap", NoiseMapUtilities.GenerateColorMap(colors));
                gameObject.GetComponent<Renderer>().material.SetFloat("_meshHeight", meshHeight);
            }
        }

        displayWaterPlain();
    }

    void displayWaterPlain()
    {
        if (waterPlane == null)
        {
            waterPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            waterPlane.name = "WaterPlane";
        }
        if (waterPlane != null && showWaterPlane == false)
        {
            DestroyImmediate(waterPlane);
            return;
        }

        waterPlane.GetComponent<Renderer>().material = waterPlaneMaterial;
        waterPlane.transform.localScale = new Vector3(noiseMapConfig.size / 10.0f, 1, noiseMapConfig.size / 10.0f);
        waterPlane.transform.position = new Vector3(gameObject.transform.position.x,
                                          gameObject.transform.position.y + meshHeight * waterPlaneThreshold,
                                        gameObject.transform.position.z);
    }

}
