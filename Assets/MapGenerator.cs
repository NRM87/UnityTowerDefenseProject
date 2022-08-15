using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public int level = -1;
    public int mapSizeX = 25;
    public int mapSizeY = 20;
    public int maxBlockSize = 4;
    public float blockSpawnChance = 0.1f;
    public Tilemap tiles;
    public TileBase wallTR , wallTL;
    public TileBase wallBL;
    public TileBase wallBR;
    public TileBase wallT;
    public TileBase wallL;
    public TileBase wallB;
    public TileBase wallR;
    public TileBase wallV;
    public TileBase wallH;
    public TileBase wallM;
    public TileBase wallCTR;
    public TileBase wallCTL;
    public TileBase wallCBL;
    public TileBase wallCBR;
    public TileBase wall3T;
    public TileBase wall3L;
    public TileBase wall3B;
    public TileBase wall3R;
    public TileBase wall4;
    public TileBase background;
    public bool[,] wallArray;
    private bool mapCompleted = false;

    void Start()
    {
        clearWalls();
        if (level >= 0)
        {
            StartCoroutine(initializeLevelWalls());
        }
        else
        {
            generateRandomWalls();
            fillTextures();
        }
    }

    private IEnumerator initializeLevelWalls()
    {
        yield return new WaitUntil(() => GetComponent<LevelData>().getDataLoaded());
        generateLevelWalls(level);
        fillTextures();
    }

    void clearWalls()
    {
        if (wallArray != null)
        {
            for (int i = 0; i < wallArray.GetLength(0); i++)
            {
                for (int j = 0; j < wallArray.GetLength(1); j++)
                {
                    wallArray[i, j] = false;
                }
            }
        }
    }

    void generateLevelWalls(int level)
    {
        int[,] levelWalls = GetComponent<LevelData>().customLevels[level];
        wallArray = new bool[levelWalls.GetLength(0), levelWalls.GetLength(1)];
        mapSizeX = wallArray.GetLength(1);
        mapSizeY = wallArray.GetLength(0);
        for (int i = 0; i < levelWalls.GetLength(0); i++)
        {
            for (int j = 0; j < levelWalls.GetLength(1); j++)
            {
                if (levelWalls[i, j] == 0) wallArray[i, j] = false;
                else wallArray[i, j] = true;
            }
        }
        mapCompleted = true;
    }

    void generateRandomWalls()
    {
        wallArray = new bool[mapSizeY, mapSizeX];
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < mapSizeX; j++)
            {
                wallArray[i,j] = true;
            }
        }
        for (int i = mapSizeY-2; i < mapSizeY; i++)
        {
            for (int j = 0; j < mapSizeX; j++)
            {
                wallArray[i,j] = true;
            }
        }
        for (int i = 2; i < mapSizeY-2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                wallArray[i,j] = true;
            }
        }
        for (int i = 2; i < mapSizeY - 2; i++)
        {
            for (int j = mapSizeX-2; j < mapSizeX; j++)
            {
                wallArray[i,j] = true;
            }
        }
        wallArray[(mapSizeY / 2), 0] = false;
        wallArray[(mapSizeY / 2), 1] = false;
        wallArray[(mapSizeY / 2), mapSizeX - 1] = false; 
        wallArray[(mapSizeY / 2), mapSizeX - 2] = false;

        for (int i = 3; i < mapSizeY-3; i++)
        {
            for (int j = 3; j < mapSizeX-3; j++)
            {
                if (Random.Range(0f, 1.0f) < blockSpawnChance && !wallOnT(i, j) && !wallOnB(i, j) && !wallOnB(i,j + 1) && !wallOnB(i, j - 1) && !wallOnL(i,j) && !wallOnR(i,j))
                {
                    int maxLength = 0;
                    int maxHeight = 0;
                    while (maxLength < maxBlockSize && !wallArray[i, j + maxLength+1] && !wallArray[i-1,j+maxLength+1]) 
                    {
                        maxLength++;
                    }
                    while (maxHeight < maxBlockSize && !wallArray[i+ maxHeight+1, j])
                    {
                        maxHeight++;
                    }
                    generateBlock(j,i,maxLength, maxHeight);
                }
            }
        }
        mapCompleted = true;
    }

    void generateBlock(int x, int y, int maxL, int maxH)
    {
        int length = (int)Random.Range(1, maxL+1);
        int height = (int)Random.Range(1, maxH+1);
        for (int i = y;i < y + height; i++)
        { 
            for (int j = x; j < x + length; j++)
            {
                wallArray[i, j] = true;
            }
        }
    }

    void fillTextures()
    {
        for (int i = 0; i < mapSizeY; i++)
        {
            for (int j = 0; j < mapSizeX; j++)
            {
                if (wallArray[i, j])
                {
                    bool wallOnTop = wallOnT(i, j);
                    bool wallOnBottom = wallOnB(i, j);
                    bool wallOnRight = wallOnR(i, j);
                    bool wallOnLeft = wallOnL(i, j);

                    if (wallOnRight && wallOnLeft && wallOnBottom && wallOnTop) tiles.SetTile(new Vector3Int(j, i, (int)tiles.transform.position.z), wallM);
                    else if (wallOnRight && wallOnLeft && wallOnBottom) tiles.SetTile(new Vector3Int(j, i, (int)tiles.transform.position.z), wallT);
                    else if (wallOnRight && wallOnLeft && wallOnTop) tiles.SetTile(new Vector3Int(j, i, (int)tiles.transform.position.z), wallB);
                    else if (wallOnRight && wallOnBottom && wallOnTop) tiles.SetTile(new Vector3Int(j, i, (int)tiles.transform.position.z), wallL);
                    else if (wallOnLeft && wallOnBottom && wallOnTop) tiles.SetTile(new Vector3Int(j, i, (int)tiles.transform.position.z), wallR);
                    else if (wallOnRight && wallOnLeft) tiles.SetTile(new Vector3Int(j, i, (int)tiles.transform.position.z), wallH);
                    else if (wallOnBottom && wallOnTop) tiles.SetTile(new Vector3Int(j, i, (int)tiles.transform.position.z), wallV);
                    else if (wallOnRight && wallOnBottom) tiles.SetTile(new Vector3Int(j, i, (int)tiles.transform.position.z), wallTL);
                    else if (wallOnRight && wallOnTop) tiles.SetTile(new Vector3Int(j, i, (int)tiles.transform.position.z), wallBL);
                    else if (wallOnLeft && wallOnBottom) tiles.SetTile(new Vector3Int(j, i, (int)tiles.transform.position.z), wallTR);
                    else if (wallOnLeft && wallOnTop) tiles.SetTile(new Vector3Int(j, i, (int)tiles.transform.position.z), wallBR);
                    else if (wallOnRight) tiles.SetTile(new Vector3Int(j, i, (int)tiles.transform.position.z), wall3L);
                    else if (wallOnLeft) tiles.SetTile(new Vector3Int(j, i, (int)tiles.transform.position.z), wall3R);
                    else if (wallOnBottom) tiles.SetTile(new Vector3Int(j, i, (int)tiles.transform.position.z), wall3T);
                    else if (wallOnTop) tiles.SetTile(new Vector3Int(j, i, (int)tiles.transform.position.z), wall3B);
                    else tiles.SetTile(new Vector3Int(j, i, (int)tiles.transform.position.z), wall4);
                }
                else
                {
                    tiles.SetTile(new Vector3Int(j, i, (int)tiles.transform.position.z), background);
                }
            }
        }
        tiles.SetTile(new Vector3Int(1, 1, (int)tiles.transform.position.z), wallCTR);
        tiles.SetTile(new Vector3Int(mapSizeX-2, 1, (int)tiles.transform.position.z), wallCTL);
        tiles.SetTile(new Vector3Int(1, mapSizeY-2, (int)tiles.transform.position.z), wallCBR);
        tiles.SetTile(new Vector3Int(mapSizeX-2, mapSizeY-2, (int)tiles.transform.position.z), wallCBL);
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

    public bool getMapCompleted()
    {
        return mapCompleted;
    }

    void printTileStates()
    {
        string s = "Tile states: \n";
        for (int i = 0; i < mapSizeY; i++)
        {
            for (int j = 0; j < mapSizeX; j++)
            {
                s += (wallArray[i, j] ? 1 : 0);
            }
            s += ("\n");
        }
        Debug.Log(s);
    }
}
