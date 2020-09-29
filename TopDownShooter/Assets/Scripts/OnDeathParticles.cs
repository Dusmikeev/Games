using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDeathParticles : MonoBehaviour
{
    public Material particleColor;

    public void ChangeColor()
    {
        particleColor.color = FindObjectOfType<Spawner>().currentWave.skinColor;
    }

   
}
