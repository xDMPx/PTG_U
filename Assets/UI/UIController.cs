using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    FloatField offsetXFF;
    FloatField offsetYFF;

    Button generateButton;

    public MapGen mapgen;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

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
        if (offsetXFF.value < 0) offsetXFF.value = 0;
        if (offsetYFF.value < 0) offsetYFF.value = 0;
    }

    void Update()
    {

    }
}
