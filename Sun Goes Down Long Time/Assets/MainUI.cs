using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour {
    //Main UI canvases
    public Canvas TechCanvas;
    public Canvas BuildCanvas;
    public Canvas DestroyCanvas;

    //UI text/info
    public Text Ore;
    public Text Food;
    public Text Wood;
    public Text Coal;
    public Text Stone;

    public Text Population;
    public Text Men;
    public Text Women;

    public Text DoomsdayClock;
    public Text Turn;
    public Text Dirt;

    //GameManager variable
    GameManager GM;
    PlayerController player;

    public Canvas[] techCs;
    public Button[] techBs; //Resize if ore techs added

    //Fuctions start here
    private void Start()
    {
        /* Tech*/
        TechCanvas.gameObject.SetActive(false);
        /* Build*/
        BuildCanvas.gameObject.SetActive(true);
        CloseLevels();
        BuildCanvas.gameObject.SetActive(false);
        /* Destroy*/
        DestroyCanvas.gameObject.SetActive(false);
        /* Other*/
        GM = FindObjectOfType<GameManager>();
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        UpdateUIText();
    }

    // updates all of the UI#'s
    void UpdateUIText()
    {
        ResourceHolder Resources = GM.resources;
        int DC;
        DC = GameManager.destructionTimer - GameManager.turnsSurvived;
        if (DC < 0) { DC = 0; }
        
        Ore.text = "" + Resources.oreBars;
        Food.text = "" + Resources.food;
        Wood.text = "" + Resources.wood;
        Coal.text = "" + Resources.coal;
        Stone.text = "" + Resources.stoneBrick;

        Population.text = "" + GM.totalPopulation;
        Men.text = "" + GM.malePopulation;
        Women.text = "" + GM.femalePopulation;

        DoomsdayClock.text = "" + DC;
        Turn.text = "" + GameManager.turnsSurvived;
        Dirt.text = "" + Resources.dirt;
    }

    /* Bottom right UI menu popups*/
    public void TechMenu()
    {
        if (TechCanvas.gameObject.activeSelf)
        {
            TechCanvas.gameObject.SetActive(false);
        }
        else
        {
            TechCanvas.gameObject.SetActive(true);
            BuildCanvas.gameObject.SetActive(false);
            DestroyCanvas.gameObject.SetActive(false);
            CloseLevels();
        }
    }

    public void BuildMenu()
    {
        if (BuildCanvas.gameObject.activeSelf)
        {
            BuildCanvas.gameObject.SetActive(false);
            CloseLevels();
        }
        else
        {
            TechCanvas.gameObject.SetActive(false);
            BuildCanvas.gameObject.SetActive(true);
            DisplayLevel(0);
            DestroyCanvas.gameObject.SetActive(false);
        }
    }

    public void DestroyMenu()
    {
        if (DestroyCanvas.gameObject.activeSelf)
        {
            DestroyCanvas.gameObject.SetActive(false);
        }
        else
        {
            TechCanvas.gameObject.SetActive(false);
            BuildCanvas.gameObject.SetActive(false);
            DestroyCanvas.gameObject.SetActive(true);
            CloseLevels();
        }
    }

    /* Opens up the proper build level menu*/
    public void CloseLevels()
    {
        for (int i = 0; i < techCs.Length; i++)
        {
            techCs[i].gameObject.SetActive(false);
        }
    }
    
    public void DisplayLevel(int levelIndex)
    {
        for (int i = 0; i < techCs.Length; i++)
        {
            if(i == levelIndex)
            {
                techCs[i].gameObject.SetActive(true);
                techBs[i].interactable = false;
            }
            else
            {
                techCs[i].gameObject.SetActive(false);
                techBs[i].interactable = true;
            }
        }
    }

    public void TechUp(int index)
    {
        int techLevel = player.DetermineNextTechLevelForImprovement(index);

        if(techLevel == -1)
        {

        }
        else
        {
            player.UnlockImprovement(index, techLevel);
        }
    }

    /* Matt's button shit. Look at that later.*/
    //Tech_index
    public void SetPlayerStateImprovement(string info)
    {
        int splitter = info.IndexOf("_");
        int techLevel = int.Parse(info.Substring(0, splitter)); //Can't Go Past 9
        int indexOfImprovement = int.Parse(info.Substring(splitter + 1));

        player.selectedTechLevel = techLevel;
        player.state = PlayerController.Player_State.Build;
        player.SelectBuildableImprovement(indexOfImprovement);
    }

    public void SetPlayerStateSettle(int tech)
    {

        player.selectedTechLevel = tech;
        player.state = PlayerController.Player_State.Settle;
    }

    public void SetPlayerStateConnect(int tech)
    {

        player.selectedTechLevel = tech;
        player.state = PlayerController.Player_State.Connect;
    }

    public void NextTurn()
    {
        player.EndTurn();
    }

}
