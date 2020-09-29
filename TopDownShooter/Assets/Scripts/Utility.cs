using System;
using System.Collections;
using System.Collections.Generic;


public class Utility{

    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        Random randomGenerator = new Random(seed);

        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = randomGenerator.Next(i, array.Length);
            T tempItem;
            tempItem = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = tempItem;
        }

        return array;
    }
    
}
