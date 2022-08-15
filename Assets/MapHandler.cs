using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class MapHandler : MonoBehaviour
{
    public GameObject towers;
    public GameObject[] towerDragGhostList;
    public Sprite[] towerGunList;
    public TileBase[] towerTileList;
    public GameObject cam;
    public Tilemap tiles;
    public LineRenderer rangeLineRenderer;
    public int[,] tileData;
    private GameObject[,] activeTowers;
    private GameObject selected;
    private Color selectedColor;
    private bool selectionMade;
    private int lastXCell;
    private int lastYCell;
    private int selectIndex;
    private Vector3 mousePos;
    private Vector3Int mouseTilePos;

    void Start()
    {
        StartCoroutine(initializeTileData());
    }

    private IEnumerator initializeTileData()
    {
        yield return new WaitUntil(() => GetComponent<MapGenerator>().getMapCompleted());

        rangeLineRenderer.positionCount = 0;
        rangeLineRenderer.startWidth = 0.1f;
        rangeLineRenderer.endWidth = 0.1f;
        rangeLineRenderer.startColor = new Color(0.25f, 0.8f, 0.95f, 0.5f);
        rangeLineRenderer.endColor = new Color(0.25f, 0.8f, 0.95f, 0.5f);

        tileData = new int[GetComponent<MapGenerator>().wallArray.GetLength(0), GetComponent<MapGenerator>().wallArray.GetLength(1)];
        activeTowers = new GameObject[GetComponent<MapGenerator>().wallArray.GetLength(0), GetComponent<MapGenerator>().wallArray.GetLength(1)];

        for (int i = 0; i < GetComponent<MapGenerator>().wallArray.GetLength(0); i++)
        {
            for (int j = 0; j < GetComponent<MapGenerator>().wallArray.GetLength(1); j++)
            {
                tileData[i, j] = (GetComponent<MapGenerator>().wallArray[i, j] ? 1 : 0);
            }
        }
        int level = GetComponent<MapGenerator>().level;
        if (level >= 0)
        {
            int[] startPoints = GetComponent<LevelData>().customStartPoints[level];
            int[] endPoints = GetComponent<LevelData>().customEndPoints[level];
            for (int i = 0; i < startPoints.Length; i+=2)
            {
                tileData[startPoints[i+1], startPoints[i]] = -1;
            }
            for (int i = 0; i < endPoints.Length; i += 2)
            {
                tileData[endPoints[i+1], endPoints[i]] = -2;
            }
        }
        else
        {
            tileData[GetComponent<MapGenerator>().wallArray.GetLength(0) / 2, 0] = -1; //start tile
            tileData[GetComponent<MapGenerator>().wallArray.GetLength(0) / 2, GetComponent<MapGenerator>().mapSizeX - 1] = -2; //end tile
        }
    }
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseTilePos = tiles.WorldToCell(mousePos);
        if (selectionMade) {
            if (mousePos.x < cam.GetComponent<CameraControl>().menuLine)
            {
                selected.transform.position = new Vector3(Mathf.Floor(mousePos.x) + 0.5f, Mathf.Floor(mousePos.y) + 0.5f, mousePos.z + 1);
                if (Input.GetMouseButtonDown(0) && !tileOccupied(mousePos)) //TODO: prevent tower spawn on active path and only possible path;
                {
                    tileData[mouseTilePos.y, mouseTilePos.x] = 2;
                    tiles.SetTile(mouseTilePos, towerTileList[selectIndex]);
                    GameObject towerInstance = Instantiate(towers);
                    towerInstance.GetComponent<TowerTargeting>().setDNA(selectIndex);
                    towerInstance.GetComponent<Transform>().position = tiles.GetCellCenterWorld(mouseTilePos);
                    towerInstance.GetComponent<Transform>().Translate(0,0,-1);
                    activeTowers[mouseTilePos.y, mouseTilePos.x] = towerInstance;
                    removeSelected();
                }
            } 
            else
            {
                selected.transform.position = new Vector3(mousePos.x, mousePos.y, mousePos.z + 1);
                if (Input.GetMouseButtonDown(0))
                    removeSelected(); 
            }
            if (Input.GetMouseButtonDown(1))
                removeSelected();
        }
        
        if (tileData != null && mouseTilePos.x == lastXCell && mouseTilePos.y == lastYCell && mouseInBounds() && tileData[mouseTilePos.y, mouseTilePos.x] == 2)
        {
            showRange(activeTowers[mouseTilePos.y, mouseTilePos.x]);
        } 
        else
        {
            rangeLineRenderer.positionCount = 0;
        }

        lastXCell = mouseTilePos.x;
        lastYCell = mouseTilePos.y;
    }
    private void showRange(GameObject selectedTower)
    {
        float range = selectedTower.GetComponent<TowerTargeting>().range;
        float posY = selectedTower.GetComponent<Transform>().position.y;
        float posX = selectedTower.GetComponent<Transform>().position.x;
        List<Vector3> rangeLinePoints = new List<Vector3>();
        int vertexCount = (int)(20 * range);
        for (int i = 0; i < vertexCount; i++) //aabb error when i = vertexCount+1, find a fix?
        {
            float angle = (((2 * Mathf.PI) / vertexCount) * i);
            rangeLinePoints.Add(new Vector3(posX + Mathf.Cos(angle) * range, posY + Mathf.Sin(angle) * range, -0.5f));
        }
        rangeLinePoints.Add(new Vector3(posX + Mathf.Cos(0) * range, posY + Mathf.Sin(0) * range, -0.5f));
        rangeLineRenderer.positionCount = rangeLinePoints.Count;
        rangeLineRenderer.SetPositions(rangeLinePoints.ToArray());
    }
    public void selectTower(int index)
    {
        selectIndex = index;
        selected = Instantiate(towerDragGhostList[index]);
        selectedColor = selected.GetComponent<SpriteRenderer>().color;
        selectedColor.a = 0.5f;
        selected.GetComponent<SpriteRenderer>().color = selectedColor;
        selectionMade = true;
    }
    bool tileOccupied(Vector3 target)
    {
        Vector3Int tilePos = tiles.WorldToCell(target);
        return tileData[tilePos.y, tilePos.x] > 0;
    }

    private bool mouseInBounds()
    {
        return (mouseTilePos.y >= 0 && mouseTilePos.y < tileData.GetLength(0)
            && mouseTilePos.x >= 0 && mousePos.x < cam.GetComponent<CameraControl>().menuLine);
    }
    void removeSelected()
    {
        selectIndex = -1;
        Destroy(selected);
        selected = null;
        selectionMade = false;
    }
    void printWalls()
    {
        string s = "Tile states: \n";
        for (int i = 0; i < GetComponent<MapGenerator>().mapSizeY; i++)
        {
            for (int j = 0; j < GetComponent<MapGenerator>().mapSizeX; j++)
            {
                s += tileData[i,j];
            }
            s += ("\n");
        }
        Debug.Log(s);
    }
}
