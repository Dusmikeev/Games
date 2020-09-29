using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    //переменная этого класса
    private Vector3 _velocity;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + _velocity*Time.fixedDeltaTime);
        
    }

    //метод с помощью которого мы передаём информацию из скрипта Player
    public void Move(Vector3 velocity)
    {
        _velocity = velocity;
    }

    public void LookAt(Vector3 point)
    {
        Vector3 liftedPoint = new Vector3(point.x, transform.position.y, point.z);
        transform.LookAt(liftedPoint);
    }
}
