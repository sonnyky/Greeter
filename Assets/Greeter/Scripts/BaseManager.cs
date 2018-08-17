using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseManager : MonoBehaviour {

    public GameObject[] m_Managers;

    public string m_NextSceneName = "";

	// Use this for initialization
	void Start () {
		for(int i=0; i<m_Managers.Length; i++)
        {
            Instantiate(m_Managers[i]);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Finish()
    {
        SceneManager.LoadScene(m_NextSceneName);
    }
}
