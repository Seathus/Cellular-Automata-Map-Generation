using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    private float width = 10;
    private float height = 10;
    private float chanceToStartAlive = 0.45f;
    private float tileSpacing = 1.0f;
    bool[,] cellmap = new bool[10,10];

    public GameObject[] tiles = new GameObject[2];

    private bool[,] InitializeMap(bool[,] map)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (Random.Range(0.0f, 1.0f) < chanceToStartAlive)
                {
                    map[i, j] = true;
                }
            }
        }
        return map;
    }

    void PopulateWorld(bool[,] map)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (map[i, j])
                {
                    Instantiate(tiles[0], new Vector3(tileSpacing * i,0,tileSpacing * j), Quaternion.identity);
                }
                else
                {
                    Instantiate(tiles[1], new Vector3(tileSpacing * i,0,tileSpacing * j), Quaternion.identity);
                }
            }
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        PopulateWorld(InitializeMap(cellmap));
        Debug.Log("GeneratedMap");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
