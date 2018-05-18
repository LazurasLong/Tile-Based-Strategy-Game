using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public enum Player_State
    {
        Default,
        Build,
        Settle,
        DestroyBuilding,
        Connect,
        DestroyRoad,
        Loading
    }

    //POINTLESS DELETE LATER
    public enum Selected_Type
    {
        Tile,
        Improvement,
        Nothing
    }

    public Player_State state;
    bool[,] unlockedImprovements; //boolean determining whether an improvement is unlocked HOUSMAN CHAN NOTICE ME
    bool[] colonizableLayers;
    public int selectedConstructableImprovementIndex;
    public int selectedTechLevel;
    GameManager gm;

    bool inStartup;

    GameObject selectedObject;
    Selected_Type selectedType;

    public void UnlockImprovement(int index,int techIndex)
    {
        unlockedImprovements[index,techIndex] = true;
    }

    public bool SelectBuildableImprovement(int index,int techIndex)
    {
        if(unlockedImprovements[index, techIndex] == false)
        {
            return false;
        }

        selectedConstructableImprovementIndex = index;
        return true;
    }

    void Build(int layer, int xLocation, int yLocation)
    {
        bool success;
        if (!gm.CheckIfClear(layer,xLocation,yLocation, state) || colonizableLayers[layer] == false)
        {
            ResetSelected();
            return;
        }

        if (state != Player_State.Settle && !gm.IsControlled(layer, xLocation, yLocation))
        {
            return;
        }

        if (state == Player_State.Build)
        {
            //FIGURE OUT WHAT TO DO IF IMPROVEMENT ALREADY THERE

            success = gm.PlaceImprovement(layer, xLocation, yLocation, selectedConstructableImprovementIndex,selectedTechLevel);
        }
        else if(state == Player_State.Settle)
        {
            success = gm.PlaceTown(layer, xLocation, yLocation);

            if(inStartup)
            {
                ResetSelected();
            }
        }
        else if(state == Player_State.Connect)
        {
            success = gm.PlaceRoad(layer, xLocation, yLocation);
        }
    }

    void DestroyThing(int layer, int xLocation, int yLocation)
    {
        if(state == Player_State.DestroyBuilding)
        {
            gm.RemoveImprovement(layer, xLocation, yLocation);
            gm.RemoveTown(layer, xLocation, yLocation);
        }
        else if(state == Player_State.DestroyRoad)
        {
            gm.RemoveRoad(layer, xLocation, yLocation);
        }
    }


    // FINISH THIS HTING LATER!!!
    void DisplayTileInfo()
    {

    }

    int[] GetSelectedTilePosition()
    {
        if(selectedObject == null)
        {
            return null;
        }
        SpriteLocationHolder slh = selectedObject.GetComponent<SpriteLocationHolder>();
        int[] location = new int[3];

        Debug.Log(slh.layer + "LAYER");
        Debug.Log(slh.xPos + "XPOS");
        Debug.Log(slh.yPos + " YPOS");
        location[0] = slh.layer;
        location[1] = slh.xPos;
        location[2] = slh.yPos;
        return location;
    }

    void Actions()
    {
        int[] tilePosition = GetSelectedTilePosition();
        if (tilePosition == null)
        {
            return;
        }

        if(state == Player_State.DestroyBuilding || state == Player_State.DestroyRoad)
        {
            DestroyThing(tilePosition[0], tilePosition[1], tilePosition[2]);
        }
        else if (state == Player_State.Settle || state == Player_State.Build || state == Player_State.Connect)
        {
            Build(tilePosition[0], tilePosition[1], tilePosition[2]);
        }
    }

    void InputHandler()
    {
        HandleMouseClick();
        ViewLayer();
        Actions();
    }

    void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(state);
            AdvancedSelect();
        }
    }

    void ViewLayer()
    {
        if (inStartup)
        {
            return;
        }


        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            gm.displayedLayer = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            gm.displayedLayer = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            gm.displayedLayer = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4))
        {
            gm.displayedLayer = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5))
        {
            gm.displayedLayer = 4;
        }
        else if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            gm.displayedLayer--;
            if(gm.displayedLayer < 0)
            {
                gm.displayedLayer = 0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            gm.displayedLayer++;
            if (gm.displayedLayer >= GameManager.layers)
            {
                gm.displayedLayer = GameManager.layers - 1;
            }
        }
        else
        {
            return;
        }
        ResetSelected();
        gm.DisplayLayer(gm.displayedLayer);
    }

    void ResetSelected()
    {
        selectedObject = null;
        selectedType = Selected_Type.Nothing;
        state = Player_State.Default;
        selectedConstructableImprovementIndex = -1;        
    }

    GameObject Select()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        GameObject temp = null;
        if (Physics.Raycast(ray, out hit))
        {
            temp = hit.transform.gameObject;
        }
        return temp;
    }

    void AdvancedSelect()
    {
        GameObject selected = Select();
        
        if(selected == null)
        {
            ResetSelected();
            state = Player_State.Default;
            return;
        }

        selectedObject = selected;

        if(state == Player_State.Default || state == Player_State.Build || state == Player_State.Settle || state == Player_State.Connect)
        {
            //Select Tile
            selectedType = Selected_Type.Tile;
        }
        else if(state == Player_State.DestroyBuilding || state == Player_State.DestroyRoad)
        {
            //Select Improvement
            selectedType = Selected_Type.Improvement;
        }
        else if(state == Player_State.Loading)
        {
            ResetSelected();
        }
    }

    void InitializeVariables()
    {
        ResetSelected();

        gm = FindObjectOfType<GameManager>();
        unlockedImprovements = new bool[gm.possibleImprovements.Length,4]; // Change to maximum tech value if it changes or something
        selectedConstructableImprovementIndex = -1;

        state = Player_State.Default;
        colonizableLayers = new bool[GameManager.layers];
        colonizableLayers[GameManager.layers - 1] = true;
        inStartup = true;
    }

    void UnlockLayer(int layer)
    {
        colonizableLayers[layer] = true;
    }

    void SetUp()
    {
        if(GameManager.turnsSurvived == 0 && gm.townTiles.Count == 0)
        {
            state = Player_State.Settle;
        }
        else
        {
            inStartup = false;
        }
    }

    public void EndTurn()
    {
        ResetSelected();
        state = Player_State.Loading;
        gm.ExecuteTurn();
        ResetSelected();
    }

    private void Start()
    {
        InitializeVariables();
    }

    private void Update()
    {
        SetUp();
        InputHandler();
    }
}
