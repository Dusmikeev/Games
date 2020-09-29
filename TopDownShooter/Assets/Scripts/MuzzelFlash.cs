using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MuzzelFlash : MonoBehaviour
{
    public GameObject[] flashes;
    private float flashTime = 0.1f;
    private Random prng;
    private int index;
    void Start()
    {
        prng = new Random();
    }

    public void Activate()
    {
        index = prng.Next(0, flashes.Length); 
        flashes[index].SetActive(true);
        Invoke(nameof(Deactivate), flashTime);
    }
    
    public void Deactivate()
    {
        flashes[index].SetActive(false);
    }
   
}
