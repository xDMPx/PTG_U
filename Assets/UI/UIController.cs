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


    DropdownField noiseSource_DF;

    Button generateButton;

    public MapGen mapgen;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

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
            }
        });
        sizeIF.RegisterCallback<FocusOutEvent>(x =>
        {
            Validate();
            sizeSI.value = sizeIF.value;
            mapgen.noiseMapConfig.size = (uint)sizeIF.value;
        });

        sizeSI = root.Q<SliderInt>("Size_SI");
        sizeSI.value = (int)mapgen.noiseMapConfig.size;
        sizeSI.RegisterValueChangedCallback(x =>
        {
            Validate();
            sizeIF.value = x.newValue;
            mapgen.noiseMapConfig.size = (uint)x.newValue;
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
            }
        });
        scaleFF.RegisterCallback<FocusOutEvent>(x =>
        {
            Validate();
            scaleS.value = scaleFF.value;
            mapgen.noiseMapConfig.scale = scaleFF.value;
        });

        scaleS = root.Q<Slider>("Scale_S");
        scaleS.value = mapgen.noiseMapConfig.scale;
        scaleS.RegisterValueChangedCallback(x =>
        {
            Validate();
            scaleFF.value = x.newValue;
            mapgen.noiseMapConfig.scale = x.newValue;
        });

        //OffsetBySize
        offsetBySizeT = root.Q<Toggle>("OffsetBySize_T");
        offsetBySizeT.value = mapgen.noiseMapConfig.offsetBySize;
        offsetBySizeT.RegisterValueChangedCallback(x =>
        {
            Validate();
            mapgen.noiseMapConfig.offsetBySize = x.newValue;
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
            }
        });
        offsetXFF.RegisterCallback<FocusOutEvent>(x =>
        {
            Validate();
            mapgen.noiseMapConfig.offsetX = offsetXFF.value;
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
            }
        });
        offsetYFF.RegisterCallback<FocusOutEvent>(x =>
        {
            Validate();
            mapgen.noiseMapConfig.offsetY = offsetYFF.value;
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
        });
        if (mapgen.noiseSource == NoiseSource.Unity)
        {
            noiseSource_DF.value = noiseSource_DF.choices[0];
        }
        else if (mapgen.noiseSource == NoiseSource.ImprovedNoise)
        {
            noiseSource_DF.value = noiseSource_DF.choices[1];
        }

        // Generate
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
    }

    void Update()
    {

    }
}
