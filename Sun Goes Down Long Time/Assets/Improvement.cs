using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Improvement {

    public Sprite sprite; //MAYBE
    public Sprite roadSprite;

    public Tile.TileType[] buildableOnTileTypes;
    public ResourceHolder buildingCost;
    public ResourceHolder required;
    public ResourceHolder output;
    public int maxWorkers;
    public int workers;
    public int turnsToDestroyTile;
    public bool isRefinery;
    public bool canFunctionAfterDoomsday;
    public int turnInstantiated;
    public LinkedList<Pop> workersLinkedList;
    bool harvestedLastTurn;

    public void Reset()
    {
        workers = 0;
        workersLinkedList = new LinkedList<Pop>();
    }

    public bool ContainsTileType(Tile.TileType type)
    {
        if(buildableOnTileTypes == null)
        {
            Debug.LogError("Set The Tile Types For This Improvement");
            return false;
        }

        for (int i = 0; i < buildableOnTileTypes.Length; i++)
        {
            if(type == buildableOnTileTypes[i])
            {
                return true;
            }
        }
        return false;
    }

    public void CopyFrom(Improvement guide)
    {
        buildingCost = new ResourceHolder();
        required = new ResourceHolder();
        output = new ResourceHolder();

        buildingCost.Copy(guide.buildingCost);
        required.Copy(guide.required);
        output.Copy(guide.output);

        sprite = guide.sprite;
        roadSprite = guide.roadSprite;
        buildableOnTileTypes = guide.buildableOnTileTypes;
        maxWorkers = guide.maxWorkers;
        workers = 0;
        turnsToDestroyTile = guide.turnsToDestroyTile;
        isRefinery = guide.isRefinery;
        turnInstantiated = GameManager.turnsSurvived;
        workersLinkedList = new LinkedList<Pop>();
        harvestedLastTurn = false;
    }

    public bool OpenJob()
    {
        if(workers < maxWorkers)
        {
            return true;
        }
        return false;
    }
    
    //return true if person added, false otherwise.
    public bool AddWorker(Pop person)
    {
        if (!workersLinkedList.Contains(person) && OpenJob())
        {
            workers++;
            workersLinkedList.AddLast(person);
            return true;
        }
        return false;
    }

    //Returns true if successful and false otherwise
    public bool RemoveWorker(Pop person)
    {
        if (workersLinkedList.Remove(person))
        {
            workers--;
            return true;
        }
        return false;
    }

    public bool RequirementMet(ResourceHolder resources)
    {
        if( (GameManager.IsEndOfTimes()) && (!canFunctionAfterDoomsday))
        {
            Debug.Log("DOOM");
            return false;
        }

        if (resources.GreaterThanOrEqualTo(required))
        {
            return true;
        }
        return false;
    }

    public ResourceHolder GetOutput()
    {
        return output;
    }

    //IF NEW RESOURCE ADDED FIX THIS
    public void SetToResourceBeingMined(ResourceHolder resources, ResourceHolder minedResources, Tile.TileType tileType)
    {
        if( (tileType == Tile.TileType.Dirt) || (tileType == Tile.TileType.Dirt))
        {
            minedResources.dirt = resources.dirt;
        }
        else if( (tileType == Tile.TileType.Coal))
        {
            minedResources.coal = resources.coal;
        }
        else if ((tileType == Tile.TileType.Ore))
        {
            minedResources.rawOre = resources.rawOre;
        }
        else if ((tileType == Tile.TileType.Stone))
        {
            minedResources.stone = resources.stone;
        }
    }

    public bool Harvest(ResourceHolder resources, Tile.TileType tileType, bool isRefining)
    {
        if (isRefinery != isRefining)
        {
            return false;
        }
        Debug.Log(isRefining + "vs" + isRefinery);
        if (RequirementMet(resources))
        {
            Debug.Log(isRefining + "vs" + isRefinery + "XXXXXXXXXXXXXXXXX");
            if (turnsToDestroyTile != -1)
            {
                //Mines Here
                ResourceHolder minedResources = new ResourceHolder();
                SetToResourceBeingMined(resources, minedResources, tileType);

                resources.Subtract(required);
                resources.Add(minedResources);
                harvestedLastTurn = true;
                return true;
            }

            //Debug.Log("Resources Meet Rquirement");
            Debug.Log(isRefining);
            resources.Subtract(required);
            resources.Add(output);
            harvestedLastTurn = true;
            return true;
        }
        //Debug.Log("Resources DONT Meet Rquirement");
        harvestedLastTurn = false;
        return false;
    }

}
