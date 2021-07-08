using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Retry()
    {
        if (SceneManager.GetSceneByName("Zone1").isLoaded)
        {
            SceneManager.LoadScene("Combat-Standard", LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync("DEAD");
        }
        else
        {
            SceneManager.LoadScene("Combat-Standard", LoadSceneMode.Single);
        }

    }
}
