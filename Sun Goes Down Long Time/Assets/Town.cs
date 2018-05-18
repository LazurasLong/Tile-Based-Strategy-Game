using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Town {

    public int layer, xPos, yPos;
    public LinkedList<Pop> inhabitants;
    public int maxPop;
    public bool inMetro = false;

    public int level;
    public ResourceHolder buildingCost;

    public Town(int layerV, int xPosV, int yPosV,int levelV)
    {
        inhabitants = new LinkedList<Pop>();
        level = levelV;

        layer = layerV;
        xPos = xPosV;
        yPos = yPosV;

        DetermineBuildingCost();
    }

    public int ModifyPopulation(int addedPops, LinkedList<Pop> popsToAdd)
    {
        int space = (maxPop - inhabitants.Count);

        int smallerValue;
        int leftovers = 0;

        if(space < addedPops)
        {
            smallerValue = space;
            leftovers = addedPops - space;
        }
        else
        {
            smallerValue = addedPops;
        }

        smallerValue = Mathf.Abs(smallerValue);
        for (int i = 0; i < smallerValue; i++)
        {
            if(popsToAdd == null)
            {
                if(addedPops >= 0)
                {
                    NewChildPop();
                }
                else
                {
                    RemoveNewestPop();
                }
            }
            else
            {
                AddPop(popsToAdd.First.Value);
                popsToAdd.RemoveFirst();
            }
        }

        if(addedPops < 0)
        {
            if(inhabitants.Count > 0)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        return leftovers;
    }

    public void NewChildPop()
    {
        int rng = Random.Range(0, 11);

        Pop.Sex sex = Pop.DetermineSex(rng);
        int age = 0;

        Pop pop = new Pop(sex,age);
        AddPop(pop);
    }

    public void RemoveNewestPop()
    {
        if(inhabitants.Count == 0)
        {
            return;
        }
        Pop newestPop = inhabitants.Last.Value;
        newestPop.LeaveJob();
        inhabitants.RemoveLast();
    }

    public void FillTown()
    {
        for (int i = 0; i < maxPop; i++)
        {
            NewChildPop();
        }
    }

    //FIGURE OUT BIRTH RATE
    void DetermineBuildingCost()
    {
        buildingCost = new ResourceHolder();
        if(level == -2)
        {
            maxPop = 50;
            FillTown();
        }
        else if(level == 0)
        {
            buildingCost.wood = 2;
            maxPop = 50;
        }
        else if(level == 1)
        {
            buildingCost.wood = 2;
            buildingCost.stoneBrick = 2;
            maxPop = 100;
        }
        else if (level == 2)
        {
            buildingCost.stoneBrick = 2;
            buildingCost.oreBars = 2;
            maxPop = 200;
        }
        else if (level == 3)
        {
            buildingCost.wood = 5;
            buildingCost.stoneBrick = 5;
            buildingCost.oreBars = 5;
            maxPop = 800;
        }
        else
        {
            Debug.LogError("This Town Level Does Not Exist");
        }
    }

    public int DetermineBirthRate()
    {
        int br = 0;
        LinkedListNode<Pop> head = inhabitants.First;

        for (int i = 0; i < inhabitants.Count; i++)
        {
            if(head.Value.gender == Pop.Sex.female)
            {
                br++;
            }

            head = head.Next;
        }
        return br;
    }

    public bool AddPop(Pop person)
    {
        if(inhabitants.Count < maxPop)
        {
            inhabitants.AddLast(person);
            return true;
        }
        return false;
    }
}
