using UnityEngine;

public class DisableGameobject : MonoBehaviour
{
    [SerializeField] private bool disableOnStart;

    private void Start()
    {
        if (disableOnStart)
            Disable();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
