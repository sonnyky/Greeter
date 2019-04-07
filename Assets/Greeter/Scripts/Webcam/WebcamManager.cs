using UnityEngine;
using UnityEngine.UI;

public class WebcamManager : MonoBehaviour
{
    RawImage rawimage;

    WebCamDevice[] devices;

    WebCamTexture _CamTex;

    int _CaptureCounter = 0;

    void Start()
    {
        devices = WebCamTexture.devices;

        rawimage = GameObject.FindGameObjectWithTag("WebcamScreen").GetComponent<RawImage>();
      
        // Stream from the first camera device
        StreamCamera(devices[0].name);

    }

    public void TakeSnapshot()
    {
        Texture2D snap = new Texture2D(_CamTex.width, _CamTex.height);
        snap.SetPixels(_CamTex.GetPixels());
        snap.Apply();

        System.IO.File.WriteAllBytes(Application.dataPath + Constants.TRAIN_IMAGES_SAVE_PATH + _CaptureCounter.ToString() + ".jpg", snap.EncodeToJPG());
        ++_CaptureCounter;
    }

    public void StreamCamera(string cameraName)
    {
        WebCamTexture webcamTexture = new WebCamTexture(cameraName);
        rawimage.texture = webcamTexture;
        rawimage.color = Color.white;
        _CamTex = webcamTexture;
        rawimage.material.mainTexture = webcamTexture;
        webcamTexture.Play();
    }
}