using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road {
    public bool wasChecked = false;
    public ResourceHolder buildingCost;
    public int tech;

    public Road(int techV)
    {
        buildingCost = new ResourceHolder();
        buildingCost.stoneBrick = 1;
        buildingCost.wood = 1;
        tech = techV;
    }
}
