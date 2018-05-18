using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourceHolder{

    [SerializeField]
    public int dirt;
    [SerializeField]
    public int coal;
    [SerializeField]
    public int wood;
    [SerializeField]
    public int rawOre;
    [SerializeField]
    public int oreBars;
    [SerializeField]
    public int perFood;
    [SerializeField]
    public int food;
    [SerializeField]
    public int stone;
    [SerializeField]
    public int stoneBrick;
    [SerializeField]
    public int power;
    [SerializeField]
    public int tech;

    public ResourceHolder()
    {
        dirt = 0;
        coal = 0;
        wood = 0;
        rawOre = 0;
        oreBars = 0;
        perFood = 0;
        food = 0;
        stone = 0;
        stoneBrick = 0;
        power = 0;
    }

    //CHANGE THIS
    public void SetToStartup()
    {
        dirt = 0;
        coal = 0;
        wood = 10;
        rawOre = 0;
        oreBars = 0;
        perFood = 0;
        food = 100;
        stone = 0;
        stoneBrick = 8;
        power = 0;
    }

    //Tech Problem
    public void Subtract(ResourceHolder resources)
    {
        dirt -= resources.dirt;
        coal -= resources.coal;
        wood -= resources.wood;
        rawOre -= resources.rawOre;
        oreBars -= resources.oreBars;
        perFood -= resources.perFood;
        food -= resources.food;
        stone -= resources.stone;
        stoneBrick -= resources.stoneBrick;
        power -= resources.power;
    }

    public void Add(ResourceHolder resources)
    {
        dirt += resources.dirt;
        coal += resources.coal;
        wood += resources.wood;
        rawOre += resources.rawOre;
        oreBars += resources.oreBars;
        perFood += resources.perFood;
        food += resources.food;
        stone += resources.stone;
        stoneBrick += resources.stoneBrick;
        power += resources.power;
    }

    public void Copy(ResourceHolder resources)
    {
        dirt = resources.dirt;
        coal = resources.coal;
        wood = resources.wood;
        rawOre = resources.rawOre;
        oreBars = resources.oreBars;
        perFood = resources.perFood;
        food = resources.food;
        stone = resources.stone;
        stoneBrick = resources.stoneBrick;
        power = resources.power;
    }

    public bool GreaterThanOrEqualTo(ResourceHolder other)
    {
        if ( (this.dirt >= other.dirt) && (this.coal >= other.coal) && (this.wood >= other.wood) && (this.rawOre >= other.rawOre) && (this.oreBars >= other.oreBars) && (this.perFood >= other.perFood) &&  (this.food >= other.food) && (this.stone >= other.stone) && (this.stoneBrick >= other.stoneBrick) && (this.power >= other.power))
        {
            return true;
        }
        return false;
    }
    

}
