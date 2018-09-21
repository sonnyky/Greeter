using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AvatarManager : MonoBehaviour {

    public GameObject avatar;
    private Vector3 m_CamPos;

    private void Start()
    {

        m_CamPos = Camera.main.transform.position;
        if (avatar != null)
        {
            GameObject avatarInstance = Instantiate(avatar);
            avatarInstance.AddComponent(Type.GetType("Avatar"));
            m_CamPos.y = 20.5f;
            avatarInstance.transform.localScale = new Vector3(5f,5f,5f);
            avatarInstance.transform.position = m_CamPos + avatarInstance.transform.forward * 10f;
        }
    }

}
