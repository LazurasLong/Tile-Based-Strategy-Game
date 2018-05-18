using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Runtime.InteropServices;


public class Temperature : MonoBehaviour {

    int[,,] TempMap;
    int[,,] NewTemp;
    GameManager GM;
    // Use this for initialization
    void Start () {
        GM = FindObjectOfType<GameManager>();
        TempMap = new int[GameManager.layers, GameManager.mapSize, GameManager.mapSize];
        NewTemp = new int[GameManager.layers, GameManager.mapSize, GameManager.mapSize];
        //GM.world[Layer, x, y].temprature;
        //GameManager.mapSize;
        //GameManager.layers;
    }

    // DO this once at the begenning if you want the TempMap to take the world temps.
    // Extract from the blocks
    void ExtractTemp()
    {
        int x, y, z;
        for (z = 0; z < GameManager.layers; z++)
        {
            for(x=0; x < GameManager.mapSize; x++)
            {
                for (y = 0; y < GameManager.mapSize; y++)
                {
                    if (GameManager.world[z, x, y] != null)
                    {
                        TempMap[z, x, y] = 10 * GameManager.world[z, x, y].temperature;
                    }
                    else
                    {
                        TempMap[z, x, y] = -1;
                    }
                }
            }
        }

    }

    void InsertTemp()
    {

    }
    
    void AirUpdate()
    {

    }

	
    /*
	// Update is called once per frame
	void Update () {
		
	}
    */
}
