using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockParent : MonoBehaviour
{
    [SerializeField] private float maxRandTime;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void StartRockFall()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Transform currChild = transform.GetChild(i);
            
            float timeToStart = Random.Range(0, maxRandTime);

            StartCoroutine(MakeRockFall(currChild, timeToStart));
        }
    }

    private IEnumerator MakeRockFall(Transform currChild, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);

        currChild.gameObject.SetActive(true);

        Animation anim = currChild.GetComponent<Animation>();
        anim.Play();
    }
}
