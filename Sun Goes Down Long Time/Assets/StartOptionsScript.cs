using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartOptionsScript : MonoBehaviour {

    //UI 
    public Dropdown QDown;
    public Dropdown SizeDown;
    public Slider GlobalVolume;
    // The values in the destruction dropdown
    int DA = 15;
    int DB = 20;
    int DC = 30;
    // The values in the size dropdown
    int SA = 10;
    int SB = 15;
    int SC = 25;
    // Use this for initialization
    void Start () {
        //Vars
        int Q = GameManager.destructionTimer;
        int Size = GameManager.mapSize;

        GlobalVolume.value = AudioListener.volume;
        //Destruction Timer Initialization
        if (Q==DA)
        {
            QDown.value = 0;
        }
        else if (Q==DB)
        {
            QDown.value = 1;
        }
        else if (Q==DC)
        {
            QDown.value = 2;
        }
        else
        {
            Debug.LogError("Destruction Timer value impossible. Check the dropdown or StartOptionsScript.");
            QDown.value = 0;
            GameManager.destructionTimer = DA;
        }

        //Map Size Initialization
        if (Size == SA)
        {
            SizeDown.value = 0;
        }
        else if (Size == SB)
        {
            SizeDown.value = 1;
        }
        else if (Size == SC)
        {
            SizeDown.value = 2;
        }
        else
        {
            Debug.LogError("Map Size value impossible. Check the dropdown or StartOptionsScript.");
            SizeDown.value = 0;
            GameManager.mapSize = SA;
        }
    }

    //Destruction Drop Down
    public void DropDestDown()
    {
        if(QDown.value == 0)
        {
            GameManager.destructionTimer = DA;
        }
        if (QDown.value == 1)
        {
            GameManager.destructionTimer = DB;
        }
        if (QDown.value == 2)
        {
            GameManager.destructionTimer = DC;
        }
    }

    //Size drop down 
    public void DropSizeDown()
    {
        if (SizeDown.value == 0)
        {
            GameManager.mapSize = SA;
        }
        if (SizeDown.value == 1)
        {
            GameManager.mapSize = SB;
        }
        if (SizeDown.value == 2)
        {
            GameManager.mapSize = SC;
        }
    }

    //Volume
    public void VolumeChange()
    {
        AudioListener.volume = GlobalVolume.value;
    }
}
