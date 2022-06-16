using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static List<Transform> GetChildrenFromParent(Transform parent)
    {
        List<Transform> toReturn = new List<Transform>();

        for(int i = 0; i< parent.childCount;i++)
        {
            toReturn.Add(parent.GetChild(i));
        }

        return toReturn;
    }
}
