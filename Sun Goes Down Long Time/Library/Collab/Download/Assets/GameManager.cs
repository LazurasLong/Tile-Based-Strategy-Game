using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static int mapSize = 10;
    public static int layers = 5;

    [SerializeField]
    public TechTree[] possibleImprovements;

    [SerializeField]
    public GameObject[] tiles;
    [SerializeField]
    public GameObject[] layerParents;

    public static int destructionTimer = 15;
    public static int turnsSurvived = 0;
    public static Tile[,,] world;

    public LinkedList<Tile> townTiles;
    public LinkedList<Metro> metros;
    public ResourceHolder resources;

    float scale = 5;
    float randomGenOffset;

    public int displayedLayer;

    public int railRoadLevel;
    public int townLevel;

    void InitializeVariables()
    {
        railRoadLevel = 1;
        townLevel = 1;

        resources = new ResourceHolder();
        resources.SetToStartup();

        world = new Tile[layers, mapSize, mapSize];
        townTiles = new LinkedList<Tile>();
    }

    public bool IsControlled(int layer, int xPos, int yPos)
    {
        LinkedListNode<Metro> node = metros.First;
        Tile tile = world[layer, xPos, yPos];

        for (int i = 0; i < metros.Count; i++)
        {
            if (node.Value.controlledTiles.Contains(tile))
            {
                Debug.Log("IS CONTROLLED");
                return true;
            }

            node = node.Next;
        }
        return false;
    }

    void WorldSetup()
    {
        //RANDOMNESS VARIABLES
        scale = 2; //Larger Scale Is Farther Zoomed Out

        for (int i = 0; i < layers; i++)
        {
            randomGenOffset = Random.Range(0, 10000);
            for (int j = 0; j < mapSize; j++)
            {
                for (int k = 0; k < mapSize; k++)
                {
                    world[i, j, k] = new Tile(i,j,k); //CHANGE THIS
                    DetermineTileType(i, j, k, world[i, j, k]);
                }
            }
        }
    }

    void SpawnWorld()
    {
        for (int i = 0; i < layers; i++)
        {
            SpawnLayer(i);
        }
        DisplayLayer(layers - 1);
        displayedLayer = layers - 1;
    }

    void DetermineTileType(int layer, int xPos, int yPos, Tile tile)
    {

        float xCoord = randomGenOffset + ( (float) xPos) / ( (float) mapSize) * scale;
        float yCoord = randomGenOffset + ( (float) yPos) / ( (float) mapSize) * scale;
        float perlinValue = Mathf.PerlinNoise(xCoord, yCoord);
        //Top Of The World
        if (layer == (layers - 1))
        {
            InitialSetTopLayerTile(perlinValue, tile);
        }
        //Underground
        else
        {
            InitialSetUndergroundLayers(perlinValue, tile);
        }
    }

    void InitialSetTopLayerTile(float perlinValue, Tile tile)
    {
        if(perlinValue < .3)
        {
            //Water
            tile.type = Tile.TileType.Water;
            tile.SetSprite(tiles[0]);
            tile.SetOriginalSprite(tiles[0]);
        }
        else
        {
            //Grass
            tile.type = Tile.TileType.Grass;
            tile.SetSprite(tiles[1]);
            tile.SetOriginalSprite(tiles[1]);
        }
    }

    void InitialSetUndergroundLayers(float perlinValue, Tile tile)
    {
        if(perlinValue < .3 || perlinValue > .9)
        {
            tile.type = Tile.TileType.Coal;
            tile.SetSprite(tiles[3]);
            tile.SetOriginalSprite(tiles[3]);
        }
        else if((perlinValue > .65 && perlinValue < .8))
        {
            tile.type = Tile.TileType.Ore;
            tile.SetSprite(tiles[4]);
            tile.SetOriginalSprite(tiles[4]);
        }
        else
        {
            tile.type = Tile.TileType.Stone;
            tile.SetSprite(tiles[2]);
            tile.SetOriginalSprite(tiles[2]);
        }
    }

    void MakeMetros()
    {
        metros = new LinkedList<Metro>();
        ResetTownMetroStatus();

        Debug.Log("THIS MANY THINGS: " + townTiles.Count);
        LinkedListNode<Tile> head = townTiles.First;
        for (int i = 0; i < townTiles.Count; i++)
        {
            if(head.Value.town.inMetro == false)
            {
                Metro metro = new Metro();
                metros.AddLast(metro);
                metro.AddTownTile(head.Value);
            }

            head = head.Next;
        }
    }

    void ResetTownMetroStatus()
    {
        LinkedListNode<Tile> head = townTiles.First;
        for (int i = 0; i < townTiles.Count; i++)
        {
            head.Value.town.inMetro = false;

            head = head.Next;
        }
    }

    /*
     *      --------------------
     *      CONSTRUCTION METHODS
     *      --------------------
     */

    public bool PlaceTown(int layer, int xLocation, int yLocation)
    {
        Town town = new Town(layer, xLocation, yLocation, townLevel);

        if (!resources.GreaterThanOrEqualTo(town.buildingCost))
        {
            return false;
        }

        resources.Subtract(town.buildingCost);

        Tile tile = world[layer, xLocation, yLocation];
        tile.SetTown(town);
        townTiles.AddLast(world[layer, xLocation, yLocation]);

        UpdateBlock(tile);

        MakeMetros();
        return true;
    }

    //Overriding problem ALSO UPGRADES
    public bool PlaceRoad(int layer, int xLocation, int yLocation)
    {
        Tile tile = world[layer, xLocation, yLocation];
        tile.PlaceRoad();

        if (!resources.GreaterThanOrEqualTo(tile.road.buildingCost))
        {
            Debug.Log("Cost Too Much For Roads");
            tile.RemoveRoad();
            return false;
        }

        resources.Subtract(tile.road.buildingCost);

        UpdateBlock(tile);

        MakeMetros();
        return true;
    }

    //Check for existing improvement and also set index improvements also make sure this provence is in one of the metros
    public bool PlaceImprovement(int layer, int xLocation, int yLocation,int indexOfImprovement,int techOfImprovement)
    {
        Tile tile = world[layer, xLocation, yLocation];

        if (!resources.GreaterThanOrEqualTo(possibleImprovements[indexOfImprovement].techLevel[techOfImprovement].buildingCost))
        {
            return false;
        }

        resources.Subtract(possibleImprovements[indexOfImprovement].techLevel[techOfImprovement].buildingCost);

        tile.SetImprovement(possibleImprovements[indexOfImprovement].techLevel[techOfImprovement]);

        UpdateBlock(tile);

        return true;
    }

    public bool CheckIfClear(int layer, int xPos, int yPos, PlayerController.Player_State state)
    {
        if(state == PlayerController.Player_State.Connect && world[layer, xPos, yPos].town != null)
        {
            return false;
        }
        if( ((world[layer,xPos,yPos].improvement != null) && (state != PlayerController.Player_State.Connect)) || (world[layer, xPos, yPos].town != null))
        {
            return false;
        }
        return true;
    }

    void UpdateBlock(Tile tile)
    {
        DetermineTileRepresentation(tile);
        ChangeBlock(tile);
    }

    public void ChangeBlock(Tile tile)
    {
        tile.GenerateTile();
    }

    void DetermineTileRepresentation(Tile tile)
    {
        if(tile.town != null)
        {
            Debug.Log("Town Return");
            tile.SetSprite(tiles[6]);
        }
        else if (tile.improvement != null)
        {
            Debug.Log("Improvement Return");
            tile.SetSprite(tiles[5]);
        }
        else if (tile.road != null)
        {
            Debug.Log("Road Return");
            tile.SetSprite(tiles[7]);
        }
        else
        {
            Debug.Log("Environmental Return");
            tile.SetSpriteToOriginalSprite();
        }
    }

    //RELOCATE MISSING PEOPLE
    public void RemoveTown(int layer, int xPos, int yPos)
    {
        Town town = world[layer, xPos, yPos].town;
        world[layer, xPos, yPos].RemoveTown();
        UpdateBlock(world[layer, xPos, yPos]);

        if (town != null)
        {
            resources.Add(town.buildingCost);
        }
    }

    public void RemoveImprovement(int layer, int xPos, int yPos)
    {
        Improvement improvement = world[layer, xPos, yPos].improvement;
        world[layer, xPos, yPos].RemoveImprovement();

        UpdateBlock(world[layer, xPos, yPos]);
        if(improvement != null)
        {
            resources.Add(improvement.buildingCost);
        }
    }

    public void RemoveRoad(int layer, int xPos, int yPos)
    {
        Road road = world[layer, xPos, yPos].road;
        if (road != null)
        {
            resources.Add(road.buildingCost);
        }
        world[layer, xPos, yPos].RemoveRoad();

        UpdateBlock(world[layer, xPos, yPos]);
    }

    void DebugMetros()
    {
        WorldSetup();
        PlaceTown(4, 3, 3);
        PlaceTown(4, 0, 3);
        PlaceRoad(4, 1, 3);
        PlaceRoad(4, 2, 3);
        MakeMetros();
        Debug.Log(metros.Count);
        Debug.Log(metros.First.Value.controlledTiles.Count);
        Debug.Log(metros.Last.Value.controlledTiles.Count);
    }

    void SpawnLayer(int layer)
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                Tile tile = world[layer, i, j];
                Vector3 location = new Vector3(i, j, -layer);
                GameObject spawnedObject = Instantiate(tile.sprite,location,Quaternion.identity,layerParents[layer].transform);
                tile.spawnedObject = spawnedObject;
                SpriteLocationHolder slh = spawnedObject.AddComponent<SpriteLocationHolder>();
                slh.layer = layer;
                slh.xPos = i;
                slh.yPos = j;
            }
        }
    }

    public static void RemoveBlock(int layer, int xPos, int yPos)
    {
        Tile tile = world[layer, xPos, yPos];
        world[layer, xPos, yPos] = null;
        tile.DestroySpawnedObject();
    }

    public void DisplayLayer(int layer)
    {
        displayedLayer = layer;

        for (int i = 0; i < layerParents.Length; i++)
        {
            if(i <= layer)
            {
                layerParents[i].gameObject.SetActive(true);
                continue;
            }
            layerParents[i].gameObject.SetActive(false);
        }
    }

    public void ExecuteTurn()
    {
        turnsSurvived++;
        DetermineResourceGain();
    }

    bool IsEndOfTimes()
    {
        if(turnsSurvived > destructionTimer)
        {
            return true;
        }
        return false;
    }

    void DetermineResourceGain()
    {
        ResourceHolder gains;
        LinkedListNode<Metro> node = metros.First;
        Debug.Log("Metros: " + metros.Count);
        for (int i = 0; i < metros.Count; i++)
        {
            LinkedList<Improvement> temp = node.Value.workplaces;
            Debug.Log("Workplaces: " + temp.Count);
            LinkedListNode<Improvement> secondNode = temp.First;
            for (int j = 0; j < temp.Count; j++)
            {
                secondNode.Value.Harvest(resources);
            }
            node = node.Next;
        }
    }

    private void Start()
    {
        InitializeVariables();
        WorldSetup();
        SpawnWorld();
    }
}
