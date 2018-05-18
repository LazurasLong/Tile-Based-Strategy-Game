using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {

    public int layer, xPos, yPos;
    public TileType type;
    public Sprite sprite;
    public GameObject originalSprite;

    public GameObject spawnedObject;

    public int temperature;
    public Improvement improvement;
    public Town town;
    public Road road;

    public Tile(int layerV, int xPosV,int yPosV)
    {
        layer = layerV;
        xPos = xPosV;
        yPos = yPosV;
    }

    int GetTemperature()
    {
        return temperature;
    }

    void SetTemperature(int temperatureV)
    {
        temperature = temperatureV;
    }

    void AddTemperature(int temperatureV)
    {
        temperature += temperatureV;
    }

    public bool DetermineWhetherToDestroyLayer()
    {
        int turnsExisted = GameManager.turnsSurvived - improvement.turnInstantiated;
        //Debug.Log("Turns Existed: " + turnsExisted);
        //Debug.Log("Turns Spawned: " + improvement.turnInstantiated);
        //Debug.Log("Turns To Destroy: " + improvement.turnsToDestroyTile);
        if (improvement.turnsToDestroyTile <= turnsExisted)
        {
            improvement.turnsToDestroyTile += turnsExisted;
            return true;
        }
        return false;
    }
    
    public enum TileType
    {
        Dirt,
        Grass,
        Water,
        Tree,
        Shrub,
        Stone,
        Coal,
        Ore
    }

    public void HandleTemperature(int temp,int abovTemp)
    {
        temperature = temp;
        if (layer + 1 >= GameManager.layers)
        {
            //Top Layer
            return;
        }

        int totalTemperatureFlowingDown = 0;
        int numOfHoles = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int xLocation = (xPos - 1) + i;
                int yLocation = (yPos - 1) + j;
                if ( (xLocation < 0 || xLocation >= GameManager.mapSize) || (yLocation < 0 || yLocation >= GameManager.mapSize))
                {
                    continue;
                }
                Tile aboveTile = GameManager.world[layer + 1,xLocation,yLocation];
                if(aboveTile == null)
                {
                    totalTemperatureFlowingDown += abovTemp;
                    numOfHoles++;
                    Debug.Log("HOLE" + xPos + " " + yPos);
                }
            }
        }
        if (numOfHoles == 0)
        {
            return;
        }
        temperature = (int)((temperature + (totalTemperatureFlowingDown / (numOfHoles * 1f))) / 2);
    }

    public void SetImprovement(Improvement improvementV)
    {
        improvement = new Improvement();
        improvement.CopyFrom(improvementV);
    }

    public void RemoveImprovement()
    {
        improvement = null;
    }

    public void SetTown(Town townV)
    {
        town = townV;
    }

    public void RemoveTown()
    {
        town = null;
    }

    public void PlaceRoad(int tech)
    {
        road = new Road(tech);
    }

    public void RemoveRoad()
    {
        road = null;
    }

    public void SetSprite(Sprite spriteV)
    {
        sprite = spriteV;
    }

    public void SetOriginalSprite(GameObject originalSpriteV)
    {
        originalSprite = originalSpriteV;
    }

    public GameObject PopSpawnedObject()
    {
        return spawnedObject;
    }
}
