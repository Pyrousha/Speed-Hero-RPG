using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private RectTransform responseBox;
    [SerializeField] private RectTransform responseButtonTemplate;
    [SerializeField] private RectTransform responseContainer;
    [SerializeField] private Transform triangleIndicator;

    private DialogueUI dialogueUI;
    private ResponseEvent[] responseEvents;

    private List<GameObject> tempResponseButtons = new List<GameObject>();

    private GameObject[] responseObjects;
    private Response[] responseArray;
    private int responseIndex;
    private bool responsesEnabled = false;

    private void Start()
    {
        dialogueUI = GetComponent<DialogueUI>();
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        this.responseEvents = responseEvents;
    }

    private void Update()
    {
        if (responsesEnabled == false)
            return;

        //Press Down
        if (Input.GetKeyDown(KeyCode.S))
        {
            responseIndex++;
            if (responseIndex > (responseObjects.Length - 1))
                responseIndex = 0;
            SetIndicator();
        }

        //Press up
        if (Input.GetKeyDown(KeyCode.W))
        {
            responseIndex--;
            if (responseIndex < 0)
                responseIndex = (responseObjects.Length - 1);
            SetIndicator();
        }

        //Press Space
        if (Input.GetKeyDown(KeyCode.Space))
            OnPickedResponse(responseArray[responseIndex], responseIndex);
    }

    public void ShowResponses(Response[] responses)
    {
        float responseBoxHeight = 0;

        responseObjects = new GameObject[responses.Length];
        responseArray = new Response[responses.Length];

        for(int i = 0; i<responses.Length; i++)
        {
            Response response = responses[i];
            int responseIndex = i;

            responseArray[i] = response;

            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
            responseButton.gameObject.SetActive(true);

            responseObjects[i] = responseButton;

            TMP_Text responseText = responseButton.GetComponentInChildren<TMP_Text>();
            responseText.text = response.ResponseText;

            responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response, responseIndex));

            tempResponseButtons.Add(responseButton);

            responseBoxHeight += responseButtonTemplate.sizeDelta.y;
        }

        responseIndex = 0;

        Invoke("SetIndicator",0);

        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
        responseBox.gameObject.SetActive(true);

        responsesEnabled = true;
    }



    private void SetIndicator()
    {
        triangleIndicator.position = responseObjects[responseIndex].transform.Find("IncidatorLocation").position;
    }

    private void OnPickedResponse(Response response, int responseIndex)
    {
        responsesEnabled = false;

        responseBox.gameObject.SetActive(false);

        foreach (GameObject button in tempResponseButtons)
        {
            Destroy(button);
        }
        tempResponseButtons.Clear();

        if (responseEvents != null && responseIndex <= responseEvents.Length)
        {
            responseEvents[responseIndex].OnPickedResponse?.Invoke();
        }

        responseEvents = null;
        responseArray = null;
        responseObjects = null;

        if (response.DialogueObject)
        {
            dialogueUI.ShowDialogue(response.DialogueObject, null);
        }
        else
        {
            dialogueUI.CloseDialogueBox();
        }
    }
}
