using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metro {

    public LinkedList<Tile> townTiles;
    public LinkedList<Tile> controlledTiles;
    public LinkedList<Tile> tilesWithRoads;

    public LinkedList<Tile> workplaces;

    public bool isStarving;
    public bool populationMaxed;

    public Metro()
    {
        townTiles = new LinkedList<Tile>();
        controlledTiles = new LinkedList<Tile>();
        workplaces = new LinkedList<Tile>();
        isStarving = false;
        populationMaxed = false;
    }

    public void AddTownTile(Tile townTile)
    {
        townTiles.AddLast(townTile);
        DetermineControlledTiles();
    }

    //Gets the total population
    public int GetTotalPopulation()
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
        //Debug.Log("Head Tiles Length: " + townTiles.Count);
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

    void AddSurroundingTownTilesToControlledTiles(Tile pivot)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                addControlledTile(pivot, i,j);
            }
        }
    }

    void AddRoadTile(Tile pivot)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if( (i == 0 && j == 0) || (i == 0 && j == 2) || (i == 2 && j == 0) || (i == 2 && j == 2))
                {
                    continue;
                }

                addControlledTile(pivot,i,j);
            }
        }
    }

    void AddSurroundingRoadTilesToControlledTiles()
    {
        tilesWithRoads = new LinkedList<Tile>();
        LinkedListNode<Tile> head = controlledTiles.First;
        for (int i = 0; i < controlledTiles.Count; i++)
        {
            //Debug.Log("Growing Controlled Tiles From Roads" + controlledTiles.Count);
            if (head.Value.road != null && head.Value.road.wasChecked == false)
            {
                head.Value.road.wasChecked = true;
                tilesWithRoads.AddLast(head.Value);
                //Debug.Log("Tiles With Roads Length: " + tilesWithRoads.Count);
                AddRoadTile(head.Value);
            }

            head = head.Next;
        }
        ResetRoadTiles();
    }

    void ResetRoadTiles()
    {
        if (tilesWithRoads.Count == 0)
        {
            //Debug.Log("No Roads");
            return;
        }
        LinkedListNode<Tile> head = tilesWithRoads.First;
        for (int i = 0; i < tilesWithRoads.Count; i++)
        {
            head.Value.road.wasChecked = false;

            head = head.Next;
        }
    }

    void addControlledTile(Tile pivot, int i, int j)
    {
        int xLocation = (pivot.xPos - 1) + i;
        int yLocation = (pivot.yPos - 1) + j;

        if (xLocation < 0 || yLocation < 0 || xLocation >= GameManager.mapSize || yLocation >= GameManager.mapSize)
        {
            return;
        }

        Tile newTile = GameManager.world[pivot.layer, xLocation, yLocation];

        if (newTile == null || controlledTiles.Contains(newTile))
        {
            return;
        }

        if (newTile.town != null && newTile.town.inMetro == false)
        {
            //Debug.Log("THIS IS A TOWN THAT WAS FOUND");
            townTiles.AddLast(newTile);
            newTile.town.inMetro = true;
            AddSurroundingTownTilesToControlledTiles(newTile);
        }

        //Debug.Log("++++++++++ Location of Added Tile: (" + xLocation + ", " + yLocation + ")");

        controlledTiles.AddLast(newTile);
    }

    public void ResetMetroStatus()
    {
        LinkedListNode<Tile> head = townTiles.First;
        for (int i = 0; i < townTiles.Count; i++)
        {
            head.Value.town.inMetro = false;

            head = head.Next;
        }
    }

    void DetermineWorkplaces()
    {
        LinkedListNode<Tile> head = controlledTiles.First;

        //Debug.Log("You Control This Many Tiles: " + controlledTiles.Count);
        for (int i = 0; i < controlledTiles.Count; i++)
        {
            if (head.Value.improvement != null)
            {
                workplaces.AddLast(head.Value);
            }

            head = head.Next;
        }
    }

    public int DetermineOpenJobs()
    {
        int maxJobs = 0;
        int takenJobs = 0;

        LinkedListNode<Tile> head = workplaces.First;

        //Debug.Log("You Control This Many Tiles: " + controlledTiles.Count);
        for (int i = 0; i < workplaces.Count; i++)
        {
            maxJobs += head.Value.improvement.maxWorkers;
            takenJobs += head.Value.improvement.workers;
            head = head.Next;
        }
        return maxJobs - takenJobs;
    }

    public int GetTotalGrowth()
    {
        int totalGrowth = 0;
        LinkedListNode<Tile> head = townTiles.First;

        for (int i = 0; i < townTiles.Count; i++)
        {
            totalGrowth += head.Value.town.DetermineBirthRate();

            head = head.Next;
        }

        return totalGrowth;
    }
    //returns total growth
    public int HandlePopulationGrowth(int totalGrowth, LinkedList<Pop> addedPops)
    {
        bool changedPopulation = false;
        LinkedListNode<Tile> head = townTiles.First;

        bool isNegative;

        if(totalGrowth < 0)
        {
            isNegative = true;
        }
        else
        {
            isNegative = false;
        }

        int remainder = totalGrowth % (townTiles.Count);
        int actual = totalGrowth / townTiles.Count;

        if( (totalGrowth < 0) && (actual < 0))
        {
            Debug.Log("Weird Modulos");
        }
;
        //CHECK IF NEGATIVE IF GOES WRONG
        for (int i = 0; i < townTiles.Count; i++)
        {
            Town town = head.Value.town;

            int leftovers = town.ModifyPopulation(actual, addedPops);
            totalGrowth += (actual - leftovers); //actual - leftovers = growth
            remainder += leftovers;

            head = head.Next;
        }

        bool remainderChangedThisLoop = true;
        head = townTiles.First;
        while ( (remainder != 0))
        {
            if(head == null)
            {
                head = townTiles.First;

                if (!remainderChangedThisLoop)
                {
                    break;
                }

                remainderChangedThisLoop = false;
            }

            Town town = head.Value.town;

            int leftovers;
            if (isNegative)
            {
                leftovers = town.ModifyPopulation(-1, addedPops);
            }
            else
            {
                leftovers = town.ModifyPopulation(1, addedPops);
            }

            if(leftovers == 0)
            {
                if (isNegative)
                {
                    remainder++;
                    totalGrowth--;
                }
                else
                {
                    remainder--;
                    totalGrowth++;
                }
                remainderChangedThisLoop = true;
            }

            head = head.Next;
        }

        return totalGrowth;
    }

    public void DebugPopulation()
    {
        LinkedListNode<Tile> tNode = townTiles.First;
        for (int i = 0; i < townTiles.Count; i++)
        {
            //Debug.Log("Town Location: ( " + tNode.Value.xPos + ", " + tNode.Value.yPos + "), with Population = " + tNode.Value.town.inhabitants.Count);

            tNode = tNode.Next;
        }
    }

    public void AllocatePops()
    {
        LinkedListNode<Tile> townNode = townTiles.First;
        LinkedListNode<Tile> workplacesNode = workplaces.First;

        bool jobsStillExist = true;

        if (workplacesNode == null)
        {
            return;
        }

        for (int i = 0; i < townTiles.Count; i++)
        {
            LinkedList<Pop> popList = townNode.Value.town.inhabitants;
            LinkedListNode<Pop> popNode = popList.First;

            for (int k = 0; k < popList.Count; k++)
            {
                Pop pop = popNode.Value;

                if ((pop.workplace != null) && (workplaces.Contains(pop.workplace)))
                {
                    //Workplace is still access to pop

                }
                else
                {
                    //Needs a new job

                    if (pop.workplace != null)
                    {
                        Tile popsWork = pop.workplace;
                        if (popsWork != null)
                        {
                            popsWork.improvement.RemoveWorker(pop);
                        }
                        pop.workplace = null;
                    }

                    if (jobsStillExist == false)
                    {
                        continue;
                    }
                    //sequentially pick

                    bool foundValidWorkplace = false;

                    for (int l = 0; l < workplaces.Count; l++)
                    {
                        if (workplacesNode == null)
                        {
                            workplacesNode = workplaces.First;
                        }

                        if (workplacesNode.Value.improvement.OpenJob())
                        {
                            foundValidWorkplace = true;
                            break;
                        }

                        workplacesNode = workplacesNode.Next;
                    }

                    if (foundValidWorkplace == false)
                    {
                        //No Jobs Left
                        jobsStillExist = false;
                        return;
                    }

                    workplacesNode.Value.improvement.AddWorker(pop);
                    pop.workplace = workplacesNode.Value;

                    workplacesNode = workplacesNode.Next;
                }

                popNode = popNode.Next;
            }
            townNode = townNode.Next;
        }
    }

    //Returns the amount of dead

        /*
    public int VacateTown(Tile vacatedTownTile)
    {
        LinkedList<Pop> popLinkedList = vacatedTownTile.town.inhabitants;
        LinkedListNode<Pop> popNode = popLinkedList.First;
        townTiles.Remove(vacatedTownTile);
        LinkedListNode<Tile> head = townTiles.First;

        if(head == null || popNode == null)
        {
            Debug.Log("No Towns Left, They All Die Or No People");
            return tile.town.inhabitants.Count;
        }

        bool noRoomLeft = false;
        int peopleSaved = 0;
        while( (popNode != null) && (noRoomLeft == false))
        {
            if(popNode == null)
            {
                Debug.LogError("HOW DID THIS HA{EN");
            }
            Pop pop = popNode.Value;

            for (int i = 0; i < townTiles.Count; i++)
            {
                if (head == null)
                {
                    head = townTiles.First;
                    noRoomLeft = true;
                }

                Town town = head.Value.town;

                if (town.AddPop(pop))
                {
                    noRoomLeft = false;
                }

                head = head.Next;
            }
            peopleSaved++;
            popNode = popNode.Next;
        }

        return tile.town.inhabitants.Count - peopleSaved;
    }
    */
}
