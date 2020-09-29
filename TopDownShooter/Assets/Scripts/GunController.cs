using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GunController : MonoBehaviour
{
    private Gun equippedGun;
    
    public Transform gunPlacement;
    //переменная для оружия по умолчанию
    public Gun[] guns;

    private void Start()
    {
       
    }

    public void EquipGun(int gunNumber)
    {
        if (equippedGun != null)
        {
            Destroy(equippedGun.gameObject);
        }
        equippedGun = Instantiate(guns[gunNumber], gunPlacement.position, gunPlacement.rotation);
        equippedGun.transform.parent = gunPlacement;
    }
    public void OnTriggerHold()
    {
        if (equippedGun != null)
        {
            equippedGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease()
    {
        if (equippedGun != null)
        {
            equippedGun.OnTriggerRelease();
        }
    }
    
    public void Reloading()
    {
        if (equippedGun != null)
        {
            equippedGun.Reload();
        }
    }

    public float GunHeight => gunPlacement.position.y;
    
    
    public void Aim(Vector3 aimPoint)
    {
        if (equippedGun != null)
        {
            equippedGun.Aim(aimPoint);
        }
    }
}
