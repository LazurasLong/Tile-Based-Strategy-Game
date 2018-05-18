using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour {

    public Canvas TechCanvas;
    public Canvas BuildCanvas;

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

    private void Start()
    {
        TechCanvas.gameObject.SetActive(false);
        BuildCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        
    }

    public void TechMenu()
    {
        if (TechCanvas.gameObject.activeSelf)
        {
            TechCanvas.gameObject.SetActive(false);
        }
        else if(!BuildCanvas.gameObject.activeSelf)
        {
            TechCanvas.gameObject.SetActive(true);
        }
    }

    public void BuildMenu()
    {
        if (BuildCanvas.gameObject.activeSelf)
        {
            BuildCanvas.gameObject.SetActive(false);
        }
        else if(!TechCanvas.gameObject.activeSelf)
        {
            BuildCanvas.gameObject.SetActive(true);
        }
    }

    public void SetPlayerStateBuild()
    {
        PlayerController player = FindObjectOfType<PlayerController>();

        player.state = PlayerController.Player_State.Build;
        player.selectedConstructableImprovementIndex = 0; //Change number for different improvements
    }

    public void SetPlayerStateSettle()
    {
        PlayerController player = FindObjectOfType<PlayerController>();

        player.state = PlayerController.Player_State.Settle;
    }

    public void SetPlayerStateConnect()
    {
        PlayerController player = FindObjectOfType<PlayerController>();

        player.state = PlayerController.Player_State.Connect;
    }

    public void DestroyBuildingMode()
    {
        PlayerController player = FindObjectOfType<PlayerController>();

        player.state = PlayerController.Player_State.DestroyBuilding;
    }

    public void DestroyRoadMode()
    {
        PlayerController player = FindObjectOfType<PlayerController>();

        player.state = PlayerController.Player_State.DestroyRoad;
    }

    public void NextTurn()
    {
        PlayerController player = FindObjectOfType<PlayerController>();

        player.EndTurn();
    }

}
