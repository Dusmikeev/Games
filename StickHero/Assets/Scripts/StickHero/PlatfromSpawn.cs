using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlatfromSpawn : MonoBehaviour
{

    [SerializeField] private float minPlatformDistance = 2.5f;
    [SerializeField] private float maxPlatfromDistance = 4f;

    [SerializeField] private float minPlatfromScale = 0.1f;
    [SerializeField] private float maxPlatfromScale = 0.6f;
    

    public GameObject platformPrefab;


    public void CreatePlatform()
    {
        GameObject newPlatform;
        newPlatform = platformPrefab;

        newPlatform.transform.localScale = new Vector3(Random.Range(minPlatfromScale, maxPlatfromScale),transform.localScale.y, transform.localScale.z);
       
        Vector3 newPosition = new Vector3(transform.position.x + Random.Range(minPlatformDistance,maxPlatfromDistance), transform.position.y, transform.position.z);
        transform.position = newPosition;

        Instantiate(newPlatform, transform.position, Quaternion.identity);
        

    }

    
    
}
