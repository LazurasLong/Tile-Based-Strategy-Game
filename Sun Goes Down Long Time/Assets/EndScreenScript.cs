using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenScript : MonoBehaviour {

    public Text TurnsSurv;
    public Text DestTimer;
    public Text MapSize;
    public Text Score;
	// Use this for initialization
	void Start () {
        int i;
        int iR;
        int Temp;
        //GameManager.turnsSurvived = 17;
        //Print the turns survived, destruction timer, and map size.
        TurnsSurv.text = "Turns Survived: " + GameManager.turnsSurvived;
        DestTimer.text = "Sunny Days: " + GameManager.destructionTimer;
        MapSize.text = "Map Size: " + GameManager.mapSize;
        //Calculate score
        i = (GameManager.destructionTimer + GameManager.mapSize) / 2;
        Temp = GameManager.turnsSurvived * 7;
        iR = Temp % i;
        i = Temp / i;
        i = i * 7;
        i += iR;
        //Print score
        Score.text = "Final Score: " + i;
        //TExt.text = "Text: " + GameManager.fff;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
