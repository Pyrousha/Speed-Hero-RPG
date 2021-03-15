using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePlaceCam : MonoBehaviour
{
    public GameObject[] stuffToDisable;
    public GameObject GridObj;
    public GameObject attackCubePrefab;
    public GameObject attackCubeRemoverPrefab;

    Camera thisCam;

    public LayerMask noteGridLayer;

    

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject go in stuffToDisable)
        {
            go.SetActive(false);
        }

        thisCam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlaceCube();
        }

        if (Input.GetMouseButtonDown(1))
        {
            RemoveCube();
        }
    }

    void PlaceCube()
    {
        RaycastHit hitInfo;
        Ray ray = thisCam.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction * 500, Color.red, 5f);

        if (Physics.Raycast(ray, out hitInfo, noteGridLayer))
        {
            GameObject atkCubeObj = Instantiate(attackCubePrefab) as GameObject;
            atkCubeObj.transform.position = hitInfo.point;
            atkCubeObj.transform.parent = GridObj.transform;
            atkCubeObj.GetComponent<AttackCube>().SetAttackNum();
        }

    }

    void RemoveCube()
    {
        RaycastHit hitInfo;
        Ray ray = thisCam.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction * 500, Color.red, 5f);

        if (Physics.Raycast(ray, out hitInfo, noteGridLayer))
        {
            GameObject atkCubeObj = Instantiate(attackCubeRemoverPrefab) as GameObject;
            atkCubeObj.transform.position = hitInfo.point;
            atkCubeObj.transform.parent = GridObj.transform;
            atkCubeObj.GetComponent<AtkCubeRemover>().removeCube();
        }

    }
}
