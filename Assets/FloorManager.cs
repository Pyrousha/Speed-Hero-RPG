using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : Singleton<FloorManager>
{
    [SerializeField] private List<GameObject> floorObjs;

    public void GoToNewFloor(int currFloorNum, int newFloorNum)
    {
        StartCoroutine(SetObjActive(floorObjs[currFloorNum], false, 1));
        StartCoroutine(SetObjActive(floorObjs[newFloorNum], true, 0));
    }

    private IEnumerator SetObjActive(GameObject obj, bool newActive, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);

        obj.SetActive(newActive);
    }
}
