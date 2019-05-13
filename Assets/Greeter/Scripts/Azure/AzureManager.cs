using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class AzureManager : MonoBehaviour
{
    private string m_ApiKey = "None";

    [SerializeField]
    string m_Endpoint="";

    int m_NumberOfPersonsInGroup = 0;

    string m_PersonGroup = "";

    bool m_IsGroupTrained = false;

    BaseManager m_BaseManager;

    WebcamManager m_WebcamManager;

    bool m_PersonGroupExists = false;
    bool m_PersonGroupNotEmpty = false;
    bool m_PersonGroupTrained = false;

    // List of persons contained in the PersonGroup. This list will be used to detect if there are any known person in the target image.
    List<PersonInGroup.Person> persons;

    WaitForSeconds m_SceneTransferDelay = new WaitForSeconds(5);
    WaitForSeconds m_DelayUntilAutoCreatePersonGroup = new WaitForSeconds(5f);
    WaitForSeconds m_DelayUntilRecognition = new WaitForSeconds(2f);
    WaitForSeconds m_DelayCommon = new WaitForSeconds(1f);

    TMPro.TextMeshProUGUI m_StatusText;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_ApiKey = EnvironmentVariables.GetVariable("GREETER_AZURE_FACE_API_KEY");
        Debug.Log("Endpoint is : " + m_Endpoint);

        m_BaseManager = GameObject.Find("BaseManager").GetComponent<BaseManager>();
        m_StatusText = GameObject.FindGameObjectWithTag("Status").GetComponent<TMPro.TextMeshProUGUI>();
        m_WebcamManager = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<WebcamManager>();

        persons = new List<PersonInGroup.Person>();
    }

    void GetPersonGroupInput()
    {
        m_PersonGroup = GameObject.FindGameObjectWithTag("PersonGroup").GetComponent<TMPro.TextMeshProUGUI>().text;
        if (m_PersonGroup.Length <= 1)
        {
            m_PersonGroup = "placeholder";
            Debug.Log("No PersonGroup specified. Setting to default (All small caps) : placeholder");
        }
    }

    public void StartValidation()
    {
        GetPersonGroupInput();
        StartCoroutine(Validation());
    }

    IEnumerator Validation()
    {
        if (m_ApiKey.Equals("None"))
        {
            Debug.LogError("No environment variable key is defined. Stopping.");
            yield break;
        }

        // Check if the person group ID already exists
        yield return RequestManager.GetPersonGroup(m_Endpoint, m_ApiKey, m_PersonGroup, value => m_PersonGroupExists = value);

        // If PersonGroup exists, check that it has at least one person object.
        if (!m_PersonGroupExists)
        {
            // If PersonGroup does not exist, automatically create it.
            m_StatusText.text = Constants.STATUS_PREFIX + "指定したグループがありません --> " + m_PersonGroup
                + " <-> 自動的にPersonGroup を作成します。";
            // m_WebcamManager = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<WebcamManager>();
            // m_WebcamManager.StopCamera();
            yield return StartCoroutine(CreatePersonGroup(m_PersonGroup));
        }

        #region Get the list of Persons in the PersonGroup
        yield return StartCoroutine(GetPersonListInPersonGroup());

        // If PersonGroup has at least one person, check if it is trained.
        if (m_PersonGroupNotEmpty)
        {
            SetStatusText("定義されているPerson があります。");
            yield return StartCoroutine(GetPersonGroupTrainedStatus());
        }
        else
        {
            SetStatusText("グループは空です。少なくとも一人を定義してください。5秒後に登録画面に遷移します。");
            // Go to registration scene and register at least one face
            StartCoroutine(GoToRegistration());
            yield break;
        }

        #endregion

        #region PersonGroup exists (is defined) and has at least one Person defined. So we check the Training Status
        // ここまで来たらグループには一人以上が定義されている。あとはトレーニングされているかどうかのチェック。
        if (m_PersonGroupTrained)
        {
            StartCoroutine(Recognize());
        }
        else
        {
            SetStatusText("PersonGroup には１人以上定義されているが、トレーニングされていない。");
            string trainPersonGroupResult = "Unknown";
            yield return RequestManager.TrainPersonGroup(m_Endpoint, m_ApiKey, m_PersonGroup, value => trainPersonGroupResult = value);
            if (trainPersonGroupResult == "")
            {
                Debug.Log("Training success. Trying identification.");
                StartCoroutine(Recognize());
            }
        }
        #endregion

        if (!m_PersonGroupExists || !m_PersonGroupNotEmpty || !m_PersonGroupTrained)
        {
            Debug.LogError("Unknown error");
            SetStatusText("顔検知の条件が揃えられなかった。再度お試しください。");
        }

    }

    IEnumerator CreatePersonGroup(string personGroup)
    {
        yield return m_DelayUntilAutoCreatePersonGroup;
        bool createPersonGroupSucceded = false;
        yield return RequestManager.CreatePersonGroup(m_Endpoint, m_ApiKey, m_PersonGroup, m_PersonGroup, "Auto created", 
            value => createPersonGroupSucceded = value);

        if (!createPersonGroupSucceded)
        {
            SetStatusText("PersonGroup作成に失敗しました。もう一度アプリを立ち上げてみてください。");
        }
        else
        {
            SetStatusText("PersonGroup作成に成功しました。検出を試みます。");
        }

    }

    void SetStatusText(string textResult)
    {
        m_StatusText.text = Constants.STATUS_PREFIX + textResult;
    }

    IEnumerator GoToRegistration()
    {
        yield return m_SceneTransferDelay;
        m_BaseManager.Finish();
    }

    IEnumerator Recognize()
    {
        yield return m_DelayUntilRecognition;
        Debug.Log("Starting recognition process");

        // Try to detect face in test image
        string[] testImageFiles = Directory.GetFiles(m_WebcamManager.GetRuntimeImagePath(), "*.jpg");

        for (int i = 0; i < testImageFiles.Length; i++)
        {
            Debug.Log(testImageFiles[i]);
            // Detect faces in the test image
            string detectFaces = "unknown";
            yield return RequestManager.DetectFaces(m_Endpoint, m_ApiKey, testImageFiles[i], value => detectFaces = value);
            Debug.Log("Response from DetectFaces : " + detectFaces);
            FacesBasic.FacesDetectionResponse[] face = JsonHelper.getJsonArray<FacesBasic.FacesDetectionResponse>(detectFaces);

            // Identify faces in the test image
            string identifyFaces = "unknown";
            yield return RequestManager.Identify(m_Endpoint, m_ApiKey, m_PersonGroup, face, value => identifyFaces = value);
            Debug.Log("Response from IdentifyFaces : " + identifyFaces);

            IdentifiedFaces.IdentifiedFacesResponse[] idFaces = JsonHelper.getJsonArray<IdentifiedFaces.IdentifiedFacesResponse>(identifyFaces);

            // TODO : Compare with list of known people to see if any known person is present
        }
    }

    IEnumerator GetPersonListInPersonGroup()
    {
        yield return m_DelayCommon;
        Debug.Log("Getting person list from PersonGroup");

        string personListRetrieved = null;
        yield return RequestManager.GetPersonListInGroup(m_Endpoint, m_ApiKey, m_PersonGroup, value => personListRetrieved = value);
        if (personListRetrieved != null)
        {
            PersonInGroup.Person[] personList = JsonHelper.getJsonArray<PersonInGroup.Person>(personListRetrieved);
            if(personList.Length > 0)
            {
                Debug.Log("Person list is NOT empty in PersonGroup");
                m_PersonGroupNotEmpty = true;
                persons.AddRange(personList);
            }
            else
            {
                Debug.Log("Person list is empty in PersonGroup");
            }
        }

    }

    IEnumerator GetPersonGroupTrainedStatus()
    {
        yield return m_DelayCommon;
        Debug.Log("Getting PersonGroup trained status");
        string trainingStatus = "Unknown";
        yield return RequestManager.GetPersonGroupTrainingStatus(m_Endpoint, m_ApiKey, m_PersonGroup, value => trainingStatus = value);
        Error.ErrorResponse res_error = JsonUtility.FromJson<Error.ErrorResponse>(trainingStatus);
        Debug.Log(res_error.error.code);

        if (res_error.error.code != null || res_error.error.code.Equals("PersonGroupNotTrained"))
        {
            // Do nothing if the PersonGroup is not trained.
        }
        else
        {
            m_PersonGroupTrained = true;
        }

    }

    public int GetNumberOfPersonsInGroup()
    {
        return m_NumberOfPersonsInGroup;
    }

    /// <summary>
    /// Rerun validation process again.
    /// </summary>
    void Revalidate()
    {
        Validation();
    }

    public string GetActivePersonGroup()
    {
        return m_PersonGroup;
    }

    /// <summary>
    /// For other objects that need to call the Azure Face API such as the Registration Guide
    /// </summary>
    /// <returns></returns>
    public string GetEndpoint()
    {
        return m_Endpoint;
    }

    /// <summary>
    /// For other objects that need to call the Azure Face API such as the Registration Guide
    /// </summary>
    /// <returns></returns>
    public string GetApiKey()
    {
        return m_ApiKey;
    }
}
