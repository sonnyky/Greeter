using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class RegistrationGuide : MonoBehaviour
{

    WebcamManager m_WebcamManager;
    AzureManager m_AzureManager;
    BaseManager m_BaseManager;


    string m_Endpoint;
    string m_ApiKey;

    string m_ActivePersonGroup;

    TMPro.TextMeshProUGUI m_StatusText;
    TMPro.TextMeshProUGUI m_GroupNameText;

    // Find the button that will send the images to Azure for registration
    private Button m_AddFacesButton;

    private Button m_CreatePersonButton;
    private bool m_CreatePersonButtonClicked = false;
    private PersonCreateSuccess.PersonCreateSuccessResponse m_PersonCreateSuccessResponse;
    private string personId = "null";

    WaitForSeconds m_SceneTransferDelay = new WaitForSeconds(5);

    System.Action retrievedHoroscope;

    // Start is called before the first frame update
    void Start()
    {
        WindowsVoice.speak("こんにちは。写真を登録してください。");

        retrievedHoroscope += OnRetrievedHoroscope;

        StartCoroutine(NetworkManager.GetRequest("http://api.jugemkey.jp/api/horoscope/free/2019/05/20", retrievedHoroscope));

        m_WebcamManager = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<WebcamManager>();
        m_AzureManager = GameObject.FindGameObjectWithTag("AzureManager").GetComponent<AzureManager>();
        m_BaseManager = GameObject.Find("BaseManager").GetComponent<BaseManager>();


        m_ActivePersonGroup = GetPersonGroupId();

        m_StatusText = GameObject.FindGameObjectWithTag("Status").GetComponent<TMPro.TextMeshProUGUI>();
        m_GroupNameText = GameObject.FindGameObjectWithTag("GroupNameText").GetComponent<TMPro.TextMeshProUGUI>();

        m_Endpoint = m_AzureManager.GetEndpoint();
        m_ApiKey = m_AzureManager.GetApiKey();

        m_CreatePersonButton = GameObject.FindGameObjectWithTag("CreatePersonButton").GetComponent<Button>();
        m_CreatePersonButton.onClick.AddListener(() => {
            if (!m_CreatePersonButtonClicked)
            {
                string name = GetCurrentPersonName();
                string birthday = GetCurrentPersonBirthday();
                if (!name.Equals("null") && !birthday.Equals("null"))
                {
                    StartCoroutine(CheckPersonIdExists(name, birthday));
                    m_CreatePersonButtonClicked = true;
                }
            }
        });

        m_AddFacesButton = GameObject.FindGameObjectWithTag("AddFacesButton").GetComponent<Button>();
        m_AddFacesButton.onClick.AddListener(() => {

          // TODO : add body
        });

    }

    void OnRetrievedHoroscope()
    {
        Debug.Log("Horoscope data retrieved!");
    }

    void InitializeDisplay()
    {
        m_GroupNameText.text = GetPersonGroupId();
    }

    IEnumerator CheckPersonIdExists(string name, string birthday)
    {
        string personListInGroup = "";
        yield return RequestManager.GetPersonListInGroup(m_AzureManager.GetEndpoint(), m_AzureManager.GetApiKey(), m_ActivePersonGroup, value => personListInGroup = value);
        if (personListInGroup != null)
        {
            PersonInGroup.Person[] personList = JsonHelper.getJsonArray<PersonInGroup.Person>(personListInGroup);
            if (personList.Length > 0)
            {
                foreach(PersonInGroup.Person person in personList)
                {
                    if (person.name.Equals(name))
                    {
                        personId = person.personId;
                        m_WebcamManager.SetPersonId(personId);
                        Debug.Log("This person exists, personId is : " + personId);
                    }
                }
            }
        }
        else
        {
            StartCoroutine(CreatePersonInPersonGroup(m_ActivePersonGroup, name, birthday));
        }

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
        // Birthday must have delimiters such as 2000/03/16
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
            m_WebcamManager.SetPersonId(personId);
        }
    }

    public void AddFacesToAzure()
    {
        StartCoroutine(AddFacesToPersonInPersonGroup(m_ActivePersonGroup, personId));
    }

    IEnumerator AddFacesToPersonInPersonGroup(string personGroupId, string personId)
    {
        string personFolder = Application.dataPath + Constants.PREFIX_TRAIN_IMAGES_PATH + Constants.PREFIX_TRAIN_IMAGE_NAME + personId;
        string[] imageFiles = Directory.GetFiles(personFolder, "*.jpg");

        for (int i = 0; i < imageFiles.Length; i++)
        {
            string result = "";
            yield return RequestManager.AddFaceToPersonInGroup(m_Endpoint, m_ApiKey, m_ActivePersonGroup, personId, imageFiles[i], "", value => result = value);
            Debug.Log("Added face to Person in Group. Persisted Face ID : " + result);
        }

        // Train the PersonGroup
        string trainPersonGroupResult = "Unknown";
        yield return RequestManager.TrainPersonGroup(m_Endpoint, m_ApiKey, m_ActivePersonGroup, value => trainPersonGroupResult = value);

        if (trainPersonGroupResult == "")
        {
            StartCoroutine(GoToDetectionScene());
        }
        else
        {
            Debug.Log("Something went wrong : " + trainPersonGroupResult);
        }

    }

    IEnumerator GoToDetectionScene()
    {
        yield return m_SceneTransferDelay;
        m_BaseManager.Finish();
    }

    public string GetPersonId()
    {
        return personId;
    }
}
