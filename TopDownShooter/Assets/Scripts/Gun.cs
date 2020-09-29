using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gun : MonoBehaviour
{
   public enum FireMode {Auto, Burst, Single}
   public FireMode fireMode;
  
   //starting position of projectiles
   public Transform[] projectileSpawn;
   //prefab of something to shoot;
   public Projectile projectile;
   public int burstCount;
   private bool triggerReleasedSinceLastShot;
   private int shotsRemainingInBurst;
   private bool isReloading;

   public int projectilesPerMag;
   private int projectilesRemaining;
   
   [Header("Effects")]
   public Shell shell;
   public Transform shellEjectionPoint;
   private MuzzelFlash muzzelFlash;
   public AudioClip shootAudio;
   public AudioClip reloadAudio;

  [Header("Recoil")]
   private Vector3 recoverVelocity;
   private float currentVelocity;
   private float smoothTime = 0.1f;
   private float recoilAngle;
   public Vector2 recoilAngleMinMax;
   public Vector2 kickMinMax;
   

   private void Start()
   {
      muzzelFlash = GetComponent<MuzzelFlash>();
      shotsRemainingInBurst = burstCount;
      projectilesRemaining = projectilesPerMag;
   }

   private void LateUpdate()
   {
      //animate recoil
      transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoverVelocity, smoothTime );
      recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref currentVelocity, smoothTime);
      transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;
      
   }

   public float msBetweenShots;
   public float muzzelVelocity;

   private float _nextShotTime;

   void Shoot()
   {
      if (!isReloading && Time.time>_nextShotTime && projectilesRemaining>0)
      {
         if (fireMode == FireMode.Burst)
         {
            if (shotsRemainingInBurst == 0)
            {
               return;
            }
            shotsRemainingInBurst--;
         }
         else if (fireMode == FireMode.Single)
         {
            if (!triggerReleasedSinceLastShot)
            {
               return;
            }
         }

         for (int i = 0; i < projectileSpawn.Length; i++)
         {
            if (projectilesRemaining == 0)
            {
               break;
            }
            projectilesRemaining--;
            _nextShotTime = Time.time + msBetweenShots;
            Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].transform.position, projectileSpawn[i].transform.rotation);
            newProjectile.SetSpeed(muzzelVelocity);
         }
         Instantiate(shell, shellEjectionPoint.transform.position, shellEjectionPoint.transform.rotation);
         muzzelFlash.Activate();
         transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
         AudioManager.instance.PlaySound(shootAudio, transform.position);
      }

      if (projectilesRemaining == 0)
      {
         Reload();
      }
   }

   public void Aim(Vector3 aimPoint)
   {
      if (!isReloading)
      {
         
         transform.LookAt(aimPoint);
      }
   }
   
   public void OnTriggerHold()
   {
      Shoot();
      triggerReleasedSinceLastShot = false;
   }

   public void OnTriggerRelease()
   {
      triggerReleasedSinceLastShot = true;
      shotsRemainingInBurst = burstCount;
   }

   public void Reload()
   {
      if (!isReloading && projectilesRemaining != projectilesPerMag)
      {
         StartCoroutine(Reloading());
         AudioManager.instance.PlaySound(reloadAudio, transform.position);
      }
   }

   IEnumerator Reloading()
   {
      isReloading = true;
      yield return new WaitForSeconds(.2f);
      
      float reloadSpeed = 3f;
      float percent = 0;
      Vector3 initialRot = transform.localEulerAngles;
     
      while (percent < 1)
      {
         percent += Time.deltaTime * reloadSpeed;
         float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
         float reloadAngle = Mathf.Lerp(0, recoilAngleMinMax.y, interpolation);
         transform.localEulerAngles = initialRot + Vector3.left*reloadAngle;

         yield return null;
      }
      
      projectilesRemaining = projectilesPerMag;
      isReloading = false;
   }
   
}
