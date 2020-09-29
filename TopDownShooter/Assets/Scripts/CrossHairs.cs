using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairs : MonoBehaviour
{
    public LayerMask targetMask;
    
    public float rotationSpeed;
    public SpriteRenderer dot;
    private SpriteRenderer circle;
    private Color originalColor;
    public Color highlightColor;

    private void Start()
    {
        Cursor.visible = false;
        
        circle = GetComponent<SpriteRenderer>();
        originalColor = dot.color;
        circle.color = originalColor;
    }

    void Update()
    {
        transform.Rotate(Vector3.forward*Time.deltaTime*rotationSpeed);
    }

    public void DetectTargets(Ray ray)
    {
        if (Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = highlightColor;
            circle.color = highlightColor;

        }

        else
        {
            dot.color = originalColor;
            circle.color = originalColor;
        }
    }
    
}
