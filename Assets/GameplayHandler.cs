using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameplayHandler : MonoBehaviour
{
    public int lives = 100;
    public int money = 500; //potential name for money - monoBits?
    public int round = -1;  //initialize rounds at -1
    public int difficulty = 1;
    public GameObject enemy;
    public List<GameObject> enemies =  new List<GameObject>();
    public Tilemap tiles;
    public LineRenderer pathLineRenderer;
    public int[][] allRoundData; 
    private string path = "";
    private GameObject roundCloser;
    private bool roundCloserSpawned;
    private int level;
    private static bool roundActive;
    //"clock" rotation (0=90deg,1=0deg,2=270deg,3=180deg)
    private static int[] xdir = new int[4]{ 0, 1, 0, -1 }; 
    private static int[] ydir = new int[4]{ 1, 0, -1, 0 };

    void Start()
    {
        pathLineRenderer.startColor = new Color(1, 0, 0, 0.5f);
        pathLineRenderer.endColor = new Color(1, 0, 0, 0.5f);
        pathLineRenderer.startWidth = 0.25f;
        pathLineRenderer.endWidth = 0.25f;
        level = GetComponent<MapGenerator>().level;
        //grab startpointdata from leveldata script
    }

    void Update()
    {
        if (roundActive)
        {
            if (roundCloserSpawned && roundCloser == null) //TODO: make round end on last enemy death, not round closer death
            {
                roundCloserSpawned = false;
                endRound();
            }
            updateEnemiesOrder();
        }
    }

    public void startRound()
    {
        if (roundActive)
        {
            //TODO: add an error sound
        } 
        else
        {
            round++;
            updateShortestPath();
            updateLine();
            roundActive = true;
            spawnRoundEnemies();
        }
    }

    public void endRound()
    {
        if (roundActive)
        {
            roundActive = false;
        }
    }

    public bool updateShortestPath() //bfs algorithm
    {
        int[,] tileData = GetComponent<MapHandler>().tileData;
        bool[,] visited = new bool[tileData.GetLength(0),tileData.GetLength(1)];
        Queue<int> xqueue = new Queue<int>();
        Queue<int> yqueue = new Queue<int>();
        Queue<string> possiblePaths = new Queue<string>();
        possiblePaths.Enqueue("");
        if (GetComponent<MapGenerator>().level < 0)
        {
            xqueue.Enqueue(0);
            yqueue.Enqueue(GetComponent<MapGenerator>().mapSizeY/2);
        } 
        else
        {
            int[] startPoints = GetComponent<LevelData>().customStartPoints[level];
            for (int i = 0; i < startPoints.Length; i+=2) //TODO: implement ability to do multiple starting points
            {
                xqueue.Enqueue(startPoints[i]);
                yqueue.Enqueue(startPoints[i + 1]);
            }
        }
        while (xqueue.Count > 0 && yqueue.Count > 0)
        {
            int x = xqueue.Dequeue();
            int y = yqueue.Dequeue();
            int maxX = tileData.GetLength(1);
            int maxY = tileData.GetLength(0);
            path = possiblePaths.Dequeue();
            if (tileData[y, x] == -2)
            {
                return true;
            }
            for (int i = 0; i < 4; i++)
            {
                int xc = x + xdir[i];
                int yc = y + ydir[i];
                if (xc < 0 || yc < 0 || xc >= maxX || yc >= maxY || visited[yc,xc] || tileData[yc,xc] > 0)
                {
                    continue;
                }
                xqueue.Enqueue(xc);
                yqueue.Enqueue(yc);
                possiblePaths.Enqueue(path + i);
                visited[yc, xc] = true;
            }
        }
        return false;
    }
    private void updateLine()
    {
        float xpos;
        float ypos;
        //TODO: implement ability to do multiple start and end points
        if (level >= 0)
        {
            xpos = GetComponent<LevelData>().customStartPoints[level][0] + 0.5f;
            ypos = GetComponent<LevelData>().customStartPoints[level][1] + 0.5f;
        } 
        else
        {
            xpos = 0.5f;
            ypos = (GetComponent<MapGenerator>().mapSizeY / 2) + 0.5f;
        }
        List<Vector3> pathLinePoints = new List<Vector3>();
        int next;
        int prev = -1;
        for (int i = 0; i < path.Length; i++)
        {
            next = path[i] - '0';
            if (next != prev) 
            {
                pathLinePoints.Add(new Vector3(xpos, ypos, -0.5f));
                prev = next;
            }
            xpos += xdir[next];
            ypos += ydir[next];
        }
        pathLinePoints.Add(new Vector3(xpos, ypos, -0.5f));
        pathLineRenderer.positionCount = pathLinePoints.Count;
        pathLineRenderer.SetPositions(pathLinePoints.ToArray());
    }

    private void spawnRoundEnemies()
    {
        StartCoroutine(spawnEnemies());
    }
    private IEnumerator spawnEnemies()
    {
        GameObject enemyInstance = null;
        int[] enemyData = GetComponent<LevelData>().enemyData[round];
        int level = GetComponent<MapGenerator>().level;
        for (int i = 0; i < enemyData.Length; i++)
        {
            float spawnInterval = (float)enemyData[i+1]/(float)enemyData[i];
            int cycles = enemyData[i+2];
            int cycleLength = enemyData[i+3];
            i += 4;
            for (int j = 0; j < cycles; j++)
            {
                for (int k = 0; k < cycleLength; k++)
                {
                    enemyInstance = Instantiate(enemy);
                    Vector3 startPos;
                    if (level < 0)
                    {
                        startPos = tiles.GetCellCenterWorld(new Vector3Int(0, GetComponent<MapGenerator>().mapSizeY / 2, 0));
                    }
                    else //TODO: implement spawning enemies in multiple custom start points (maybe switch to an array of start values)
                    {
                        int[] startPoints = GetComponent<LevelData>().customStartPoints[level];
                        startPos = tiles.GetCellCenterWorld(new Vector3Int(startPoints[0], startPoints[1], 0));
                    }
                    enemyInstance.GetComponent<Transform>().position = new Vector3(startPos.x, startPos.y, -1);
                    enemyInstance.GetComponent<EnemyPathing>().setDNA(enemyData[i + k]);
                    enemyInstance.GetComponent<EnemyPathing>().path = path;
                    enemies.Add(enemyInstance);
                    yield return new WaitForSeconds(spawnInterval);
                }
            }
            i += cycleLength-1;
        }
        roundCloser = enemyInstance; //Perhaps use different way to end round than round closer method
        roundCloserSpawned = true;
    }
    private void updateEnemiesOrder()
    {
        for (int i = 0; i < enemies.Count-1; i++)
        {
            if (enemies[i].GetComponent<EnemyPathing>().distanceTraveled < enemies[i + 1].GetComponent<EnemyPathing>().distanceTraveled)
            {
                GameObject holder = enemies[i];
                enemies[i] = enemies[i + 1];
                enemies[i + 1] = holder;
            }
        }
    }
    public bool getActive()
    {
        return roundActive;
    }


}
