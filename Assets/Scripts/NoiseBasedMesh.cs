using UnityEngine;

static public class NoiseBasedMesh
{
    static class MeshGenCache
    {
        static public int width = 0;
        static public int height = 0;
        static public int vertices_len = 0;
        static public int[] triangles;
    }

    static public Mesh GenerateMeshfromNoiseMap(float[,] noiseMap, float meshHight, uint LOD = 0, bool useAvg = false)
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        uint meshIncrement = LOD <= 0 ? 1 : LOD * 2;
        Vector3[] vertices = MeshVertices(noiseMap, meshHight, meshIncrement, useAvg);
        mesh.vertices = vertices;

        int width = noiseMap.GetLength(0) / (int)meshIncrement;
        int height = noiseMap.GetLength(1) / (int)meshIncrement;
        int vertices_len = width * height;

        if (MeshGenCache.width != width || MeshGenCache.height != height || MeshGenCache.vertices_len != vertices_len)
        {
            MeshGenCache.width = width;
            MeshGenCache.height = height;
            MeshGenCache.vertices_len = vertices_len;
            MeshGenCache.triangles = MeshTriangles(width, height, vertices_len);
        }
        mesh.triangles = MeshGenCache.triangles;
        mesh.uv = MeshUVs(mesh.vertices, noiseMap.GetLength(0), noiseMap.GetLength(1));
        mesh.RecalculateNormals();

        return mesh;
    }

    static public Vector3[] MeshVertices(float[,] noiseMap, float meshHight, uint meshIncrement, bool useAvg)
    {
        uint width = (uint)noiseMap.GetLength(0) / meshIncrement;
        uint height = (uint)noiseMap.GetLength(1) / meshIncrement;

        Vector3[] vertices = new Vector3[width * height];

        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < height; x++)
            {
                vertices[y * width + x].x = (x - width / 2) * meshIncrement;
                if (!useAvg) vertices[y * width + x].y = noiseMap[x, y] * meshHight;
                else vertices[y * width + x].y = AverageOfSurroundingCells(noiseMap, x, y) * meshHight;
                vertices[y * width + x].z = (y - height / 2) * meshIncrement;
            }
        }

        return vertices;
    }

    static float AverageOfSurroundingCells(float[,] noiseMap, int x, int y)
    {

        int max_x = noiseMap.GetLength(0);
        int max_y = noiseMap.GetLength(1);

        (int, int)[] d = new (int, int)[]{
            (-1,-1), (0,-1), (1,-1),
            (-1,0),  (0,0),  (1,0),
            (-1,1),  (0,1),  (1,1),
        };

        int count = 0;
        float sum = 0;
        foreach (var (dx, dy) in d)
        {
            int nx = x + dx;
            int ny = y + dy;
            if (nx >= 0 && nx < max_x && ny >= 0 && ny < max_y)
            {
                sum += noiseMap[nx, ny];
                count += 1;
            }
        }

        return sum / count;
    }

    static public int[] MeshTriangles(int width, int height, int vertices_len)
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

    static public Vector2[] MeshUVs(Vector3[] vertices, int width, int height)
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
