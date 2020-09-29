using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum EnemyState
    {
        Attacking,
        Chasing,
        Idle,
    }
    private EnemyState currentState;

    public ParticleSystem deathEffect;
    
    private float myCollisonRadius;
    private float targetCollisionRadius;

    private OnDeathParticles particles;
    public static event Action OnDeathStatic;

    public float enemyDamage;

    private NavMeshAgent _agent;
    private Player target;
    private Material skinMaterial;

    private Color originalColor;
    
    private float attackDistanceThreshold = .5f;
    private float timeBetweenAttacks = 1;
    private float nextAttackTime;

    private bool hasTarget;


    private void Awake()
    {
        currentState = EnemyState.Idle;
        _agent = GetComponent<NavMeshAgent>();
        target = FindObjectOfType<Player>();
        particles = FindObjectOfType<OnDeathParticles>();
        if (target != null)
        {
            hasTarget = true;
            myCollisonRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }
        

    }

    protected override void Start()
    {
        base.Start();
        currentState = EnemyState.Idle;
        if (hasTarget)
        {
            currentState = EnemyState.Chasing;
            target.OnDeath += OnTargetDeath;
        }
        
        StartCoroutine("UpdatePath");
    }

    private void Update()
    {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDistanceToTarget = (target.transform.position - transform.position).sqrMagnitude;

                if (sqrDistanceToTarget <
                    Math.Pow(attackDistanceThreshold + myCollisonRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    AudioManager.instance.PlaySound("Enemy Attack", transform.position);
                    StartCoroutine(Attack());
                }
            }
        }
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        AudioManager.instance.PlaySound("Impact", transform.position);
        if (damage >= health)
        {
            OnDeathStatic?.Invoke();
            AudioManager.instance.PlaySound("Enemy Death", transform.position);
            Destroy(Instantiate(deathEffect, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)), 2f);
        }
        base.TakeDamage(damage);
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = EnemyState.Idle;
    }
    IEnumerator Attack()
    {
        currentState = EnemyState.Attacking;
        _agent.enabled = false;
        
        Vector3 currentPosition = transform.position;
        Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
        Vector3 attackPosition = target.transform.position - dirToTarget*(myCollisonRadius);

        float attackSpeed = 3;
        float percent = 0;
        
        skinMaterial.color = Color.red;;
        bool hasAppliedDamage = false;

        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                target.TakeDamage(enemyDamage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(currentPosition, attackPosition, interpolation);
           
            yield return null;
        }
        skinMaterial.color = originalColor;
        currentState = EnemyState.Chasing;
        _agent.enabled = true;
    }

    public void SetCharacteristics(float enemyMoveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor)
    {
        _agent.speed = enemyMoveSpeed;
        if (hasTarget)
        {
            enemyDamage = Mathf.Ceil(target.startingHealth / hitsToKillPlayer);
        }

        startingHealth = enemyHealth;
        
        deathEffect.startColor = new Color(skinColor.r, skinColor.g,skinColor.b);
        skinMaterial = GetComponent<Renderer>().material;
        skinMaterial.color = skinColor;
        originalColor = skinMaterial.color;

    }
    IEnumerator UpdatePath()
    {

        while (hasTarget)
        {
            if (currentState == EnemyState.Chasing)
            {
                Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
                Vector3 targetPosition = target.transform.position - directionToTarget*(myCollisonRadius+targetCollisionRadius+attackDistanceThreshold/2);
                if (!dead)
                {
                    _agent.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    
}
