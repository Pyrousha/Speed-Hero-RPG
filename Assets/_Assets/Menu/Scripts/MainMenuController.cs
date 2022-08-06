using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : Singleton<MainMenuController>
{
    public Transform selectableParent;
    public Transform triangleIndicator;
    public GameObject menuParent;
    public GameObject persistentGameInfo;
    public PlayerMove2D hero;
    public Color offColor;
    public Color onColor;

    public List<Text> selectablesList = new List<Text>();
    
    int oldSelectedIndex = 0;
    public int selectedIndex = 0;
    int maxIndex;
    int input;

    bool interactable;

    //[Header("Submenus")]

    // Start is called before the first frame update
    void Start()
    {
        //menuState = MenuState.normal;

        for (int i = 0; i< selectableParent.childCount; i++)
        {
            selectablesList.Add(selectableParent.GetChild(i).GetComponent<Text>());
        }

        maxIndex = selectablesList.Count - 1;

        interactable = true;
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    private void UpdateSelection()
    {
        Text currText = selectablesList[selectedIndex];

        selectablesList[oldSelectedIndex].color = offColor;
        currText.color = onColor;

        triangleIndicator.position = currText.transform.GetChild(0).position;

        oldSelectedIndex = selectedIndex;
    }

    public void SelectMenuButton(int selected)
    {
        switch (selected)
        {
            case 0: //Play
                {
                    interactable = false;

                    ScreenwipeCanvas.Instance.ClearToBlack();

                    Invoke(nameof(LoadNextScene), 65f / 60f);

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
                    //QuitGame

                    break;
                }
        }
    }

    private void LoadNextScene()
    {
        SceneController.Instance.LoadNextScene();
    }
}
