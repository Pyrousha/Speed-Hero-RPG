using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Transform selectableParent;
    public Transform triangleIndicator;
    public GameObject menuParent;
    public PlayerMove2D hero;
    public Color offColor;
    public Color onColor;

    bool interactable;

    List<Text> selectablesList = new List<Text>();
    
    int oldSelectedIndex = 0;
    int selectedIndex = 0;
    int maxIndex;
    int input;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i< selectableParent.childCount; i++)
        {
            selectablesList.Add(selectableParent.GetChild(i).GetComponent<Text>());
        }

        maxIndex = selectablesList.Count - 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Toggle Menu

            if (!interactable)
                OpenMenu();
            else
                CloseMenu();
        }

        if (!interactable)
            return;

        input = 0;
        if (Input.GetKeyDown(KeyCode.S))
            input--;
        if (Input.GetKeyDown(KeyCode.W))
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

        if (Input.GetKeyDown(KeyCode.Space))
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

    public void CloseMenu()
    {
        interactable = false;
        hero.canMove = true;
        menuParent.SetActive(false);
    }
}
