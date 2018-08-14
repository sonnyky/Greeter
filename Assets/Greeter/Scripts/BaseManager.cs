using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseManager : MonoBehaviour {

    public GameObject[] m_Managers;

    public string m_NextSceneName = "";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Finish()
    {
        SceneManager.LoadScene(m_NextSceneName);
    }
}
