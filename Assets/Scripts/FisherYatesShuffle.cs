using System;

static public class FisherYatesShuffle
{
    static Random rnd = new Random();

    static public int[] Shuffle(int[] array)
    {
        int length = array.Length;

        for (int i = 0; i < array.Length - 1; i++)
        {
            int j = rnd.Next(length);
            var temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
        return array;
    }

}
