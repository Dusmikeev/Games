using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class MapGenerator : MonoBehaviour
{
    public Map[] maps;
    public int mapIndex;
    
    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform navmeshFloor;
    public Transform mapFloor;
    public Transform navmeshMaskPrefab;
    

    public Vector2 maxMapSize;

    [Range(0,1)]
    public float outlinePercent;
    

    public float tileScale;
    
    private List<Coord> allTileCoords;
    private Queue<Coord> shuffledTileCoords;
    private Queue<Coord> shuffledOpenTileCoords;

    private Transform[,] tileMap;
    
    private Map currentMap;

    private Spawner spawner;
    
    void Start()
    {
        GenerateMap();
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber)
    {
        spawner.ResetPlayerPosition();
        mapIndex = waveNumber - 1;
        GenerateMap();
    }
    
    public void GenerateMap()
    {
        currentMap = maps[mapIndex];
        tileMap = new Transform[currentMap.mapSize.x,currentMap.mapSize.y];
        Random prng = new Random(currentMap.seed);

        //Generating coords        
        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
               allTileCoords.Add(new Coord(x,y));
            }
        }
        
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(),currentMap.seed));
        //Create map holder object
        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }
        
        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;
        
        //Spawning tiles
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
               
                //Создавая переменную при создании объекта к ней можно будет легко обращаться в будущем
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(90, 0, 0));
                
                newTile.localScale = Vector3.one * (1 - outlinePercent)*tileScale;
                newTile.parent = mapHolder;
                tileMap[x, y] = newTile;
            }
        }
        
        //Spawning obstacles    
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x,(int)currentMap.mapSize.y];

        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;
        List<Coord> allOpenCoords = new List<Coord>(allTileCoords);
        
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;
            
            if (randomCoord != currentMap.mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(currentMap.minimumObstacleHeight, currentMap.maximumObstacleHeight, (float)prng.NextDouble());
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
            
                Transform newObstacle = Instantiate(obstaclePrefab, new Vector3(obstaclePosition.x, obstacleHeight/2f, obstaclePosition.z), Quaternion.identity);
                newObstacle.parent = mapHolder;
                newObstacle.localScale = new Vector3((1 - outlinePercent)*tileScale, obstacleHeight, (1 - outlinePercent)*tileScale);

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMAterial = new Material(obstacleRenderer.sharedMaterial);
                float colorPercent = randomCoord.y / (float) currentMap.mapSize.y;
                obstacleMAterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMAterial;

                allOpenCoords.Remove(randomCoord);

            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }
        
        shuffledOpenTileCoords = new Queue<Coord> (Utility.ShuffleArray (allOpenCoords.ToArray (), currentMap.seed));
        
        //Creating navmesh masks
        Transform maskLeft = Instantiate(navmeshMaskPrefab,Vector3.left*(currentMap.mapSize.x + maxMapSize.x)/4f *tileScale, Quaternion.identity);
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxMapSize.x-currentMap.mapSize.x)/2f,1, currentMap.mapSize.y)*tileScale;
        
        Transform maskRight = Instantiate(navmeshMaskPrefab,Vector3.right*(currentMap.mapSize.x + maxMapSize.x)/4f *tileScale, Quaternion.identity);
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxMapSize.x-currentMap.mapSize.x)/2f,1, currentMap.mapSize.y)*tileScale;
        
        Transform maskTop = Instantiate(navmeshMaskPrefab,Vector3.forward*(currentMap.mapSize.y + maxMapSize.y)/4f *tileScale, Quaternion.identity);
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxMapSize.x,1, (maxMapSize.y - currentMap.mapSize.y)/2f)*tileScale;
        
        Transform maskBottom = Instantiate(navmeshMaskPrefab,Vector3.back*(currentMap.mapSize.y + maxMapSize.y)/4f *tileScale, Quaternion.identity);
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(maxMapSize.x,1, (maxMapSize.y - currentMap.mapSize.y)/2f)*tileScale;
        
        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y)*tileScale;
        mapFloor.localScale = new Vector3(currentMap.mapSize.x*tileScale, currentMap.mapSize.y*tileScale);
        
    }

    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0),obstacleMap.GetLength(1)];
        Queue<Coord>queue = new Queue<Coord>();
        queue.Enqueue(currentMap.mapCenter);
        mapFlags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;

        int accessibleTileCount=1;
        
        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            for (int x = -1; x <=1; x++)
            {
                for (int y = -1; y <=1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x == 0 || y == 0)
                    {
                        if(neighbourX>=0 && neighbourX<obstacleMap.GetLength(0) && neighbourY>=0 && neighbourY<obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX,neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }

        }

        int targetAcessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
        return targetAcessibleTileCount == accessibleTileCount;
    }
    
    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-currentMap.mapSize.x/2f+0.5f+x,0,-currentMap.mapSize.y/2f+0.5f+y)*tileScale;
    }


    public Transform GetTileFromPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / tileScale + (currentMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / tileScale + (currentMap.mapSize.y - 1) / 2f);
        x = Mathf.Clamp(x, 0, tileMap.GetLength(0)-1);
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1)-1);
        
        return tileMap[x, y];
    }
    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }


    public Transform GetRandomOpenTile()
    {
        Coord randomCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(randomCoord);
        return tileMap[randomCoord.x, randomCoord.y];
    }
    
    [Serializable]
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }
        
        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
    }
    
    [Serializable]
    public class Map
    {
        public Coord mapSize;
       [Range(0,1)]
        public float obstaclePercent;
        public int seed;
        public float minimumObstacleHeight;
        public float maximumObstacleHeight;
        public Color foregroundColor;
        public Color backgroundColor;

        public Coord mapCenter
        {
            get
            {
                return new Coord(mapSize.x/2, mapSize.y/2);
            }
        }

    }
}
