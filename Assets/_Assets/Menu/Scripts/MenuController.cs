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

    List<Text> selectablesList = new List<Text>();
    
    int oldSelectedIndex = 0;
    int selectedIndex = 0;
    int maxIndex;
    int input;

    //Controls
    KeyCode menuButton = KeyCode.Escape;
    KeyCode selectButton = KeyCode.Space;
    KeyCode upButton = KeyCode.W;
    KeyCode downbutton = KeyCode.S;
    KeyCode leftButton = KeyCode.A;
    KeyCode rightButton = KeyCode.D;

    [Header("Submenus")]
    public GameObject beatOffsetParent;

    //Beat Offset
    bool bOCancelSelected = true;
    public Transform bOTriangleIndicator;
    public Text bOCancelText;
    public Text bOSaveText;

    enum MenuState
    {
        normal, 
        beatOffset
    }

    MenuState menuState;

    // Start is called before the first frame update
    void Start()
    {
        menuState = MenuState.normal;

        for (int i = 0; i< selectableParent.childCount; i++)
        {
            selectablesList.Add(selectableParent.GetChild(i).GetComponent<Text>());
        }

        maxIndex = selectablesList.Count - 1;
    }

    // Update is called once per frame
    void Update()
    {
        switch (menuState)
        {
            case (MenuState.normal):
                {
                    if (Input.GetKeyDown(menuButton))
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
                    if (Input.GetKeyDown(downbutton))
                        input--;
                    if (Input.GetKeyDown(upButton))
                        input++;

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
                    if (Input.GetKeyDown(selectButton))
                    {
                        SelectMenuButton(selectedIndex);
                    }

                    break;
                }
            case (MenuState.beatOffset):
                {
                    if (Input.GetKeyDown(menuButton))
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
        }

    }

    private void UpdateSelection()
    {
        Text currText = selectablesList[selectedIndex];

        selectablesList[oldSelectedIndex].color = offColor;
        currText.color = onColor;

        triangleIndicator.position = currText.transform.GetChild(0).position;

        oldSelectedIndex = selectedIndex;
    }

    private void UpdateBOSelection()
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
    }

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
                    OpenBeatCalibration();

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
        //Set default parameters
        oldSelectedIndex = 0;
        selectedIndex = 0;

        selectablesList[0].color = onColor;
        for (int i = 1; i < selectablesList.Count; i++)
            selectablesList[i].color = offColor;

        triangleIndicator.localPosition = new Vector3(0, 0, 0);

        interactable = true;
        hero.canMove = false;
        menuParent.SetActive(true);
    }

    void OpenBeatCalibration()
    {
        //Set default variable values
        bOCancelText.color = onColor;
        bOSaveText.color = offColor;

        bOTriangleIndicator.position = bOCancelText.transform.GetChild(0).position;
        bOCancelSelected = true;

        //Load scene
        persistentGameInfo.GetComponent<CombatStartingState>().combatState = SongLoader.CombatState.BeatOffset;
        persistentGameInfo.GetComponent<BattleTransition>().TransitionToBattle("Combat-Standard", false);

        menuState = MenuState.beatOffset;

        beatOffsetParent.SetActive(true);
    }

    void CloseBeatCalbiration(bool updateOffset)
    {
        menuState = MenuState.normal;

        beatOffsetParent.SetActive(false);

        if(updateOffset)
        {
            persistentGameInfo.GetComponent<CombatStartingState>().beatOffset =  GameObject.Find("Note Grid/State Controller").GetComponent<SongLoader>().beatTravelTime;
        }

        persistentGameInfo.GetComponent<BattleTransition>().TransitionFromBattle(false);
        persistentGameInfo.GetComponent<CombatStartingState>().combatState = SongLoader.CombatState.Playing;
    }

    public void CloseMenu()
    {
        interactable = false;
        hero.canMove = true;
        menuParent.SetActive(false);
    }
}
