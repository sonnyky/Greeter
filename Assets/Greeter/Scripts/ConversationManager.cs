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

    // Use this for initialization
    void Start() {
        ChatData testData = new ChatData();
        testData.endPoint = m_Endpoints[0].endPoint;
        testData.apiKey = m_Endpoints[0].apiKey;
        testData.message = "誰ですか？";
        SendMessage(testData);
    }

    // Update is called once per frame
    void Update() {

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
