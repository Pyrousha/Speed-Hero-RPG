using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMenuController : MonoBehaviour
{
    [SerializeField] private ComboAbility[] availableMoves;
    [SerializeField] private Transform selectionArrow;
    [SerializeField] private Transform moveListParent;
    [SerializeField] private GameObject movePrefab;
    [SerializeField] private MoveTimerController moveTimerController;

    [SerializeField] private Color usableColor;
    [SerializeField] private Color unusableColor;
    private HeroMenuMove[] menuMoves;
    private int selectedMoveIndex;

    // Start is called before the first frame update
    void Start()
    {
        LoadMovesIntoMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Insert))
            TryStartMove();

        //Move selection down 1
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ChangeSelection(selectedMoveIndex + 1);
        
        //Move selection up 1
        if (Input.GetKeyDown(KeyCode.UpArrow))
            ChangeSelection(selectedMoveIndex - 1);

    }

    public void OnHeroManaUpdated(int newMP)
    {
        if (menuMoves == null)
            LoadMovesIntoMenu();

        foreach (HeroMenuMove menuMove in menuMoves)
            menuMove.UpdateCanUse(newMP, usableColor, unusableColor);
    }

    private void TryStartMove()
    {
        ComboAbility moveToPerform = availableMoves[selectedMoveIndex];
        moveTimerController.TryPerformMove(moveToPerform);
    }

    private void LoadMovesIntoMenu()
    {
        menuMoves = new HeroMenuMove[availableMoves.Length];

        for (int i = 0; i< availableMoves.Length; i++)
        {
            GameObject newMoveObject = Instantiate(movePrefab, moveListParent.GetChild(i));
            newMoveObject.transform.localPosition = Vector3.zero;

            menuMoves[i] = newMoveObject.GetComponent<HeroMenuMove>();
            menuMoves[i].LoadMove(availableMoves[i]);
        }

        UpdateSelectionArrow(0);
    }

    private void ChangeSelection(int targetIndex)
    {
        int numMoves = menuMoves.Length;
        int newIndex = (targetIndex + numMoves) % numMoves;
        UpdateSelectionArrow(newIndex);
    }

    private void UpdateSelectionArrow(int newIndex)
    {
        //make sure index is valid
        if (newIndex >= menuMoves.Length)
            throw new NotImplementedException();

        selectionArrow.SetParent(menuMoves[newIndex].ArrowLocation, true);
        selectionArrow.localPosition = Vector3.zero;
        selectedMoveIndex = newIndex;

        moveTimerController.LoadMoveIntoSheetDisplay(availableMoves[newIndex]);
    }
}
