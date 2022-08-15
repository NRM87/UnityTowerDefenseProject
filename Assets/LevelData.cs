using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    private bool dataLoaded = false;
    public int[][] enemyData; //number of each enemy for each round
    public int[][] customStartPoints;
    public int[][] customEndPoints;
    public int[][,] customLevels;

    void Start()
    {
        enemyData = new int[5][];
        // enemyData[round] is organized in segments. First (S) and second (T) elements are spawns per seconds. 
        // Third (C) element is the amount of times/cycles the spawns are repeated. Fourth (N) element is amount of spawns per 
        // spawn cycle. Next N elements (E) contain the values corresponding to different enemies (ex: '0' will spawn EnemyO).
        // [S,T,C,N,E_1,E_2,E_3,...,E_N, repeat]

        enemyData[0] = new int[] { 1, 5, 5, 1, 0, 1, 5, 10, 1, 1 };
        enemyData[1] = new int[] { 1, 5, 5, 1, 0, 1, 5, 10, 1, 1 };
        enemyData[2] = new int[] { 1, 5, 5, 1, 0, 2, 1, 10, 1, 1 };
        enemyData[3] = new int[] { 1, 5, 5, 1, 0, 2, 1, 10, 1, 1 };
        enemyData[4] = new int[] { 1, 5, 5, 1, 0, 2, 1, 10, 1, 1 };


        customStartPoints = new int[1][];
        customEndPoints = new int[1][];
        customLevels = new int[1][,]; //maps are upside-down compared to array in code.

        //coordinate pairs (x,y) is listed as x1,y1,x2,y2...
        customStartPoints[0] = new int[] {0,10 };
        customEndPoints[0] = new int[] {24,10 };

        customLevels[0] = new int[20,25] 
        { {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
          {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
          {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
          {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
          {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
          {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
          {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
          {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
          {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
          {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
          {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
          {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
          {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
          {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
          {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
          {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
          {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
          {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
          {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
          {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }, };
        dataLoaded = true;
    }

    public bool getDataLoaded()
    {
        return dataLoaded;
    }
}
