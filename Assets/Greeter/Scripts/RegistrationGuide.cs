using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class RegistrationGuide : MonoBehaviour
{

    WebcamManager m_WebcamManager;
    AzureManager m_AzureManager;

    string m_ActivePersonGroup;

    TMPro.TextMeshProUGUI m_StatusText;
    TMPro.TextMeshProUGUI m_GroupNameText;

    // Find the button that will send the images to Azure for registration
    private Button m_AddFacesButton;

    private Button m_CreatePersonButton;
    private PersonCreateSuccess.PersonCreateSuccessResponse m_PersonCreateSuccessResponse;
    private string personId = "null";


    // Start is called before the first frame update
    void Start()
    {
        WindowsVoice.speak("こんにちは。写真を登録してください。");

        m_WebcamManager = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<WebcamManager>();
        m_AzureManager = GameObject.FindGameObjectWithTag("AzureManager").GetComponent<AzureManager>();

        m_ActivePersonGroup = GetPersonGroupId();

        m_StatusText = GameObject.FindGameObjectWithTag("Status").GetComponent<TMPro.TextMeshProUGUI>();
        m_GroupNameText = GameObject.FindGameObjectWithTag("GroupNameText").GetComponent<TMPro.TextMeshProUGUI>();

        m_CreatePersonButton = GameObject.FindGameObjectWithTag("CreatePersonButton").GetComponent<Button>();
        m_CreatePersonButton.onClick.AddListener(() => {

            string name = GetCurrentPersonName();
            string birthday = GetCurrentPersonBirthday();
            if (!name.Equals("null") && !birthday.Equals("null"))
            {
                // TODO ! Must check first if the name and birthday are already registered. if yes, return that personId

                StartCoroutine(CreatePersonInPersonGroup(m_ActivePersonGroup, name, birthday));
            }
        });

        m_AddFacesButton = GameObject.FindGameObjectWithTag("AddFacesButton").GetComponent<Button>();
        m_AddFacesButton.onClick.AddListener(() => {

          // TODO : add body
        });

    }

    void InitializeDisplay()
    {
        m_GroupNameText.text = GetPersonGroupId();
    }

    string GetPersonGroupId()
    {
        return m_AzureManager.GetActivePersonGroup();
    }

    string GetCurrentPersonName()
    {
        string name = GameObject.FindGameObjectWithTag("PersonName").GetComponent<TMPro.TextMeshProUGUI>().text;
        if (name.Length <= 1)
        {
            name = "null";
            Debug.Log("No PersonName specified. Try again!");
        }
        return name;
    }

    string GetCurrentPersonBirthday()
    {
        string birthday = GameObject.FindGameObjectWithTag("PersonBirthday").GetComponent<TMPro.TextMeshProUGUI>().text;
        if (birthday.Length <= 1 || !DateTimeUtils.DateTimeFormatCheck(birthday))
        {
            birthday = "null";
            Debug.Log("No Birthday specified. Try again!");
        }
        return birthday;
    }

    void SetStatusText(string textResult)
    {
        m_StatusText.text = Constants.STATUS_PREFIX + textResult;
    }

    IEnumerator CreatePersonInPersonGroup(string personGroupId, string personName, string birthday)
    {
        yield return RequestManager.CreatePersonInGroup(
            m_AzureManager.GetEndpoint(),
            m_AzureManager.GetApiKey(),
            personGroupId,
            personName,
            birthday,
            value => personId = value);

        m_PersonCreateSuccessResponse = JsonUtility.FromJson<PersonCreateSuccess.PersonCreateSuccessResponse>(personId);
        personId = m_PersonCreateSuccessResponse.personId;

        if (personId.Equals("null"))
        {
            SetStatusText("CreatePerson 失敗しました。入力を確認し、もう一度お試しください。");
        }
        else
        {
            // Add faces to the Person object created

            

        }
    }

    IEnumerator AddFacesToPersonInPersonGroup(string personGroupId, string personId)
    {
        string[] imageFiles = Directory.GetFiles(m_ImageFolderPath, "*.jpg");
    }

    public string GetPersonId()
    {
        return personId;
    }
}
