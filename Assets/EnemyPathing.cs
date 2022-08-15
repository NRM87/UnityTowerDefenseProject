using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyPathing : MonoBehaviour
{
    public string path;
    public float distanceTraveled;
    private int dna = -1;
    private float health = 0;
    private float speed = 0;
    private Vector3 targetPosition;
    private int next;
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;
    public static int id = 0;
    //"clock" rotation (0=90deg,1=0deg,2=270deg,3=180deg)
    private static int[] ydirs = new int[4] { 1, 0, -1, 0 };
    private static int[] xdirs = new int[4] { 0, 1, 0, -1 };
    private static float[] enemySpeeds = new float[2] { 1.0f , 1.75f};
    private static float[] enemyHealths = new float[2] { 10.0f, 10.0f };
    public Sprite[] enemySkins; //public because otherwise it would not show in unity editor
    private GameObject mapControl;

    void Start()
    {
        distanceTraveled = 0; 
        mapControl = GameObject.Find("MapCommand");
        speed = enemySpeeds[dna];
        health = enemyHealths[dna];
        GetComponent<SpriteRenderer>().sprite = enemySkins[dna];
        calculateTarget();
        name = "EnemyDNA[" + dna + "] " + id;
        id++;
    }
    void Update()
    {
        
        if (dna > -1 && health > 0 && mapControl.GetComponent<GameplayHandler>().getActive())
        {
            if (transform.position == targetPosition)
            {
                calculateTarget();
            } 
            else
            {
                //unsure if this is the most optimal movement and clamping code
                transform.Translate(speed * Time.deltaTime, 0f, 0f);
                distanceTraveled += Time.deltaTime * speed;
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);
            }
        }
        else
        {
            delete();
        }
    }

    void calculateTarget()
    {
        if (path.Length == 0) {
            delete(); //possibly change this to destroy upon reaching end rather then running out of path 
        }
        else
        {
            next = path[0] - '0';
            path = path.Substring(1, path.Length - 1);
            targetPosition = new Vector3(transform.position.x + xdirs[next], transform.position.y + ydirs[next], transform.position.z);
            transform.eulerAngles = new Vector3(0, 0, 90*(-next)+90);
            minX = Mathf.Min(transform.position.x, targetPosition.x);
            maxX = Mathf.Max(transform.position.x, targetPosition.x);
            minY = Mathf.Min(transform.position.y, targetPosition.y);
            maxY = Mathf.Max(transform.position.y, targetPosition.y);
        }
    }

    public void setDNA(int dna)
    {
        this.dna = dna;
    }
    private void delete()
    {
        mapControl.GetComponent<GameplayHandler>().enemies.Remove(gameObject);
        Destroy(gameObject);
    }
}
