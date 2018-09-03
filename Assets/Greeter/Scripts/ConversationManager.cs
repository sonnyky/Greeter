using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ConversationManager : MonoBehaviour {

    public int m_ApiIndex = 0;

    public ChatData[] m_Endpoints;

    private ChatData m_Chat;
    private TMPro.TextMeshProUGUI m_UserInput;
    private TMPro.TextMeshProUGUI m_AvatarInput;

    private GameObject m_ConversationCanvas;

    // Use this for initialization
    void Start() {
        m_Chat = new ChatData();
        m_Chat.endPoint = m_Endpoints[0].endPoint;
        m_Chat.apiKey = m_Endpoints[0].apiKey;

        m_ConversationCanvas = GameObject.FindGameObjectWithTag("ConversationCanvas").gameObject;
        m_UserInput = m_ConversationCanvas.transform.Find("UserInput").transform.Find("Text Area").transform.Find("Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        m_AvatarInput = m_ConversationCanvas.transform.Find("AvatarResponse").GetComponent<TMPro.TextMeshProUGUI>();

        //WindowsVoice.speak("日本語で話します");
    }

    public void SetUserInputFromVoice(string input)
    {
        m_UserInput.text = input;
        m_Chat.message = input;
        SendMessage(m_Chat);
    }

   public void GetUserInput()
    {
        m_Chat.message = m_UserInput.text;
        SendMessage(m_Chat);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GetUserInput();
        }
    }

    public void SendMessage(ChatData data)
    {
        StartCoroutine(SendChat(data));
    }

    IEnumerator SendChat(ChatData chatData)
    {
        WWWForm form = new WWWForm();

        form.AddField("apikey", chatData.apiKey);
        form.AddField("query", chatData.message, Encoding.UTF8);

        // 通信
        using (UnityWebRequest request = UnityWebRequest.Post(chatData.endPoint, form))
        {

            yield return request.Send();

            if (request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
                try
                {
                    // 取得したものをJsonで整形
                    string itemJson = request.downloadHandler.text;
                    JsonNode jsnode = JsonNode.Parse(itemJson);
                    // Jsonから会話部分だけ抽出してTextに代入
                    //if (text.text != null)
                    //{
                    //    text.text = jsnode["results"][0]["reply"].Get<string>();
                    //}
                    Debug.Log(jsnode["results"][0]["reply"].Get<string>());
                    m_AvatarInput.text = jsnode["results"][0]["reply"].Get<string>();
                    WindowsVoice.speak(jsnode["results"][0]["reply"].Get<string>());
                }
                catch (Exception e)
                {
                    // エラーが出たらこれがログに吐き出される
                    Debug.Log("JsonNode:" + e.Message);
                }
            }
        }
    }

    [System.Serializable]
    public class ChatData{

        public string endPoint;
        public string apiKey;
        public string message;
    }

}
