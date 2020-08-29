using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoCounter : MonoBehaviour
{

    WebcamManager m_WebcamManager;

    int m_PhotoCount = 0;

    TMPro.TextMeshProUGUI m_Count;

    // Start is called before the first frame update
    void Start()
    {
        m_WebcamManager = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<WebcamManager>();
        WebcamManager.RegisterPhoto += PhotoCountUp;
        m_Count = GetComponent<TMPro.TextMeshProUGUI>();
    }

    void PhotoCountUp()
    {
        m_PhotoCount++;
        m_Count.text = m_PhotoCount.ToString() + "枚";
    }

}
