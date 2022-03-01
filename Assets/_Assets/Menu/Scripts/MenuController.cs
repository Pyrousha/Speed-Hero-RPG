using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Transform selectableParent;
    public Transform triangleIndicator;
    public GameObject menuParent;
    public GameObject persistentGameInfo;
    public PlayerMove2D hero;
    public Color offColor;
    public Color onColor;

    bool interactable;

    public List<Text> selectablesList = new List<Text>();
    
    int oldSelectedIndex = 0;
    public int selectedIndex = 0;
    int maxIndex;
    int input;

    [Header("Submenus")]
    public GameObject beatOffsetParent;

    //Beat Offset
    //bool bOCancelSelected = true;
    public Transform bOTriangleIndicator;
    public Text bOCancelText;
    public Text bOSaveText;

    /*enum MenuState
    {
        normal, 
        beatOffset
    }

    MenuState menuState;*/

    // Start is called before the first frame update
    void Start()
    {
        //menuState = MenuState.normal;

        for (int i = 0; i< selectableParent.childCount; i++)
        {
            selectablesList.Add(selectableParent.GetChild(i).GetComponent<Text>());
        }

        maxIndex = selectablesList.Count - 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputHandler.Instance.Menu.down)
        {
            Debug.Log("pressed ESC");
           
            //Toggle Menu

            if (!interactable)
                OpenMenu();
            else
                CloseMenu();
        }

        if (!interactable)
            return;

        //Get input if selection should move up/down
        input = 0;
        if (InputHandler.Instance.Up.down)
            input++;
        if (InputHandler.Instance.Down.down)
            input--;

        if (input > 0)
        {
            //move up
            if (selectedIndex == 0)
                selectedIndex = maxIndex;
            else
                selectedIndex--;

            UpdateSelection();
        }
        else
        {
            if (input < 0)
            {
                //move down
                if (selectedIndex == maxIndex)
                    selectedIndex = 0;
                else
                    selectedIndex++;

                UpdateSelection();
            }
        }

        //Select specified button
        if (InputHandler.Instance.Interact.down)
        {
            SelectMenuButton(selectedIndex);
        }

        /*switch (menuState)
        {
            case (MenuState.normal):
                {
                    if (InputHandler.Instance.Menu.down)
                    {
                        //Toggle Menu

                        if (!interactable)
                            OpenMenu();
                        else
                            CloseMenu();
                    }

                    if (!interactable)
                        return;

                    //Get input if selection should move up/down
                    input = 0;
                    if (InputHandler.Instance.Up.down)
                        input++;
                    if (InputHandler.Instance.Down.down)
                        input--;

                    if (input > 0)
                    {
                        //move up
                        if (selectedIndex == 0)
                            selectedIndex = maxIndex;
                        else
                            selectedIndex--;

                        UpdateSelection();
                    }
                    else
                    {
                        if (input < 0)
                        {
                            //move down
                            if (selectedIndex == maxIndex)
                                selectedIndex = 0;
                            else
                                selectedIndex++;

                            UpdateSelection();
                        }
                    }

                    //Select specified button
                    if (InputHandler.Instance.Interact.down)
                    {
                        SelectMenuButton(selectedIndex);
                    }

                    break;
                }
            case (MenuState.beatOffset):
                {
                    if (InputHandler.Instance.Button_Menu.down)
                    {
                        CloseBeatCalbiration(false);
                        break;
                    }

                    if (Input.GetKeyDown(leftButton) || Input.GetKeyDown(rightButton))
                    {
                        bOCancelSelected = !bOCancelSelected;
                        UpdateBOSelection();
                    }

                    if (Input.GetKeyDown(selectButton))
                        CloseBeatCalbiration(!bOCancelSelected);

                    break;
                }
        }*/

    }

    private void UpdateSelection()
    {
        Text currText = selectablesList[selectedIndex];

        selectablesList[oldSelectedIndex].color = offColor;
        currText.color = onColor;

        triangleIndicator.position = currText.transform.GetChild(0).position;

        oldSelectedIndex = selectedIndex;
    }


    /*private void UpdateBOSelection()
    {
        if (bOCancelSelected)
        {
            //Highlight "Cancel"
            bOCancelText.color = onColor;
            bOSaveText.color = offColor;

            bOTriangleIndicator.position = bOCancelText.transform.GetChild(0).position;
        }
        else
        {
            //Highlight "Save"
            bOCancelText.color = offColor;
            bOSaveText.color = onColor;

            bOTriangleIndicator.position = bOSaveText.transform.GetChild(0).position;
        }
    }*/

    public void SelectMenuButton(int selected)
    {
        switch (selected)
        {
            case 0:
                {
                    //Resume
                    CloseMenu();
                    break;
                }
            case 1:
                {
                    //Settings
                    
                    break;
                }
            case 2:
                {
                    //Keybinds
                    
                    break;
                }
            case 3:
                {
                    //BeatCalibration
                    //OpenBeatCalibration();

                    break;
                }
            case 4:
                {
                    //QuitGame
                    
                    break;
                }
        }

    }

    public void OpenMenu()
    {
        Debug.Log("open menu");

        //Set default parameters
        oldSelectedIndex = 0;
        selectedIndex = 0;

        selectablesList[0].color = onColor;
        for (int i = 1; i < selectablesList.Count; i++)
            selectablesList[i].color = offColor;

        triangleIndicator.localPosition = new Vector3(0, 0, 0);

        interactable = true;
        hero.SetCanMove(false);
        menuParent.SetActive(true);
    }

    /*void OpenBeatCalibration()
    {
        //Set default variable values
        bOCancelText.color = onColor;
        bOSaveText.color = offColor;

        bOTriangleIndicator.position = bOCancelText.transform.GetChild(0).position;
        bOCancelSelected = true;

        //Load scene
        persistentGameInfo.GetComponent<CombatStartingState>().combatState = SongLoader.CombatState.BeatOffset;
        persistentGameInfo.GetComponent<BattleTransition>().TransitionToSceneAdditive("Combat-Standard");

        menuState = MenuState.beatOffset;

        beatOffsetParent.SetActive(true);
    }

    void CloseBeatCalbiration(bool updateOffset)
    {
        menuState = MenuState.normal;

        beatOffsetParent.SetActive(false);

        if(updateOffset)
        {
            persistentGameInfo.GetComponent<CombatStartingState>().beatOffset =  GameObject.Find("Note Grid/State Controller").GetComponent<SongLoader>().BeatTravelTime;
        }

        persistentGameInfo.GetComponent<BattleTransition>().CloseBeatCalibration();
        persistentGameInfo.GetComponent<CombatStartingState>().combatState = SongLoader.CombatState.Playing;
    }*/

    public void CloseMenu()
    {
        interactable = false;
        hero.SetCanMove(true);
        menuParent.SetActive(false);
    }
}
