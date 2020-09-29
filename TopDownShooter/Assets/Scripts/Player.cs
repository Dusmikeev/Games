using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float playerSpeed;
    private PlayerController _playerController;
    private Camera _mainCamera;
    private GunController _gunController;

    public CrossHairs crosshairs;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _playerController = GetComponent<PlayerController>();
        _gunController = GetComponent<GunController>();
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber)
    {
        health = startingHealth;
        _gunController.EquipGun(waveNumber-1);
    }

    public override void Die()
    {
        AudioManager.instance.PlaySound("Player Death", Vector3.zero);
        base.Die();
        
    }

    void Update()
    {
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * playerSpeed;
        _playerController.Move(moveVelocity);
        
        //срздание луча из камеры до мышки
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        //создать плейн и определить направление нормалей у него - Vector3.up
        Plane groundPlane = new Plane(Vector3.up, Vector3.up*_gunController.GunHeight);
        float rayDistance;
        
        //утверждение будет верным если луч соприкоснётся с нашим новым плейном
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            //через метод луча узнаем о точке пересечения луча и поля
            Vector3 pointOfIntersection = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, pointOfIntersection,Color.red);
            _playerController.LookAt(pointOfIntersection);
            crosshairs.transform.position = pointOfIntersection;
            crosshairs.DetectTargets(ray);

            if ((new Vector2(pointOfIntersection.x, pointOfIntersection.z) - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude > 20f)
            {
                _gunController.Aim(pointOfIntersection);
            }
            
        }

        if (Input.GetMouseButton(0))
        {
            _gunController.OnTriggerHold();
        }
        if (Input.GetMouseButtonUp(0))
        {
            _gunController.OnTriggerRelease();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            _gunController.Reloading();
        }
        

    }
}
