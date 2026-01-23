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
        if (mapgen.noiseSource == NoiseSource.ImprovedNoise)
        {

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.IntField(new GUIContent("Permutation Table Size"), PerlinNoiseGenerator.GetPermutationTableSize());
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Randomize Permutation Table"))
            {
                PerlinNoiseGenerator.RandomizePermutationTable(mapgen.noiseMapConfig.improvedNoiseMapSizeMultiplier);
                mapgen.GenerateMap();
            }
            if (GUILayout.Button("Ken Perlin Permutation Table"))
            {
                PerlinNoiseGenerator.RestoreKenPerlinPermutationTable();
                mapgen.GenerateMap();
            }
        }
        if (GUILayout.Button("Export Mesh"))
            if (!Application.isPlaying)
            {
                Mesh mesh = mapgen.gameObject.GetComponent<MeshFilter>().sharedMesh;
                NoiseBasedMesh.ExportMeshToPLY(mesh);
            }
            else
            {
                Mesh mesh = mapgen.gameObject.GetComponent<MeshFilter>().sharedMesh;
                NoiseBasedMesh.ExportMeshToPLY(mesh);
            }
    }
}
