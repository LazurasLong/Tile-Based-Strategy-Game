using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TechTree {
    [SerializeField]
    public Improvement[] techLevel;
    
    public int getTechNum()
    {
        return techLevel.Length;
    }
}
