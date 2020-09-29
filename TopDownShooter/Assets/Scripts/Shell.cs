using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shell : MonoBehaviour
{
    private float shellLifeTime= 10f;
    private float shellFadeSpeed = 2f;

    public float forceMin;
    public float forceMax;
    
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        float force = Random.Range(forceMin, forceMax);
        rb.AddForce(transform.right*force);
        rb.AddTorque(Random.insideUnitSphere * force);

        StartCoroutine(ShellFade());
    }


    IEnumerator ShellFade()
    {
        yield return new WaitForSeconds(shellLifeTime);
        
        Material shellMat = gameObject.GetComponent<Renderer>().material;
        Color originalShellColor = shellMat.color;
        
        float percent = 0;
        
        while (percent<1)
        {
            percent += Time.deltaTime * shellFadeSpeed;
            shellMat.color = Color.Lerp(originalShellColor, Color.clear, percent);
            yield return null;
        }
        
        Destroy(gameObject);
        
    }
}
