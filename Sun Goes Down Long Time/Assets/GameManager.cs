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
    public GameObject spritePictureFrame;

    public static int destructionTimer = 15;
    public static int turnsSurvived = 0;
    public int totalPopulation, malePopulation, femalePopulation;
    public static Tile[,,] world;

    public LinkedList<Tile> townTiles;
    public LinkedList<Metro> metros;
    public ResourceHolder resources;
    public Sprite townSprite, roadSprite;

    float scale = 5;
    float randomGenOffset;

    public int displayedLayer;
    public static int[] atmosphericTemperature;
    

    void InitializeVariables()
    {
        resources = new ResourceHolder();
        resources.SetToStartup();

        world = new Tile[layers, mapSize, mapSize];
        townTiles = new LinkedList<Tile>();

        totalPopulation = 0;
        malePopulation = 0;
        femalePopulation = 0;
    }

    public bool IsControlled(int layer, int xPos, int yPos)
    {
        LinkedListNode<Metro> node = metros.First;
        Tile tile = world[layer, xPos, yPos];

        for (int i = 0; i < metros.Count; i++)
        {
            if (node.Value.controlledTiles.Contains(tile))
            {
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

        InitializeTileTemps();
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

    void InitializeTileTemps()
    {
        atmosphericTemperature = new int[layers + 1];
        atmosphericTemperature[layers] = 90;
        for (int i = (layers - 1); i >= 0; i--)
        {
            atmosphericTemperature[i] = (atmosphericTemperature[i + 1] - 9);
        }
        UpdateTemp();
    }

    void DebugAtmosphericTempAndCoreTemp()
    {
        for (int i = 0; i < 30; i++)
        {
            Debug.Log("TURN " + turnsSurvived);
            UpdateAtmosphericTempAndCoreTemp();
            turnsSurvived++;
            Debug.Log("=======================");
        }
    }

    void UpdateAtmosphericTempAndCoreTemp()
    {
        if (IsEndOfTimes())
        {
            int maxDifferenceBetweenLayers = 25;
            int randomChangeValue = Random.Range(10, 25);
            for (int i = atmosphericTemperature.Length - 1; i >= 0; i--)
            {
                Debug.Log("Layer " + i + ", temp: " + atmosphericTemperature[i]);
                if ( (i != 0) && (atmosphericTemperature[i] <= (atmosphericTemperature[i-1] - maxDifferenceBetweenLayers)))
                {
                    continue;
                }
                atmosphericTemperature[i] -= randomChangeValue;
                //Debug.Log(randomChangeValue);
                float fractionalNumerator = Random.Range(layers / 3f, layers);
                randomChangeValue = (int) (randomChangeValue * (fractionalNumerator/layers));
                //Debug.Log(randomChangeValue);
            }
        }
    }

    void SetTileTempsOnLayer(int layer)
    {
        int AvgAtmoTemp = atmosphericTemperature[layer];
        int AvgAtmoAbovTemp = atmosphericTemperature[layer + 1];

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                Tile tile = world[layer,i, j];
                if(tile == null)
                {
                    Debug.Log("Null Tile");
                    continue;
                }
                tile.HandleTemperature(AvgAtmoTemp, AvgAtmoAbovTemp);
                HandleColdTemperature(tile);
            }
        }
    }

    void UpdateTemp()
    {
        UpdateAtmosphericTempAndCoreTemp();
        for (int i = (layers - 1); i >= 0; i--)
        {
            SetTileTempsOnLayer(i);
        }
    }

    void InitialSetTopLayerTile(float perlinValue, Tile tile)
    {
        tile.SetSprite(null);
        if (perlinValue < .3)
        {
            //Water
            tile.type = Tile.TileType.Water;
            tile.SetOriginalSprite(tiles[0]);
        }
        else
        {
            int roll = Random.Range(0, 11);

            if(roll >= 10)
            {
                tile.type = Tile.TileType.Tree;
                tile.SetOriginalSprite(tiles[5]);
            }
            else if(roll >= 9)
            {
                tile.type = Tile.TileType.Shrub;
                tile.SetOriginalSprite(tiles[6]);
            }
            else
            {
                tile.type = Tile.TileType.Grass;
                tile.SetOriginalSprite(tiles[1]);
            }
            
        }
    }

    void InitialSetUndergroundLayers(float perlinValue, Tile tile)
    {
        tile.SetSprite(null);
        if (perlinValue < .3 || perlinValue > .9)
        {
            tile.type = Tile.TileType.Coal;
            tile.SetOriginalSprite(tiles[3]);
        }
        else if((perlinValue > .65 && perlinValue < .8))
        {
            tile.type = Tile.TileType.Ore;
            tile.SetOriginalSprite(tiles[4]);
        }
        else
        {
            tile.type = Tile.TileType.Stone;
            tile.SetOriginalSprite(tiles[2]);
        }
    }

    void MakeMetros()
    {
        metros = new LinkedList<Metro>();
        ResetTownMetroStatus();

        //Debug.Log("THIS MANY TOWN Tiles in GameManager: " + townTiles.Count);
        LinkedListNode<Tile> head = townTiles.First;
        for (int i = 0; i < townTiles.Count; i++)
        {
            if(head.Value.town.inMetro == false)
            {
                head.Value.town.inMetro = true;
                Metro metro = new Metro();
                metros.AddLast(metro);
                metro.AddTownTile(head.Value);
            }

            head = head.Next;
        }
        //Debug.Log("Metro Count: " + metros.Count);
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

    public bool PlaceTown(int layer, int xLocation, int yLocation, int level)
    {
        Town town = new Town(layer, xLocation, yLocation, level);
        Tile tile = world[layer, xLocation, yLocation];

        if ((tile.type == Tile.TileType.Water))
        {
            return false;
        }

        if (!resources.GreaterThanOrEqualTo(town.buildingCost))
        {
            return false;
        }

        resources.Subtract(town.buildingCost);

        tile.SetTown(town);
        townTiles.AddLast(world[layer, xLocation, yLocation]);

        UpdateBlock(tile);

        MakeMetros();
        return true;
    }

    //Overriding problem ALSO UPGRADES
    public bool PlaceRoad(int layer, int xLocation, int yLocation, int tech)
    {
        Tile tile = world[layer, xLocation, yLocation];

        if( (tile.type == Tile.TileType.Water) && tech == 0)
        {
            return false;
        }

        tile.PlaceRoad(tech);

        if (!resources.GreaterThanOrEqualTo(tile.road.buildingCost))
        {
            //Debug.Log("Cost Too Much For Roads");
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
        Debug.Log(indexOfImprovement);
        Improvement improvement = possibleImprovements[indexOfImprovement].techLevel[techOfImprovement];

        if (!improvement.ContainsTileType(tile.type))
        {
            return false;
        }

        if (!resources.GreaterThanOrEqualTo(improvement.buildingCost))
        {
            return false;
        }

        resources.Subtract(improvement.buildingCost);

        tile.SetImprovement(improvement);

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
        GameObject newObject = Instantiate(tile.originalSprite, tile.spawnedObject.transform.position, Quaternion.identity, layerParents[tile.layer].transform);
        Destroy(tile.PopSpawnedObject());

        SpriteLocationHolder slh = newObject.AddComponent<SpriteLocationHolder>();
        slh.layer = tile.layer;
        slh.xPos = tile.xPos;
        slh.yPos = tile.yPos;

        if (tile.sprite != null)
        {
            GameObject sf = Instantiate(spritePictureFrame, newObject.transform.position + new Vector3(0,0,-.51f), Quaternion.identity,newObject.transform);
            SpriteRenderer sr = sf.GetComponent<SpriteRenderer>();
            sr.sprite = tile.sprite;
            Debug.Log(sf);
        }

        tile.spawnedObject = newObject;
    }

    void DetermineTileRepresentation(Tile tile)
    {
        if(tile.town != null)
        {
            tile.SetSprite(townSprite); //NEED TO CHANGE
        }
        else if (tile.improvement != null && tile.road != null)
        {
            tile.SetSprite(tile.improvement.roadSprite);
        }
        else if (tile.improvement != null)
        {
            tile.SetSprite(tile.improvement.sprite);
        }
        else if (tile.road != null)
        {
            tile.SetSprite(roadSprite);
        }
        else
        {
            tile.SetSprite(null);
        }
    }

    public Metro FindTownsMetro(Tile t)
    {
        LinkedListNode<Metro> metroNode = metros.First;

        for (int i = 0; i < metros.Count; i++)
        {
            LinkedList<Tile> towns = metroNode.Value.townTiles;

            if (towns.Contains(t))
            {
                return metroNode.Value;
            }
        }
        return null;
    }

    public LinkedList<Metro> GetRankedMetros()
    {
        //first is most open jobs
        LinkedListNode<Metro> metroNode = metros.First;
        LinkedList<Metro> rankedMetros = new LinkedList<Metro>();

        if(metroNode.Value == null)
        {
            return null;
        }

        for (int i = 0; i < metros.Count; i++)
        {
            LinkedListNode<Metro> temp = rankedMetros.First;
            int metroNodeOpenJobs = metroNode.Value.DetermineOpenJobs();
            while( (temp != null) && (temp.Value.DetermineOpenJobs() > metroNodeOpenJobs))
            {
                temp = temp.Next;
            }

            if(temp == null)
            {
                rankedMetros.AddLast(metroNode.Value);
            }
            else
            {
                rankedMetros.AddBefore(temp, metroNode.Value);
            }
            
            metroNode = metroNode.Next;
        }

        return rankedMetros;
    }

    void RelocateDisplacedPeoples(Town town)
    {
        LinkedList<Metro> metrosRankedByJobs = GetRankedMetros();
        LinkedListNode<Metro> metroRankedNode = metrosRankedByJobs.First;

        LinkedList<Pop> pops = town.inhabitants;
        int citizensLeftToRelocate = pops.Count;

        float[] weights = GetWeightedMetrosProbabilities(metrosRankedByJobs);
        int[] roughAllocation = GetNewPopLocationDistributionFromWeights(weights, citizensLeftToRelocate);
        Debug.Log(weights.Length + "GRRR");
        //First Wave Of Migration
        for (int i = 0; i < roughAllocation.Length; i++)
        {
            LinkedList<Pop> groupOfPops = new LinkedList<Pop>();
            for (int k = 0; k < roughAllocation[i]; k++)
            {
                groupOfPops.AddLast(pops.First.Value);
                pops.Remove(pops.First.Value);
            }

            metroRankedNode.Value.HandlePopulationGrowth(roughAllocation[i], groupOfPops);
            metroRankedNode = metroRankedNode.Next;
        }

        LinkedListNode<Metro> node = metrosRankedByJobs.First;
        for (int i = 0; i < metrosRankedByJobs.Count; i++)
        {
            if(pops.Count == 0)
            {
                break;
            }
            node.Value.HandlePopulationGrowth(pops.Count, pops);

            node = node.Next;
        }

    }

    float[] GetWeightedMetrosProbabilities(LinkedList<Metro> weightedMetros)
    {
        if(weightedMetros == null)
        {
            Debug.LogError("weightedMetros SHOULDN'T BE NULL");
            return null;
        }
        float[] weights = new float[weightedMetros.Count];
        int totalJobs = GetTotalMetroJobs();

        LinkedListNode<Metro> weightedMetroNode = weightedMetros.First;
        for (int i = 0; i < weightedMetros.Count; i++)
        {
            if(totalJobs != 0)
            {
                weights[i] = ((float)weightedMetroNode.Value.DetermineOpenJobs()) / ((float)totalJobs);
            }
            else
            {
                weights[i] = 1f / weightedMetros.Count;
            }
            
            weightedMetroNode = weightedMetroNode.Next;
        }
        return weights;
    }

    int[] GetNewPopLocationDistributionFromWeights(float[] weights, int migratingPops)
    {
        if(weights == null || migratingPops == 0)
        {
            Debug.LogError("Weights is null or MigratingPops Is 0");
            return null;
        }
        int[] generalRelocationWithLittleLoss = new int[weights.Length];
        for (int i = 0; i < weights.Length; i++)
        {
            int peopleOnThisIndex = (int) ((weights[i]) * ((float)migratingPops));
            generalRelocationWithLittleLoss[i] = peopleOnThisIndex;
        }
        return generalRelocationWithLittleLoss;
    }

    int GetTotalMetroJobs()
    {
        LinkedListNode<Metro> metroNode = metros.First;
        int totalJobs = 0;
        for (int i = 0; i < metros.Count; i++)
        {
            totalJobs += metroNode.Value.DetermineOpenJobs();

            metroNode = metroNode.Next;
        }
        return totalJobs;
    }

    public void RemoveTown(int layer, int xPos, int yPos)
    {
        if(townTiles.Count == 1)
        {
            //Can't Delete Last Town
            return;
        }

        Tile townTile = world[layer, xPos, yPos];
        Town town = townTile.town;

        townTiles.Remove(townTile);
        townTile.RemoveTown();
        MakeMetros();

        RelocateDisplacedPeoples(town);
        townTile.RemoveTown();
        townTiles.Remove(townTile);

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

    void SpawnLayer(int layer)
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                Tile tile = world[layer, i, j];
                Vector3 location = new Vector3(i, j, -layer);
                GameObject spawnedObject = Instantiate(tile.originalSprite,location,Quaternion.identity,layerParents[layer].transform);
                tile.spawnedObject = spawnedObject;
                SpriteLocationHolder slh = spawnedObject.AddComponent<SpriteLocationHolder>();
                slh.layer = layer;
                slh.xPos = i;
                slh.yPos = j;
            }
        }
    }

    void RemoveBlock(int layer, int xPos, int yPos)
    {
        Tile tile = world[layer, xPos, yPos];

        if(tile == null)
        {
            Debug.Log("Can't Remove This block");
            return;
        }

        GameObject temp = tile.PopSpawnedObject();
        Debug.Log(temp);
        world[layer, xPos, yPos] = null;

        Destroy(temp);
        tile = null;

        UpdateBlock(world[layer-1, xPos, yPos]);
    }

    public void Mine(Tile tile)
    {
        if (tile.layer == 0 || tile.improvement.workers == 0)
        {
            //Nothing
            return;
        }

        bool canDestroyLayer = tile.DetermineWhetherToDestroyLayer();

        if (!canDestroyLayer)
        {
            //Nothing As Well
        }
        else if ((world[tile.layer - 1, tile.xPos, tile.yPos].improvement != null) || (world[tile.layer - 1, tile.xPos, tile.yPos].town != null))
        {
            //Destroy This Improvement
            RemoveBlock(tile.layer, tile.xPos, tile.yPos);

        }
        else if ((tile.layer - 1 >= 0) && (GameManager.world[tile.layer - 1, tile.xPos, tile.yPos].improvement == null) && (world[tile.layer - 1, tile.xPos, tile.yPos].town == null))
        {
            //Improvement Falls A Level
            tile.improvement.Reset();
            world[tile.layer - 1, tile.xPos, tile.yPos].improvement = tile.improvement;
            RemoveBlock(tile.layer, tile.xPos, tile.yPos);
        }
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

    public void UpdatePopulationValues()
    {
        if (metros == null)
        {
            Debug.Log("Still In Startup");
            return;
        }

        LinkedListNode<Metro> node = metros.First;
        
        int tp = 0;
        int mp = 0;
        int fp = 0;

        for (int i = 0; i < metros.Count; i++)
        {
            tp += node.Value.GetTotalPopulation();
            fp += node.Value.GetTotalGrowth();
        }
        mp = tp - fp;

        totalPopulation = tp;
        malePopulation = mp;
        femalePopulation = fp;
    }

    public void ExecuteTurn()
    {
        //THIS IS GOOD
        MakeMetros();
        PopJobFinder();
        HandleResources();
        PopulationGrowth();
        HandleStarvation();
        PopJobFinder();
        UpdateTemp();
        turnsSurvived++;
    }

    public int DetermineUnfedPops()
    {
        int perishableFood = resources.perFood;
        int cannedFood = resources.food;
        int starvingPeople = totalPopulation;

        if(perishableFood >= starvingPeople)
        {
            perishableFood -= starvingPeople;
            resources.perFood = perishableFood;
            return 0;
        }

        starvingPeople -= perishableFood;
        perishableFood = 0;

        if (cannedFood >= starvingPeople)
        {
            cannedFood -= starvingPeople;
            resources.food = cannedFood;
            return 0;
        }

        starvingPeople -= cannedFood;
        cannedFood = 0;

        resources.perFood = perishableFood;
        resources.food = cannedFood;

        return starvingPeople;
    }
    /*TODO:
     * Add UI elements to see growth and what not
     */
    public void HandleStarvation()
    {
        int starvingPops = DetermineUnfedPops();

        LinkedListNode<Metro> metroNode = metros.First;

        int starvingPopsRemainder = starvingPops % metros.Count;
        int starvingPopsDivided = starvingPops / metros.Count;
        for (int i = 0; i < metros.Count; i++)
        {
            Metro metro = metroNode.Value;

            int changeInPopulation = metro.HandlePopulationGrowth(starvingPopsDivided * -1, null);
            starvingPopsRemainder += (starvingPopsDivided + changeInPopulation);

            metroNode = metroNode.Next;
        }
        bool foundAPerson = true;
        while(starvingPopsRemainder > 0)
        {
            if(metroNode == null)
            {
                metroNode = metros.First;
                if(foundAPerson == false)
                {
                    break;
                }
                foundAPerson = false;
            }

            if( metroNode.Value.HandlePopulationGrowth(-1, null) != 0)
            {
                foundAPerson = true;
            }

            starvingPopsRemainder--;
            metroNode = metroNode.Next;
        }

        UpdatePopulationValues();
    }

    public void HandleColdTemperature(Tile tile)
    {
        if(tile.temperature < 32)
        {
            if (tile.town != null)
            {
                int inhabitantsToDie = -tile.town.inhabitants.Count;
                tile.town.ModifyPopulation(inhabitantsToDie, null);
            }
        }
    }

    public void TryMining(Tile tile)
    {
        if (tile.improvement != null && tile.improvement.turnsToDestroyTile != -1)
        {
            Mine(tile);
        }
    }

    //Puts Pops At Jobs
    public void PopJobFinder()
    {
        LinkedList<Metro> metro = metros;
        LinkedListNode<Metro> node = metro.First;

        for (int i = 0; i < metro.Count; i++)
        {
            node.Value.AllocatePops();
        }
    }

    public void PopulationGrowth()
    {
        LinkedListNode<Metro> node = metros.First;

        for (int i = 0; i < metros.Count; i++)
        {
            node.Value.HandlePopulationGrowth(node.Value.GetTotalGrowth(), null);

            node = node.Next;
        }

        UpdatePopulationValues();
    }

    public static bool IsEndOfTimes()
    {
        if(turnsSurvived > destructionTimer)
        {
            return true;
        }
        return false;
    }

    void DetermineResourceGain(bool isRefining)
    {
        LinkedListNode<Metro> node = metros.First;
        //Debug.Log("Metros: " + metros.Count);
        for (int i = 0; i < metros.Count; i++)
        {
            LinkedList<Tile> temp = node.Value.workplaces;
            LinkedListNode<Tile> secondNode = temp.First;

            Debug.Log("Workplaces: " + temp.Count);
            Debug.Log("Perishable Food: " + resources.perFood);
            for (int j = 0; j < temp.Count; j++)
            {
                for (int k = 0; k < secondNode.Value.improvement.workers; k++)
                {
                    secondNode.Value.improvement.Harvest(resources, secondNode.Value.type, isRefining);
                }
                TryMining(secondNode.Value);
                secondNode = secondNode.Next;
            }
            node = node.Next;
        }
    }

    void HandleResources()
    {
        DetermineResourceGain(false);
        DetermineResourceGain(true);
    }

    private void Start()
    {
        InitializeVariables();
        WorldSetup();
        SpawnWorld();
    }
}
