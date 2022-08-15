using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTargeting : MonoBehaviour
{
    public float range;
    private int dna = -1;
    private int targeting = -1;
    private List<GameObject> enemies = new List<GameObject>();
    private GameObject mapControl;
    public GameObject target;
    private static float[] towerRanges = new float[4] { 2.0f, 2.0f, 4.0f, 3.0f };

    void Start()
    {
        mapControl = GameObject.Find("MapCommand");
        enemies = mapControl.GetComponent<GameplayHandler>().enemies;
        range = towerRanges[dna]+0.5f; 

        targeting = 0;
        target = null;
    }

    void Update()
    {
        if (mapControl.GetComponent<GameplayHandler>().getActive())
        {
            if (targeting == 0) //targeting by "first"
            {
                findFirstTarget();
                
            } 
            else if (targeting == 1)
            {
                findLastTarget();
                //TODO: implement targetting system for more targetting types (last, strong, close, etc...)
            }

            if (target != null && inRange(target))
            {
                pointAtTarget();
            }
        }

        

    }

    private void findFirstTarget()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (inRange(enemies[i]))
            {
                target = enemies[i];
                break;
            }
        }
    }

    private void findLastTarget()
    {
        for (int i = enemies.Count-1; i >= 0; i--)
        {
            if (inRange(enemies[i]))
            {
                target = enemies[i];
                break;
            }
        }
    }
    private bool inRange(GameObject enemy) //TODO: figure out how to account for walls and towers blocking LOS
    {
        float xDist = enemy.transform.position.x - transform.position.x;
        float yDist = enemy.transform.position.y - transform.position.y;
        return Mathf.Sqrt(xDist * xDist + yDist * yDist) < range;
    }

    private void pointAtTarget()
    {
        float xDist = target.transform.position.x - transform.position.x;
        float yDist = target.transform.position.y - transform.position.y;
        transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(yDist,xDist));
    }

    public void setTargeting(int mode)
    {
        targeting = mode;
    }
    public void setDNA(int dna)
    {
        this.dna = dna;
    }

    
}
