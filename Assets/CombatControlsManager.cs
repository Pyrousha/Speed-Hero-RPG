using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControlsManager : MonoBehaviour
{
    [SerializeField] private Hero_Controller_Combat heroControllerCombat;
    [SerializeField] private HeroMenuController     heroMenuController;
    [SerializeField] private MoveTimerController    moveTimerController;

    //Blocking Inputs
    public KeyCode blockInputLeft { get; private set; }
    public KeyCode blockInputDiagLeft { get; private set; }
    public KeyCode blockInputUp { get; private set; }
    public KeyCode blockInputDiagRight { get; private set; }
    public KeyCode blockInputRight { get; private set; }
    public KeyCode blockInputDown { get; private set; }

    //Menu Inputs
    public KeyCode menuInputUp { get; private set; }
    public KeyCode menuInputDown { get; private set; }
    public KeyCode menuInputSelect { get; private set; }

    //Attacking-Combo Inputs
    public KeyCode attackInputLeft { get; private set; }
    public KeyCode attackInputDiagLeft { get; private set; }
    public KeyCode attackInputUp { get; private set; }
    public KeyCode attackInputDiagRight { get; private set; }
    public KeyCode attackInputRight { get; private set; }
    public KeyCode attackInputFinish { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        #region Blocking Inputs
        //Blocking: AWERV 
        blockInputLeft =      KeyCode.A;
        blockInputDiagLeft =  KeyCode.W;
        blockInputUp =        KeyCode.E;
        blockInputDiagRight = KeyCode.R;
        blockInputRight =     KeyCode.V;
        blockInputDown =      KeyCode.D;//

        /*Blocking: WASD
        blockInputLeft =      KeyCode.A;
        blockInputDiagLeft =  KeyCode.Q;
        blockInputUp =        KeyCode.W;
        blockInputDiagRight = KeyCode.E;
        blockInputRight =     KeyCode.D;
        blockInputDown =      KeyCode.S;*/
        #endregion
        

        #region Menuing Inputs
        /*Menuing: Arrowkeys
        menuInputUp =     KeyCode.UpArrow;
        menuInputDown =   KeyCode.DownArrow;
        menuInputSelect = KeyCode.Insert;*/

        //Menuing: IJKL
        menuInputUp = KeyCode.I;
        menuInputDown = KeyCode.K;
        menuInputSelect = KeyCode.Semicolon;
        #endregion


        #region Attacking Inputs
        /*Attacking: AWERV
        attackInputLeft =      KeyCode.A;
        attackInputDiagLeft =  KeyCode.W;
        attackInputUp =        KeyCode.E;
        attackInputDiagRight = KeyCode.R;
        attackInputRight =     KeyCode.V;*/

        /*Attacking: NIOP'
        attackInputLeft =      KeyCode.N;
        attackInputDiagLeft =  KeyCode.I;
        attackInputUp =        KeyCode.O;
        attackInputDiagRight = KeyCode.P;
        attackInputRight =     KeyCode.Quote;*/

        /*Attacking: WASD
        attackInputLeft =      KeyCode.A;
        attackInputDiagLeft =  KeyCode.Q;
        attackInputUp =        KeyCode.W;
        attackInputDiagRight = KeyCode.E;
        attackInputRight =     KeyCode.D;*/

        //Attacking: IJLK
        attackInputLeft =      KeyCode.J;
        attackInputDiagLeft =  KeyCode.U;
        attackInputUp =        KeyCode.I;
        attackInputDiagRight = KeyCode.O;
        attackInputRight =     KeyCode.L;
        #endregion


        //Load controls
        heroControllerCombat.LoadControls(this);
        heroMenuController.LoadControls(this);
        moveTimerController.LoadControls(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
