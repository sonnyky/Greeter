using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AvatarManager : MonoBehaviour {

    public GameObject avatar;

    private void Start()
    {
        if (avatar != null)
        {
            GameObject avatarInstance = Instantiate(avatar);
            avatarInstance.AddComponent(Type.GetType("Avatar"));
        }
    }

}
