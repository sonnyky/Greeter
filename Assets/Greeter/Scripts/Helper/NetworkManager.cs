using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager
{
    public static IEnumerator GetRequest(string uri, System.Action action)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);

            Horoscope[] horoscope = JsonHelper.getJsonArray<Horoscope>(uwr.downloadHandler.text);
            Debug.Log(horoscope);

            action.Invoke();
        }
    }
}
