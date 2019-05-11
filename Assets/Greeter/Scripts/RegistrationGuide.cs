using System.Collections;
using System.Collections.Generic;
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
    private Button m_SendToAzureButton;

    public delegate void RegisterToAzure();
    public event RegisterToAzure Register;

    public void OnSendToAzureButtonPressed()
    {
        Register();
    }


    // Start is called before the first frame update
    void Start()
    {
        WindowsVoice.speak("こんにちは。写真を登録してください。");

        m_WebcamManager = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<WebcamManager>();
        m_AzureManager = GameObject.FindGameObjectWithTag("AzureManager").GetComponent<AzureManager>();

        m_ActivePersonGroup = GetPersonGroupId();

        m_StatusText = GameObject.FindGameObjectWithTag("Status").GetComponent<TMPro.TextMeshProUGUI>();
        m_GroupNameText = GameObject.FindGameObjectWithTag("GroupNameText").GetComponent<TMPro.TextMeshProUGUI>();

    }

    void InitializeDisplay()
    {
        m_GroupNameText.text = GetPersonGroupId();
    }

    string GetPersonGroupId()
    {
        return m_AzureManager.GetActivePersonGroup();
    }
}
