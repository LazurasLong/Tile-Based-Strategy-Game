using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metro {

    public LinkedList<Tile> townTiles;
    public LinkedList<Tile> controlledTiles;
    public LinkedList<Tile> tilesWithRoads;

    public LinkedList<Improvement> workplaces;
    int openJobs;

    public Metro()
    {
        townTiles = new LinkedList<Tile>();
        controlledTiles = new LinkedList<Tile>();
    }

    public void AddTownTile(Tile townTile)
    {
        townTiles.AddLast(townTile);
        DetermineControlledTiles();
    }

    //Gets the total population
    int GetTotalPopulation()
    {
        int total = 0;
        LinkedListNode<Tile> head = townTiles.First;
        //loops through all the towns
        for (int i = 0; i < townTiles.Count; i++)
        {
            //amount of town inhabitants added to total
            total += head.Value.town.inhabitants.Count;

            head = head.Next;
        }

        return total;
    }

    public void DetermineControlledTiles()
    {

        LinkedListNode<Tile> head = townTiles.First;
        Debug.Log("Head Tiles Length: " + townTiles.Count);
        //loops through all the towns
        for (int i = 0; i < townTiles.Count; i++)
        {
            //amount of town inhabitants added to total
            AddSurroundingTownTilesToControlledTiles(head.Value);

            head = head.Next;
        }

        AddSurroundingRoadTilesToControlledTiles();
        DetermineWorkplaces();
    }

    void AddSurroundingTownTilesToControlledTiles(Tile town)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int xLocation = town.xPos - 1 + i;
                int yLocation = town.yPos - 1 + j;
                if(xLocation < 0 || yLocation < 0 || xLocation >= GameManager.mapSize || yLocation >= GameManager.mapSize)
                {
                    continue;
                }
                addControlledTile(GameManager.world[town.layer, xLocation, yLocation]);
            }
        }
    }

    void AddRoadTile(Tile tile)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if( (i == 0 && j == 0) || (i == 0 && j == 2) || (i == 2 && j == 0) || (i == 2 && j == 2))
                {
                    continue;
                }

                int xLocation = tile.xPos - 1 + i;
                int yLocation = tile.yPos - 1 + j;
                if (xLocation < 0 || yLocation < 0 || xLocation >= GameManager.mapSize || yLocation >= GameManager.mapSize)
                {
                    continue;
                }

                addControlledTile(GameManager.world[tile.layer, tile.xPos - 1 + i, tile.yPos - 1 + j]);
            }
        }
    }

    void AddSurroundingRoadTilesToControlledTiles()
    {
        tilesWithRoads = new LinkedList<Tile>();
        LinkedListNode<Tile> head = controlledTiles.First;
        for (int i = 0; i < controlledTiles.Count; i++)
        {
            
            if (head.Value.road != null && head.Value.road.wasChecked == false)
            {
                Debug.Log("HELLO");
                head.Value.road.wasChecked = true;
                tilesWithRoads.AddLast(head.Value);
                AddRoadTile(head.Value);
            }

            head = head.Next;
        }
        if (tilesWithRoads.Count == 0)
        {
            return;
        }
        ResetRoadTiles();
    }

    void ResetRoadTiles()
    {
        LinkedListNode<Tile> head = tilesWithRoads.First;
        for (int i = 0; i < tilesWithRoads.Count; i++)
        {
            head.Value.road.wasChecked = false;

            head = head.Next;
        }
    }

    void addControlledTile(Tile tile)
    {
        if(tile.town != null)
        {
            Debug.Log("THIS IS A TOWN THAT WAS FOUND");
            tile.town.inMetro = true;
        }

        if (controlledTiles.Contains(tile))
        {
            return;
        }
        Debug.Log("added a terriots");
        controlledTiles.AddLast(tile);
    }

    void DetermineWorkplaces()
    {
        workplaces = new LinkedList<Improvement>();
        openJobs = 0;
        LinkedListNode<Tile> head = controlledTiles.First;

        Debug.Log(controlledTiles.Count + "!!!!!!!!!!!");
        for (int i = 0; i < controlledTiles.Count; i++)
        {
            Debug.Log("In Loop");
            if(head.Value.improvement != null)
            {
                Debug.Log("Improvement FOUND");
                openJobs += (head.Value.improvement.maxWorkers - head.Value.improvement.workers);
                workplaces.AddLast(head.Value.improvement);
            }

            head = head.Next;
        }
    }

}
