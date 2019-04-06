using UnityEngine;
using UnityEngine.UI;

public class WebcamManager : MonoBehaviour
{
    public RawImage rawimage;

    WebCamDevice[] devices;

    WebCamTexture _CamTex;

    string _SavePath = "D:/Temp/";
    int _CaptureCounter = 0;

    void Start()
    {
        devices = WebCamTexture.devices;
      
        // Stream from the first camera device
        StreamCamera(devices[0].name);

    }

    public void TakeSnapshot()
    {
        Texture2D snap = new Texture2D(_CamTex.width, _CamTex.height);
        snap.SetPixels(_CamTex.GetPixels());
        snap.Apply();

        System.IO.File.WriteAllBytes(_SavePath + _CaptureCounter.ToString() + ".png", snap.EncodeToPNG());
        ++_CaptureCounter;
    }

    public void StreamCamera(string cameraName)
    {
        WebCamTexture webcamTexture = new WebCamTexture(cameraName);
        rawimage.texture = webcamTexture;
        _CamTex = webcamTexture;
        rawimage.material.mainTexture = webcamTexture;
        webcamTexture.Play();
    }
}