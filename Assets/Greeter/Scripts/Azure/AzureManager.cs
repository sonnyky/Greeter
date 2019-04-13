using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AzureManager : MonoBehaviour
{
    private string m_ApiKey = "None";

    [SerializeField]
    string m_Endpoint="";

    [SerializeField]
    string m_PersonGroup = "Placeholder";

    int m_NumberOfPersonsInGroup = 0;

    bool m_IsGroupTrained = false;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_ApiKey = EnvironmentVariables.GetVariable("GREETER_AZURE_FACE_API_KEY");
        Debug.Log("Endpoint is : " + m_Endpoint);
    }

   IEnumerator Validation()
    {
        if (m_ApiKey.Equals("None"))
        {
            Debug.LogError("No environment variable key is defined. Stopping.");
            yield break;
        }

        // Check if the person group ID already exists
        bool requestSucceded = false;
        yield return RequestManager.GetPersonGroup(m_Endpoint, m_ApiKey, m_PersonGroup, value => requestSucceded = value);

    }

    public int GetNumberOfPersonsInGroup()
    {
        return m_NumberOfPersonsInGroup;
    }
}
