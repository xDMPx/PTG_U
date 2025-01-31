using UnityEngine;
using System.Linq;

static public class PerlinNoiseGenerator
{
    static int[] p = KenPerlinPermutationTable();

    //https://mrl.cs.nyu.edu/~perlin/noise/
    static int[] KenPerlinPermutationTable()
    {
        int[] permutationTable = new int[] { 151,160,137,91,90,15,
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
            190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
            88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
            77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
            102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
            223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
            129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
            251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
            49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
            };

        permutationTable = permutationTable.Concat<int>(permutationTable).ToArray<int>();

        return permutationTable;
    }

    static int[] RandomPermutationTable()
    {
        int[] permutationTable = new int[] { 151,160,137,91,90,15,
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
            190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
            88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
            77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
            102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
            223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
            129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
            251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
            49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
            };

        FisherYatesShuffle.Shuffle(permutationTable);

        permutationTable = permutationTable.Concat<int>(permutationTable).ToArray<int>();

        return permutationTable;
    }

    //https://mrl.cs.nyu.edu/~perlin/noise/
    // <-1,1>
    static public float PerlinNoise(float x, float y, float z = 1)
    {
        int X = (int)Mathf.Floor(x) & 255,                                  // FIND UNIT CUBE THAT
            Y = (int)Mathf.Floor(y) & 255,                                  // CONTAINS POINT.
            Z = (int)Mathf.Floor(z) & 255;
        x -= Mathf.Floor(x);                                                // FIND RELATIVE X,Y,Z
        y -= Mathf.Floor(y);                                                // OF POINT IN CUBE.
        z -= Mathf.Floor(z);
        float u = Fade(x),                                                  // COMPUTE FADE CURVES
              v = Fade(y),                                                  // FOR EACH OF X,Y,Z.
              w = Fade(z);
        int A = p[X] + Y, AA = p[A] + Z, AB = p[A + 1] + Z,                 // HASH COORDINATES OF
            B = p[X + 1] + Y, BA = p[B] + Z, BB = p[B + 1] + Z;             // THE 8 CUBE CORNERS,

        return Lerp(w, Lerp(v, Lerp(u, Grad(p[AA], x, y, z),                // AND ADD
                                       Grad(p[BA], x - 1, y, z)),           // BLENDED
                               Lerp(u, Grad(p[AB], x, y - 1, z),            // RESULTS
                                       Grad(p[BB], x - 1, y - 1, z))),      // FROM  8
                       Lerp(v, Lerp(u, Grad(p[AA + 1], x, y, z - 1),        // CORNERS
                                       Grad(p[BA + 1], x - 1, y, z - 1)),   // OF CUBE
                               Lerp(u, Grad(p[AB + 1], x, y - 1, z - 1),
                                       Grad(p[BB + 1], x - 1, y - 1, z - 1))));
    }

    //https://mrl.cs.nyu.edu/~perlin/noise/
    static float Fade(float t) { return t * t * t * (t * (t * 6 - 15) + 10); }

    //https://mrl.cs.nyu.edu/~perlin/noise/
    static float Lerp(float t, float a, float b) { return a + t * (b - a); }

    //https://mrl.cs.nyu.edu/~perlin/noise/
    static float Grad(int hash, float x, float y, float z)
    {
        int h = hash & 15;                                      // CONVERT LO 4 BITS OF HASH CODE
        float u = h < 8 ? x : y,                                // INTO 12 GRADIENT DIRECTIONS.
               v = h < 4 ? y : h == 12 || h == 14 ? x : z;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }

    static public float NormalizedPerlinNoise(float x, float y, float z = 1)
    {
        return Mathf.InverseLerp(-1, 1, PerlinNoise(x, y, z));
    }

    static public void RandomizePermutationTable()
    {
        p = RandomPermutationTable();
    }

    static public void RestoreKenPerlinPermutationTable()
    {
        p = KenPerlinPermutationTable();
    }

}
