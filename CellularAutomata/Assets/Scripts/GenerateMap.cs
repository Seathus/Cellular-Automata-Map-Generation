using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    private float chanceToStartAlive = 0.45f;
    private float birthLimit = 3;
    private float deathLimit = 2;
    private float numberOfSteps = 1;
    private float tileSpacing = 1.0f;
    bool[,] cellmap = new bool[100,100];
    public GameObject[] tiles = new GameObject[2];

    private bool[,] InitializeMap(bool[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (Random.Range(0.0f, 1.0f) < chanceToStartAlive)
                {
                    map[i, j] = true;
                }
            }
        }
        return map;
    }

    private bool[,] DoSimulationStep(bool[,] oldMap)
    {
        bool[,] newMap = new bool[oldMap.GetLength(0),oldMap.GetLength(1)];
        for (int i = 0; i < oldMap.GetLength(0); i++)
        {
            for (int j = 0; j < oldMap.GetLength(1); j++)
            {
                int nbs = CountAliveNeighbours(oldMap, i, j);
                //The new value is based on our simulation rules
                //First, if a cell is alive but has too few neighbours, kill it.
                if (oldMap[i, j])
                {
                    if (nbs < deathLimit)
                    {
                        newMap[i, j] = false;
                    }
                    else
                    {
                        newMap[i, j] = true;
                    }
                }
                else //Otherwise, if the cell is dead now, check if it has the right number of neighbours to be 'born'
                {
                    if (nbs > birthLimit)
                    {
                        newMap[i, j] = true;
                    }
                    else
                    {
                        newMap[i, j] = false;
                    }
                }
            }
        }

        return newMap;
    }

    private int CountAliveNeighbours(bool[,] map, int x, int y)
    {
        int count = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                int neighbour_x = x + i;
                int neighbour_y = y + j;
                //If we're looking at the middle point
                if(i == 0 && j == 0){
                    //Do nothing, we don't want to add ourselves in!
                }
                //In case the index we're looking at it off the edge of the map
                else if(neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= map.GetLength(0) || neighbour_y >= map.GetLength(1)){
                    count = count + 1;
                }
                //Otherwise, a normal check of the neighbour
                else if(map[neighbour_x,neighbour_y]){
                    count = count + 1;
                }
            }
        }

        return count;
    }

    private void PopulateWorld(bool[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
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

    private void GenerateWorld()
    {
        bool[,] finalMap = new bool[100,100];
        for (int i = 0; i < numberOfSteps; i++)
        {
            finalMap = DoSimulationStep(InitializeMap(cellmap));
        }
        PopulateWorld(finalMap);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateWorld();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
