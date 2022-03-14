using UnityEngine;

[CreateAssetMenu(menuName = "Combat/ComboAbility")]
public class ComboAbility : ScriptableObject
{
    public enum CrystalTypeEnum
    {
        Speed
    }

    [SerializeField] private CrystalTypeEnum crystalType;
    public CrystalTypeEnum GetCrystalType => crystalType;

    [SerializeField] private int manaCost;
    public int GetManaCost => manaCost;

    public enum attackDir
    {
        Left,
        DiagLeft,
        Up,
        DiagRight,
        Right,
        Space
    }

    [System.Serializable]
    public struct ComboInput
    {
        public attackDir attackDirection;
        public int eigthNotesFromStart;
        public int damageAfterPressed;
    }

    [SerializeField] private ComboInput[] comboInputs;
    public ComboInput[] GetComboInputs => comboInputs;

    private int[] inputDirs;
    private int[] inputTimesENotes;
    private int[] damages;

    public int[] GetInputDirs()
    {
        if ((inputDirs == null) || (inputDirs.Length != comboInputs.Length))
            ConvertComboInputs();

        return inputDirs;
    }

    public int[] GetinputTimesENotes()
    {
        if ((inputTimesENotes == null) || (inputTimesENotes.Length != comboInputs.Length))
            ConvertComboInputs();

        return inputTimesENotes;
    }

    public int[] GetDamages()
    {
        if ((damages == null) || (damages.Length != comboInputs.Length))
            ConvertComboInputs();

        return damages;
    }

    public void ConvertComboInputs()
    {
        int numInputs = comboInputs.Length;
        inputDirs = new int[numInputs];
        inputTimesENotes = new int[numInputs];
        damages = new int[numInputs];

        for (int i = 0; i< numInputs; i++)
        {
            ComboInput currInput = comboInputs[i];
            inputDirs[i] = AttackDirToInt(currInput.attackDirection);
            inputTimesENotes[i] = currInput.eigthNotesFromStart;
            damages[i] = currInput.damageAfterPressed;
        }
    }

    public int AttackDirToInt(attackDir dir)
    {
        switch(dir)
        {
            case attackDir.Left:
                return 2;
            case attackDir.DiagLeft:
                return 3;
            case attackDir.Up:
                return 5;
            case attackDir.DiagRight:
                return 7;
            case attackDir.Right:
                return 9;
        }

        Debug.Log("INVALID ATTACKDIR PASSED IN: "+dir);
        return(-1);
    }
}
