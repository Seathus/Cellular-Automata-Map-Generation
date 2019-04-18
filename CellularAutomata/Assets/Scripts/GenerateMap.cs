using System;
using UnityEngine;
using Random = System.Random;

public class GenerateMap : MonoBehaviour
{
    [Range(0, 20)] [SerializeField] private float birthLimit = 11;

    [SerializeField] private bool[,] cellmap = new bool[50, 15];

    [Range(0, 100)] [SerializeField] private float chanceToStartAlive = 45.0f;

    [Range(0, 20)] [SerializeField] private float deathLimit = 5;

    [Range(0, 5)] [SerializeField] private float numberOfSteps = 3;

    [SerializeField] private string seed;

    public GameObject[] tiles = new GameObject[2];
    private readonly float tileSpacing = 1.0f;
    public bool useRandomSeed;

    private bool[,] InitializeMap(bool[,] map)
    {
        if (useRandomSeed) seed = DateTime.Now.Ticks.ToString();
        var pseudoRandom = new Random(seed.GetHashCode());

        for (var i = 0; i < map.GetLength(0); i++)
        for (var j = 0; j < map.GetLength(1); j++)
            if (pseudoRandom.Next(0, 100) < chanceToStartAlive)
                map[i, j] = true;
        return map;
    }

    private bool[,] DoSimulationStep(bool[,] oldMap)
    {
        var newMap = new bool[oldMap.GetLength(0), oldMap.GetLength(1)];
        for (var i = 0; i < oldMap.GetLength(0); i++)
        for (var j = 0; j < oldMap.GetLength(1); j++)
        {
            var nbs = CountAliveNeighbours(oldMap, i, j);
            //The new value is based on our simulation rules
            //First, if a cell is alive but has too few neighbours, kill it.
            if (oldMap[i, j])
            {
                if (nbs < deathLimit)
                    newMap[i, j] = false;
                else
                    newMap[i, j] = true;
            }
            else //Otherwise, if the cell is dead now, check if it has the right number of neighbours to be 'born'
            {
                if (nbs > birthLimit)
                    newMap[i, j] = true;
                else
                    newMap[i, j] = false;
            }
        }

        return newMap;
    }

    private int CountAliveNeighbours(bool[,] map, int x, int y)
    {
        var count = 0;
        for (var i = -1; i < 2; i++)
        for (var j = -1; j < 2; j++)
        {
            var neighbour_x = x + i;
            var neighbour_y = y + j;
            //If we're looking at the middle point
            if (i == 0 && j == 0)
            {
                //Do nothing, we don't want to add ourselves in!
            }
            //In case the index we're looking at it off the edge of the map
            else if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= map.GetLength(0) ||
                     neighbour_y >= map.GetLength(1))
            {
                count = count + 1;
            }
            //Otherwise, a normal check of the neighbour
            else if (map[neighbour_x, neighbour_y])
            {
                count = count + 1;
            }
        }

        return count;
    }

    private void PopulateWorld(bool[,] map)
    {
        for (var i = 0; i < map.GetLength(0); i++)
        for (var j = 0; j < map.GetLength(1); j++)
        {
            GameObject tile;
            if (map[i, j])
                tile = Instantiate(tiles[0], new Vector3(tileSpacing * i, 0, tileSpacing * j), Quaternion.identity);
            else
                tile = Instantiate(tiles[1], new Vector3(tileSpacing * i, 0, tileSpacing * j), Quaternion.identity);

            tile.transform.SetParent(transform, false);
        }
    }

    private void GenerateWorld()
    {
        var finalMap = new bool[50, 15];
        for (var i = 0; i < numberOfSteps; i++) finalMap = DoSimulationStep(InitializeMap(cellmap));
        PopulateWorld(finalMap);
    }

    private void CleanWorld()
    {
        for (var i = 0; i < cellmap.GetLength(0); i++)
        for (var j = 0; j < cellmap.GetLength(1); j++)
            cellmap[i, j] = false;

        for (var i = 0; i < transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);
    }

    // Start is called before the first frame update
    private void Start()
    {
        GenerateWorld();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CleanWorld();
            GenerateWorld();
        }
    }
}