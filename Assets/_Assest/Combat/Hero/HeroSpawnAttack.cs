using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSpawnAttack : MonoBehaviour
{
    public GameObject shotOrientation;
    public GameObject shotSpawnPoint;

    public GameObject shotProjectilePrefab;

    public float shotSpeed;

    public void Atk(int attackNum)
    {
        //Set parent Obj orientation
        switch (attackNum)
        {
            case (2):
                shotOrientation.transform.eulerAngles = new Vector3(0, 0, 180);
                break;
            case (3):
                shotOrientation.transform.eulerAngles = new Vector3(0, 0, 135);
                break;
            case (5):
                shotOrientation.transform.eulerAngles = new Vector3(0, 0, 90);
                break;
            case (7):
                shotOrientation.transform.eulerAngles = new Vector3(0, 0, 45);
                break;
            case (9):
                shotOrientation.transform.eulerAngles = new Vector3(0, 0, 0);
                break;
        }

        //Create attack projectile
        SpawnShot();
    }

    void SpawnShot()
    {
        GameObject childObject = Instantiate(shotProjectilePrefab) as GameObject;

        childObject.transform.parent = shotSpawnPoint.transform;

        childObject.transform.localPosition = new Vector3(0, 0, 0);
        childObject.transform.localRotation = Quaternion.Euler(0, 0, 0);

        childObject.GetComponent<Rigidbody>().velocity = shotSpawnPoint.transform.right * shotSpeed;
    }
}
