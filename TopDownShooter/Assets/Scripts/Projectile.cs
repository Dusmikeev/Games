using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;
    private float _bulletSpeed;

    public Color trailColor; 
    
    public float damage;
    private float bulletLifetime = 1.5f;
    private float skinWidth = 0.1f;
    private void Start()
    {
        Destroy(gameObject,bulletLifetime);
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        if (initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0], transform.position, transform.forward);
        }
    }

    public void SetSpeed(float newSpeed)
    {
        _bulletSpeed = newSpeed;
    }
    void Update()
    {
        float moveDistance = _bulletSpeed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward*(moveDistance));
    }

    private void CheckCollisions(float moveDistance)
    {
       Ray ray = new Ray(transform.position, transform.forward);
       RaycastHit hit;
       if (Physics.Raycast(ray, out hit, moveDistance+skinWidth, collisionMask, QueryTriggerInteraction.Collide))
       {
          OnHitObject(hit.collider, hit.point, transform.forward);
       }

    }


    void OnHitObject(Collider c, Vector3 hitPoint, Vector3 hitDirection)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
        }
        Destroy(gameObject);
    }
 
}
