using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject mapControl;
    public GameObject menuBlock;
    public float scrollSens = 5.0f;
    public float minZoom = 4.0f;
    public float maxMenuWidth = 11.0f;
    public float menuLine;
    private float maxZoom;
    private float mapWidth;
    private float mapHeight;
    private float maxScreenWidth;
    private float moveSpeed;
    private bool dragOriginSet;
    private Vector3 menuPos;
    private Vector3 menuScale;
    private Vector3 dragOrigin;
    private Vector3 dragDifference;
    void Start()
    {
        mapHeight = mapControl.GetComponent<MapGenerator>().mapSizeY;
        mapWidth = mapControl.GetComponent<MapGenerator>().mapSizeX;
        maxScreenWidth = maxMenuWidth + mapWidth;
        maxZoom = Mathf.Min(mapHeight / 2.0f, ((mapWidth + maxMenuWidth) / Camera.main.aspect) / 2.0f);
        Camera.main.transform.position = new Vector3((mapWidth+menuBlock.GetComponent<Transform>().lossyScale.x)/2, mapHeight/2, -10f);
        Camera.main.orthographicSize = maxZoom;
    }
    void Update()
    {
        //process scroll wheel and zoom
        Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * scrollSens;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
        moveSpeed = Camera.main.orthographicSize * 0.005f;

        //process inputs and movements
        if (Input.GetKey("a") || Input.GetKey("left"))
        {
            transform.Translate(-moveSpeed, 0.0f, 0.0f);
        }
        if (Input.GetKey("w") || Input.GetKey("up"))
        {
            transform.Translate(0.0f, moveSpeed, 0.0f);
        }
        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            transform.Translate(moveSpeed, 0.0f, 0.0f);
        }
        if (Input.GetKey("s") || Input.GetKey("down"))
        {
            transform.Translate(0.0f, -moveSpeed, 0.0f);
        }
        if (Input.GetMouseButtonDown(0) && cursorInDragBounds())
        {
            dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragOriginSet = true;
        }
        if (Input.GetMouseButton(0) && dragOriginSet)
        {
            dragDifference = dragOrigin - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.transform.position += dragDifference;
        } 
        else
        {
            dragOriginSet = false;
        }

        float screenWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float camHeight = Camera.main.orthographicSize;
        menuLine = Camera.main.transform.position.x + screenWidth * (1.0f - 2 * maxMenuWidth / maxScreenWidth); 
        //clamp camera position
        float minX = screenWidth;
        float minY = camHeight;
        float maxX = mapWidth - screenWidth + menuBlock.GetComponent<Transform>().lossyScale.x;
        float maxY = mapHeight - camHeight;
        Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x, minX, maxX), Mathf.Clamp(Camera.main.transform.position.y, minY, maxY), -10f);

        //keep menu within screen
        menuPos = new Vector3(Camera.main.transform.position.x + ((screenWidth) * (1.0f - maxMenuWidth/maxScreenWidth)), Camera.main.transform.position.y, -5.0f);
        menuBlock.GetComponent<Transform>().position = menuPos;
        menuScale = new Vector3((2.0f * screenWidth * (maxMenuWidth / maxScreenWidth)), (camHeight * 2),1);
        menuBlock.GetComponent<Transform>().localScale = menuScale;
        
        

    }
    
    bool cursorInDragBounds()
    {
        float mouseX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        return (mouseX < menuLine && mouseX > Camera.main.transform.position.x - (Camera.main.orthographicSize * Camera.main.aspect));
    }
}
