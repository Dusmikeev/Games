using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Enemy enemy;
    private int enemiesRemaining;
    private int enemiesRemainingAlive;
    private float nextSpawnTime;
    private int currentWaveNumber;

    [NonSerialized]
    public Wave currentWave;

    private LivingEntity playerEntity;
    private Transform playerT;
    
    private MapGenerator map;

    private float timeBetweenCampingChecks = 2;
    private float nextCampCheckTime;
    private Vector3 campPositionOld;
    private float campThresholdDistance = 1.5f;
    private bool isCamping;

    private bool isDisabled;

    public event Action <int> OnNewWave;
    
    public Wave[] waves;
    [System.Serializable] 
    public class Wave
    {
        public bool infinite;
        
        public int enemyCount;
        public float timeBetweenSpawns;
       
        public float enemyMoveSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public Color skinColor;
        
    }

    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = Time.time + timeBetweenCampingChecks;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;
        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    private void Update()
    {
        if (!isDisabled)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;
                isCamping = Vector3.Distance(campPositionOld, playerT.position) < campThresholdDistance;
                campPositionOld = playerT.position;
            }

            if ((enemiesRemaining > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
            {
                enemiesRemaining--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
                StartCoroutine(SpawnEnemy());
            }
        }
    }

    IEnumerator SpawnEnemy()
    {

        float spawnDelay = 1;
        float tileFlashSpeed = 4;

        Transform spawnTile = map.GetRandomOpenTile();
        if (isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerT.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        
        Color originalColor = Color.white;
        Color flashColor = Color.red;
        float spawnTimer = 0;
        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(originalColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
            
            spawnTimer += Time.deltaTime;
            yield return null;
        }
        
        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position+Vector3.up, Quaternion.identity);
        spawnedEnemy.OnDeath += OnEnemyDeath;
        spawnedEnemy.SetCharacteristics(currentWave.enemyMoveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);

    }

    
    
    void OnPlayerDeath()
    {
        isDisabled = true;
    }
    void OnEnemyDeath()
    {
        enemiesRemainingAlive--;
        if (enemiesRemainingAlive <= 0 && currentWaveNumber<waves.Length)
        {
            NextWave();
        }
    }

    public void ResetPlayerPosition()
    {
        playerT.position = Vector3.zero+ Vector3.up*3;
    }
    void NextWave()
    {
        if (currentWaveNumber>0)
        {
            AudioManager.instance.PlaySound2d("Level Complete");
        }
        currentWaveNumber++;
        currentWave = waves[currentWaveNumber - 1];

        enemiesRemaining = currentWave.enemyCount;
        enemiesRemainingAlive = enemiesRemaining;

        OnNewWave?.Invoke(currentWaveNumber);
    }
}
