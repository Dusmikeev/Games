using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickHeroStick : MonoBehaviour
{
    [SerializeField] private StickHeroController m_Controller;
    private bool isScale;
    private bool isRotate;
    private float angleZ;
	
    public void ResetStick(Vector3 newPosition)
    {
        transform.position = newPosition;
        var scale = new Vector3(1f, 0f, 1f);
        transform.localScale = scale;

        transform.localEulerAngles = Vector3.zero;
    }

    public void StartScaling()
    {
        isScale = true;
    }
	
    public void StartRotate()
    {
        isRotate = true;
    }

    public void StopScaling()
    {
        isScale = false;
        angleZ = 0f;
        m_Controller.StopStickScale();
    }

    private void StopRotate()
    {
        isRotate = false;
        m_Controller.StopStickRotate();
        transform.localEulerAngles = new Vector3(0f, 0f, -90f);
		
        m_Controller.StartPlayerMovement(transform.localScale.y);
    }


    private void Update () 
    {
        if (isScale)
        {
            float scaleY = transform.localScale.y;
            if (scaleY >= 2f)
            {
                StopScaling();
                return;
            }

            scaleY += Time.deltaTime * 0.5f;
            transform.localScale = new Vector3(transform.localScale.x, scaleY, transform.localScale.z);
        }

        if (!isRotate) return;

        if (angleZ <= -90f)
        {
            StopRotate();
            return;
        }

        angleZ -= Time.deltaTime * 90f;
        transform.localEulerAngles = new Vector3(0f, 0f, angleZ);
    }
}
