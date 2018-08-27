using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour {

    private ConversationManager m_Conv;

	// Use this for initialization
	void Start () {
        m_Conv = GameObject.Find("ConversationManager").GetComponent<ConversationManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            m_Conv.GetUserInput();
        }
    }
}
