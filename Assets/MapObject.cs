using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    public static int maxBlockSize = 4;
    public static float blockSpawnChance = 0.1f;
    public int mapSizeX = 25;
    public int mapSizeY = 20;
    public bool[,] wallArray;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void generateRandomWalls()
    {
        wallArray = new bool[mapSizeY, mapSizeX];
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < mapSizeX; j++)
            {
                wallArray[i, j] = true;
            }
        }
        for (int i = mapSizeY - 2; i < mapSizeY; i++)
        {
            for (int j = 0; j < mapSizeX; j++)
            {
                wallArray[i, j] = true;
            }
        }
        for (int i = 2; i < mapSizeY - 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                wallArray[i, j] = true;
            }
        }
        for (int i = 2; i < mapSizeY - 2; i++)
        {
            for (int j = mapSizeX - 2; j < mapSizeX; j++)
            {
                wallArray[i, j] = true;
            }
        }
        wallArray[(mapSizeY / 2), 0] = false;
        wallArray[(mapSizeY / 2), 1] = false;
        wallArray[(mapSizeY / 2), mapSizeX - 1] = false;
        wallArray[(mapSizeY / 2), mapSizeX - 2] = false;

        for (int i = 3; i < mapSizeY - 3; i++)
        {
            for (int j = 3; j < mapSizeX - 3; j++)
            {
                if (Random.Range(0f, 1.0f) < blockSpawnChance && !wallOnT(i, j) && !wallOnB(i, j) && !wallOnB(i, j + 1) && !wallOnB(i, j - 1) && !wallOnL(i, j) && !wallOnR(i, j))
                {
                    int maxLength = 0;
                    int maxHeight = 0;
                    while (maxLength < maxBlockSize && !wallArray[i, j + maxLength + 1] && !wallArray[i - 1, j + maxLength + 1])
                    {
                        maxLength++;
                    }
                    while (maxHeight < maxBlockSize && !wallArray[i + maxHeight + 1, j])
                    {
                        maxHeight++;
                    }
                    generateBlock(j, i, maxLength, maxHeight);
                }
            }
        }
    }
    void generateBlock(int x, int y, int maxL, int maxH)
    {
        int length = (int)Random.Range(1, maxL + 1);
        int height = (int)Random.Range(1, maxH + 1);
        for (int i = y; i < y + height; i++)
        {
            for (int j = x; j < x + length; j++)
            {
                wallArray[i, j] = true;
            }
        }
    }
    bool wallOnL(int i, int j)
    {
        return (j > 0) && (wallArray[i, j - 1]);
    }
    bool wallOnR(int i, int j)
    {
        return (j < mapSizeX - 1) && (wallArray[i, j + 1]);
    }
    bool wallOnT(int i, int j)
    {
        return (i < mapSizeY - 1) && (wallArray[i + 1, j]);
    }
    bool wallOnB(int i, int j)
    {
        return (i > 0) && (wallArray[i - 1, j]);
    }
}
