using UnityEngine;

public class StickHeroPlatform : MonoBehaviour
{

    [SerializeField] private Transform m_StickPoint;
    public Vector3 GetStickPosition()
    {
        return m_StickPoint.position;
    }

    public float GetPlatformSize()
    {
        return transform.localScale.x;
    }
   
}
