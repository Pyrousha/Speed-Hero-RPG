using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class CubePlaceCam : MonoBehaviour
{
    public GameObject[] stuffToDisable;
    public GameObject GridObj;
    public GameObject attackCubePrefab;
    public GameObject attackCubeRemoverPrefab;
    public GameObject noteParent;

    public GameObject songLoadedPrefab;

    NoteEditorPlaySong buttonController;

    public string songName;

    public InputField inputField;

    Camera thisCam;

    public LayerMask noteGridLayer;
    float minZ;

    float scrollInput;

    [SerializeField] private Transform newNoteTransform;

    // Start is called before the first frame update
    void Start()
    {
        thisCam = GetComponent<Camera>();
        minZ = transform.position.z;

        buttonController = transform.GetChild(0).GetComponent<NoteEditorPlaySong>();
    }

    public void DisableGameComponents()
    {
        foreach (GameObject go in stuffToDisable)
        {
            go.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (buttonController.songIsPlaying)
        {
            if (Input.GetMouseButtonDown(0))
                PlaceCubeWhilePlaying();
            return;
        }

            //add notes
        if (Input.GetMouseButton(0))
        {
            PlaceCube();
        }

        //remove notes
        if (Input.GetMouseButton(1))
        {
            RemoveCube();
        }


        scrollInput = 0;

        scrollInput = Input.GetAxisRaw("Horizontal");

        scrollInput += 50 * Input.GetAxis("Mouse ScrollWheel");

        //Move camera
        if (scrollInput > 0)
        {
            transform.Translate(Vector3.right * scrollInput);
        }
        else
        {
            if (scrollInput < 0)
            {
                if (transform.position.z > minZ)
                {
                    transform.Translate(Vector3.right * scrollInput);
                }
            }
        }
        
    }

    void PlaceCubeWhilePlaying()
    {
        RaycastHit hitInfo;
        Ray ray = thisCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo, noteGridLayer))
        {
            GameObject atkCubeObj = Instantiate(attackCubePrefab) as GameObject;
            atkCubeObj.transform.position = newNoteTransform.position;
            atkCubeObj.transform.parent = noteParent.transform;
            atkCubeObj.GetComponent<AttackCube>().SetAttackNum();
        }
    }

    void PlaceCube()
    {
        RaycastHit hitInfo;
        Ray ray = thisCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo, noteGridLayer))
        {
            GameObject atkCubeObj = Instantiate(attackCubePrefab) as GameObject;
            atkCubeObj.transform.position = hitInfo.point;
            atkCubeObj.transform.parent = noteParent.transform;
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
            atkCubeObj.transform.parent = noteParent.transform;
            atkCubeObj.GetComponent<AtkCubeRemover>().removeCube();
        }

    }

    public void SaveNotes()
    {
        songName = inputField.text;

        //Save loaded song's properties to new prefab
        if (songLoadedPrefab != null)
        {
            noteParent.GetComponent<AudioSource>().clip = songLoadedPrefab.GetComponent<AudioSource>().clip;
            noteParent.GetComponent<SongProperties>().BPM = songLoadedPrefab.GetComponent<SongProperties>().BPM;
        }

        //Set path where prefab will be saved
        string localPath = "Assets/_Assets/Combat/NoteGrid/SongMaps/" + songName + ".prefab";

        #if UNITY_EDITOR
        //Make sure filename is unique
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

        //Create new prefab
        if (noteParent != null)
            PrefabUtility.SaveAsPrefabAssetAndConnect(noteParent, localPath, InteractionMode.UserAction);

        //Unpack so notes can still be edited
        PrefabUtility.UnpackPrefabInstance(noteParent, PrefabUnpackMode.Completely, InteractionMode.UserAction);
        #endif
    }

}
