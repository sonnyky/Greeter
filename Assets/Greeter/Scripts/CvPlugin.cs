using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class CvPlugin : MonoBehaviour
{

    private IntPtr captureInstance;

    private static ILogger logger = Debug.unityLogger;
    private static string kTAG = "Unity Plugin For Face Detection";

    [DllImport("uplugin_face", EntryPoint = "com_tinker_recognition_create")]
    private static extern IntPtr _Create();

    [DllImport("uplugin_face", EntryPoint = "com_tinker_recognition_get_plugin_name")]
    private static extern IntPtr _GetPluginName(IntPtr instance);

    [DllImport("uplugin_face", EntryPoint = "com_tinker_recognition_detect_faces", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void _GetColorImage(IntPtr instance, IntPtr input, IntPtr processed, int width, int height);

    private void Awake()
    {
       captureInstance = _Create();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
