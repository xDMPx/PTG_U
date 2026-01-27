using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    //NoiseMapConfig
    IntegerField sizeIF;
    SliderInt sizeSI;
    FloatField scaleFF;
    Slider scaleS;
    FloatField offsetXFF;
    FloatField offsetYFF;
    Toggle offsetBySizeT;
    GroupBox improvedNoise_GB;
    FloatField inOffsetZFF;
    IntegerField inMapSizeMulIF;

    FloatField meshHeightFF;
    IntegerField lodIF;

    DropdownField noiseSource_DF;
    Toggle applyEaseFunctionT;
    Toggle useAvgT;

    DropdownField cShader_DF;
    FloatField waterPlaneThresholdFF;
    Slider waterPlaneThresholdS;
    Toggle waterPlaneT;
    Toggle noisePlaneT;

    IntegerField permTableSizeIF;
    Button randomizePT_Button;
    Button kenPerlinPT_Button;

    Toggle autoUpdateT;
    Button exportMeshButton;
    Button generateButton;

    public MapGen mapgen;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;


        autoUpdateT = root.Q<Toggle>("AutoUpdate_T");
        autoUpdateT.value = false;

        //PermutationTableSize
        permTableSizeIF = root.Q<IntegerField>("PermTableSize_IF");
        permTableSizeIF.value = PerlinNoiseGenerator.GetPermutationTableSize();

        //Randomize Permutation Table
        randomizePT_Button = root.Q<Button>("RandomizePT_B");
        randomizePT_Button.clicked += () =>
        {
            Validate();
            PerlinNoiseGenerator.RandomizePermutationTable(mapgen.noiseMapConfig.improvedNoiseMapSizeMultiplier);
            permTableSizeIF.value = PerlinNoiseGenerator.GetPermutationTableSize();
            AutoUpdate();
        };
        //Ken Perlin Permutation Table
        kenPerlinPT_Button = root.Q<Button>("KenPerlinPT_B");
        kenPerlinPT_Button.clicked += () =>
        {
            Validate();
            PerlinNoiseGenerator.RestoreKenPerlinPermutationTable();
            permTableSizeIF.value = PerlinNoiseGenerator.GetPermutationTableSize();
            AutoUpdate();
        };

        //ImprovedNoiseConfig
        improvedNoise_GB = root.Q<GroupBox>("ImprovedNoise_GB");

        //Size
        sizeIF = root.Q<IntegerField>("Size_IF");
        sizeIF.value = (int)mapgen.noiseMapConfig.size;
        sizeIF.RegisterCallback<KeyDownEvent>(x =>
        {
            if (x.keyCode == KeyCode.Return)
            {
                Validate();
                sizeSI.value = sizeIF.value;
                mapgen.noiseMapConfig.size = (uint)sizeIF.value;
                AutoUpdate();
            }
        });
        sizeIF.RegisterCallback<FocusOutEvent>(x =>
        {
            Validate();
            sizeSI.value = sizeIF.value;
            mapgen.noiseMapConfig.size = (uint)sizeIF.value;
            AutoUpdate();
        });

        sizeSI = root.Q<SliderInt>("Size_SI");
        sizeSI.value = (int)mapgen.noiseMapConfig.size;
        sizeSI.RegisterValueChangedCallback(x =>
        {
            Validate();
            sizeIF.value = x.newValue;
            mapgen.noiseMapConfig.size = (uint)x.newValue;
            AutoUpdate();
        });


        //Scale
        scaleFF = root.Q<FloatField>("Scale_FF");
        scaleFF.value = mapgen.noiseMapConfig.scale;
        scaleFF.RegisterCallback<KeyDownEvent>(x =>
        {
            if (x.keyCode == KeyCode.Return)
            {
                Validate();
                scaleS.value = scaleFF.value;
                mapgen.noiseMapConfig.scale = scaleFF.value;
                AutoUpdate();
            }
        });
        scaleFF.RegisterCallback<FocusOutEvent>(x =>
        {
            Validate();
            scaleS.value = scaleFF.value;
            mapgen.noiseMapConfig.scale = scaleFF.value;
            AutoUpdate();
        });

        scaleS = root.Q<Slider>("Scale_S");
        scaleS.value = mapgen.noiseMapConfig.scale;
        scaleS.RegisterValueChangedCallback(x =>
        {
            Validate();
            scaleFF.value = x.newValue;
            mapgen.noiseMapConfig.scale = x.newValue;
            AutoUpdate();
        });

        //OffsetBySize
        offsetBySizeT = root.Q<Toggle>("OffsetBySize_T");
        offsetBySizeT.value = mapgen.noiseMapConfig.offsetBySize;
        offsetBySizeT.RegisterValueChangedCallback(x =>
        {
            Validate();
            mapgen.noiseMapConfig.offsetBySize = x.newValue;
            AutoUpdate();
        });

        //OffsetX
        offsetXFF = root.Q<FloatField>("OffsetX_FF");
        offsetXFF.value = mapgen.noiseMapConfig.offsetX;
        offsetXFF.RegisterCallback<KeyDownEvent>(x =>
        {
            if (x.keyCode == KeyCode.Return)
            {
                Validate();
                mapgen.noiseMapConfig.offsetX = offsetXFF.value;
                AutoUpdate();
            }
        });
        offsetXFF.RegisterCallback<FocusOutEvent>(x =>
        {
            Validate();
            mapgen.noiseMapConfig.offsetX = offsetXFF.value;
            AutoUpdate();
        });

        //OffsetY
        offsetYFF = root.Q<FloatField>("OffsetY_FF");
        offsetYFF.value = mapgen.noiseMapConfig.offsetY;
        offsetYFF.RegisterCallback<KeyDownEvent>(x =>
        {
            if (x.keyCode == KeyCode.Return)
            {
                Validate();
                mapgen.noiseMapConfig.offsetY = offsetYFF.value;
                AutoUpdate();
            }
        });
        offsetYFF.RegisterCallback<FocusOutEvent>(x =>
        {
            Validate();
            mapgen.noiseMapConfig.offsetY = offsetYFF.value;
            AutoUpdate();
        });

        //ImprovedNoiseConfigOffsetZ
        inOffsetZFF = root.Q<FloatField>("InOffsetZ_FF");
        inOffsetZFF.value = mapgen.noiseMapConfig.improvedNoiseZ;
        inOffsetZFF.RegisterCallback<KeyDownEvent>(x =>
        {
            if (x.keyCode == KeyCode.Return)
            {
                Validate();
                mapgen.noiseMapConfig.improvedNoiseZ = inOffsetZFF.value;
                AutoUpdate();
            }
        });
        inOffsetZFF.RegisterCallback<FocusOutEvent>(x =>
        {
            Validate();
            mapgen.noiseMapConfig.improvedNoiseZ = inOffsetZFF.value;
            AutoUpdate();
        });

        //ImprovedNoiseConfigMapSizeMultiplier
        inMapSizeMulIF = root.Q<IntegerField>("InMapSizeMul_IF");
        inMapSizeMulIF.value = (int)mapgen.noiseMapConfig.improvedNoiseMapSizeMultiplier;
        inMapSizeMulIF.RegisterCallback<KeyDownEvent>(x =>
        {
            if (x.keyCode == KeyCode.Return)
            {
                Validate();
                mapgen.noiseMapConfig.improvedNoiseMapSizeMultiplier = (uint)inMapSizeMulIF.value;
                AutoUpdate();
            }
        });
        inMapSizeMulIF.RegisterCallback<FocusOutEvent>(x =>
        {
            Validate();
            mapgen.noiseMapConfig.improvedNoiseMapSizeMultiplier = (uint)inMapSizeMulIF.value;
            AutoUpdate();
        });

        //MeshHeight
        meshHeightFF = root.Q<FloatField>("MeshHeight_FF");
        meshHeightFF.value = mapgen.meshHeight;
        meshHeightFF.RegisterCallback<KeyDownEvent>(x =>
        {
            if (x.keyCode == KeyCode.Return)
            {
                Validate();
                mapgen.meshHeight = meshHeightFF.value;
                AutoUpdate();
            }
        });
        meshHeightFF.RegisterCallback<FocusOutEvent>(x =>
        {
            Validate();
            mapgen.meshHeight = meshHeightFF.value;
            AutoUpdate();
        });

        //MeshLOD
        lodIF = root.Q<IntegerField>("LOD_IF");
        lodIF.value = (int)mapgen.meshLOD;
        lodIF.RegisterCallback<KeyDownEvent>(x =>
        {
            if (x.keyCode == KeyCode.Return)
            {
                Validate();
                mapgen.meshLOD = (uint)lodIF.value;
                AutoUpdate();
            }
        });
        lodIF.RegisterCallback<FocusOutEvent>(x =>
        {
            Validate();
            mapgen.meshLOD = (uint)lodIF.value;
            AutoUpdate();
        });

        //NoiseSource
        noiseSource_DF = root.Q<DropdownField>("NoiseSource_DF");
        noiseSource_DF.RegisterValueChangedCallback(x =>
        {
            if (x.newValue == "Unity")
            {
                mapgen.noiseSource = NoiseSource.Unity;
            }
            else if (x.newValue == "Improved Noise")
            {
                mapgen.noiseSource = NoiseSource.ImprovedNoise;
            }
            if (mapgen.noiseSource == NoiseSource.ImprovedNoise)
            {
                randomizePT_Button.style.display = DisplayStyle.Flex;
                kenPerlinPT_Button.style.display = DisplayStyle.Flex;
                improvedNoise_GB.style.display = DisplayStyle.Flex;
                permTableSizeIF.style.display = DisplayStyle.Flex;
            }
            else
            {
                randomizePT_Button.style.display = DisplayStyle.None;
                kenPerlinPT_Button.style.display = DisplayStyle.None;
                improvedNoise_GB.style.display = DisplayStyle.None;
                permTableSizeIF.style.display = DisplayStyle.None;
            }
            AutoUpdate();
        });
        if (mapgen.noiseSource == NoiseSource.Unity)
        {
            noiseSource_DF.value = noiseSource_DF.choices[0];
        }
        else if (mapgen.noiseSource == NoiseSource.ImprovedNoise)
        {
            noiseSource_DF.value = noiseSource_DF.choices[1];
        }

        //ApplyEaseFunction
        applyEaseFunctionT = root.Q<Toggle>("ApplyEaseFunction_T");
        applyEaseFunctionT.value = mapgen.applyEaseFunction;
        applyEaseFunctionT.RegisterValueChangedCallback(x =>
        {
            Validate();
            mapgen.applyEaseFunction = x.newValue;
            AutoUpdate();
        });

        //ApplyEaseFunction
        useAvgT = root.Q<Toggle>("UseAvg_T");
        useAvgT.value = mapgen.useAvg;
        useAvgT.RegisterValueChangedCallback(x =>
        {
            Validate();
            mapgen.useAvg = x.newValue;
            AutoUpdate();
        });

        //CShader
        cShader_DF = root.Q<DropdownField>("CShader_DF");
        cShader_DF.RegisterValueChangedCallback(x =>
        {
            if (x.newValue == "Height")
            {
                mapgen.cshader = ColoringShader.HEIGHT;
            }
            else if (x.newValue == "Texture")
            {
                mapgen.cshader = ColoringShader.TEXTURE;
            }
            AutoUpdate();
        });
        if (mapgen.cshader == ColoringShader.HEIGHT)
        {
            cShader_DF.value = cShader_DF.choices[0];
        }
        else if (mapgen.cshader == ColoringShader.TEXTURE)
        {
            cShader_DF.value = cShader_DF.choices[1];
        }


        //WaterPlaneThreshold
        waterPlaneThresholdFF = root.Q<FloatField>("WaterPlaneThreshold_FF");
        waterPlaneThresholdFF.value = mapgen.waterPlaneThreshold;
        waterPlaneThresholdFF.RegisterCallback<KeyDownEvent>(x =>
        {
            if (x.keyCode == KeyCode.Return)
            {
                Validate();
                waterPlaneThresholdS.value = waterPlaneThresholdFF.value;
                mapgen.waterPlaneThreshold = waterPlaneThresholdFF.value;
                AutoUpdate();
            }
        });
        waterPlaneThresholdFF.RegisterCallback<FocusOutEvent>(x =>
        {
            Validate();
            waterPlaneThresholdS.value = waterPlaneThresholdFF.value;
            mapgen.waterPlaneThreshold = waterPlaneThresholdFF.value;
            AutoUpdate();
        });

        waterPlaneThresholdS = root.Q<Slider>("WaterPlaneThreshold_S");
        waterPlaneThresholdS.value = mapgen.waterPlaneThreshold;
        waterPlaneThresholdS.RegisterValueChangedCallback(x =>
        {
            Validate();
            waterPlaneThresholdFF.value = x.newValue;
            mapgen.waterPlaneThreshold = x.newValue;
            AutoUpdate();
        });

        //ShowWaterPlane
        waterPlaneT = root.Q<Toggle>("WaterPlane_T");
        waterPlaneT.value = mapgen.showWaterPlane;
        waterPlaneT.RegisterValueChangedCallback(x =>
        {
            Validate();
            mapgen.showWaterPlane = x.newValue;
            AutoUpdate();
        });

        //ShowNoisePlane
        noisePlaneT = root.Q<Toggle>("NoisePlane_T");
        noisePlaneT.value = mapgen.showNoisePlane;
        noisePlaneT.RegisterValueChangedCallback(x =>
        {
            Validate();
            mapgen.showNoisePlane = x.newValue;
            AutoUpdate();
        });

        //ExportMesh
        exportMeshButton = root.Q<Button>("ExportMesh_B");
        exportMeshButton.clicked += () =>
        {
            Validate();
            mapgen.GenerateMap();
            Mesh mesh = mapgen.gameObject.GetComponent<MeshFilter>().sharedMesh;
            NoiseBasedMesh.ExportMeshToPLY(mesh);
        };

        //Generate
        generateButton = root.Q<Button>("Generate_B");
        generateButton.clicked += () =>
        {
            Validate();
            mapgen.GenerateMap();
        };
    }

    void Validate()
    {
        if (sizeSI.value < 10) sizeSI.value = 10;
        if (sizeIF.value < 10) sizeIF.value = 10;
        if (scaleFF.value < 0.1f) scaleFF.value = 0.1f;
        if (scaleS.value < 0.1f) scaleS.value = 0.1f;
        if (offsetXFF.value < 0) offsetXFF.value = 0;
        if (offsetYFF.value < 0) offsetYFF.value = 0;
        if (inOffsetZFF.value < 0) inOffsetZFF.value = 0;
        if (inMapSizeMulIF.value == 0) inMapSizeMulIF.value = 1;
        if (inMapSizeMulIF.value > 10) inMapSizeMulIF.value = 10;
        if (meshHeightFF.value < 0) meshHeightFF.value = 1;
        if (lodIF.value < 0) lodIF.value = 0;
        if (waterPlaneThresholdS.value < 0.01) waterPlaneThresholdS.value = 0.01f;
        if (waterPlaneThresholdS.value > 1) waterPlaneThresholdS.value = 1f;
        permTableSizeIF.value = PerlinNoiseGenerator.GetPermutationTableSize();
    }

    void AutoUpdate()
    {
        if (autoUpdateT.value)
        {
            mapgen.GenerateMap();
        }
    }

    void Update()
    {

    }
}
