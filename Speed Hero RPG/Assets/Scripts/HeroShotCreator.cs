using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroShotCreator : MonoBehaviour
{

    public GameObject shotParent;
    public GameObject shotProjectile;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void attack_2()
    {
        gameObject.transform.eulerAngles = new Vector3(0, 0, 180);
        spawnShot();
    }

    public void attack_3()
    {
        gameObject.transform.eulerAngles = new Vector3(0, 0, 135);
        spawnShot();
    }
    public void attack_5()
    {
        gameObject.transform.eulerAngles = new Vector3(0, 0, 90);
        spawnShot();
    }
    public void attack_7()
    {
        gameObject.transform.eulerAngles = new Vector3(0, 0, 45);
        spawnShot();
    }
    public void attack_9()
    {
        gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        spawnShot();
    }

    public void spawnShot()
    {
        GameObject childObject = Instantiate(shotProjectile) as GameObject;
        childObject.transform.parent = shotParent.transform;
        childObject.transform.localPosition = new Vector3(0, 0, 0);
        childObject.transform.localRotation = Quaternion.Euler(0, 0, 0); 
        childObject.GetComponent<Rigidbody>().velocity = transform.right*(childObject.GetComponent<BladeShot>().initialSpeed);
    }
}
