using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGen))]
public class MapGenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGen mapgen = (MapGen)target;
        if (DrawDefaultInspector() && mapgen.autoUpdateInEditor) mapgen.GenerateMap();
        if (GUILayout.Button("Generate")) mapgen.GenerateMap();
        if (GUILayout.Button("Randomize Permutation Table"))
        {
            PerlinNoiseGenerator.RandomizePermutationTable();
            mapgen.GenerateMap();
        }
    }
}
