using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Improvement {
    
    public ResourceHolder buildingCost;
    public ResourceHolder required;
    public ResourceHolder output;
    public int maxWorkers;
    public int workers = 0;
    public int turnsToDestroyTile;
    public int turnInstantiated;
    Pop[] workersArray;
    bool harvestedLastTurn;

    public Improvement(ResourceHolder requiredV, ResourceHolder outputV, int maxWorkersV)
    {
        required = requiredV;
        output = outputV;
        maxWorkers = maxWorkersV;
        harvestedLastTurn = true;

        turnInstantiated = GameManager.turnsSurvived;
    }
    
    //return true if person added, false otherwise.
    bool AddWorker(Pop person)
    {
        for (int i = 0; i < workersArray.Length; i++)
        {
            if (workersArray[i].Equals(person))
            {
                workers++;
                workersArray[i] = person;
                return true;
            }
        }
        return false;
    }

    //Returns true if successful and false otherwise
    bool RemoveWorker(Pop person)
    {
        for (int i = 0; i < workersArray.Length; i++)
        {
            if (workersArray[i].Equals(person))
            {
                workers--;
                workersArray[i] = null;
                return true;
            }
        }
        return false;
    }

    public bool RequirementMet(ResourceHolder resources)
    {
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

    public bool Harvest(ResourceHolder resources)
    {
        if (RequirementMet(resources))
        {
            Debug.Log("Resources Meet Rquirement");
            resources.Subtract(required);
            resources.Add(output);
            harvestedLastTurn = true;
            return true;
        }
        Debug.Log("Resources DONT Meet Rquirement");
        harvestedLastTurn = false;
        return false;
    }

}
