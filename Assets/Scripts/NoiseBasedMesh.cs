using UnityEngine;

public static class NoiseBasedMesh
{

    public static Mesh generateMeshfromNoiseMap(float[,] noiseMap, float meshHight, uint LOD = 0)
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        uint meshIncrement = LOD <= 0 ? 1 : LOD * 2;
        Vector3[] vertices = meshVertices(noiseMap, meshHight, meshIncrement);
        mesh.vertices = vertices;

        int width = noiseMap.GetLength(0) / (int)meshIncrement;
        int height = noiseMap.GetLength(1) / (int)meshIncrement;
        int vertices_len = width * height;

        mesh.triangles = meshTriangles(width, height, vertices_len);
        mesh.uv = meshUVs(mesh.vertices, noiseMap.GetLength(0), noiseMap.GetLength(1));
        mesh.RecalculateNormals();

        return mesh;
    }

    public static Vector3[] meshVertices(float[,] noiseMap, float meshHight, uint meshIncrement)
    {
        uint width = (uint)noiseMap.GetLength(0) / meshIncrement;
        uint height = (uint)noiseMap.GetLength(1) / meshIncrement;

        Vector3[] vertices = new Vector3[width * height];

        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < height; x++)
            {
                vertices[y * width + x].x = (x - width / 2) * meshIncrement;
                vertices[y * width + x].y = noiseMap[x, y] * meshHight;
                vertices[y * width + x].z = (y - height / 2) * meshIncrement;
            }
        }

        return vertices;
    }

    public static int[] meshTriangles(int width, int height, int vertices_len)
    {
        int triangles_num = ((width * height) - (width + height - 1)) * 2;
        int[] triangles = new int[triangles_num * 3];

        for (int i = 0, triangle = 0; i < vertices_len - width; i++)
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

        return triangles;
    }

    public static Vector2[] meshUVs(Vector3[] vertices, int width, int height)
    {
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = vertices.Length - 1; i >= 0; i--)
        {
            uvs[vertices.Length - 1 - i].x = (vertices[i].x + width / 2) / width;
            uvs[vertices.Length - 1 - i].y = (vertices[i].z + height / 2) / height;
        }

        return uvs;
    }

}
