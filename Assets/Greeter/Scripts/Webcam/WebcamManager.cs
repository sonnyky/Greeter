using UnityEngine;
using UnityEngine.UI;

public class WebcamManager : MonoBehaviour
{
    RawImage rawimage;

    WebCamDevice[] devices;

    WebCamTexture _CamTex;

    int _CaptureCounter = 0;

    private Button m_CaptureButton;

    void Start()
    {
        devices = WebCamTexture.devices;

        rawimage = GameObject.FindGameObjectWithTag("WebcamScreen").GetComponent<RawImage>();

        m_CaptureButton = GameObject.FindGameObjectWithTag("DetectionButton").GetComponent<Button>();
        m_CaptureButton.onClick.AddListener(()=> {
            TakeSnapshot();
        });
      
        // Stream from the first camera device
        StreamCamera(devices[0].name);

    }

    public void TakeSnapshot()
    {

        string buttonTag = m_CaptureButton.gameObject.tag;
        switch (buttonTag)
        {
            case "DetectionButton":
                Debug.Log("Capture button pressed during detection");
                break;
            case "RegistrationButton":
                Texture2D snap = new Texture2D(_CamTex.width, _CamTex.height);
                snap.SetPixels(_CamTex.GetPixels());
                snap.Apply();

                System.IO.File.WriteAllBytes(Application.dataPath + Constants.PREFIX_TRAIN_IMAGES_PATH + _CaptureCounter.ToString() + ".jpg", snap.EncodeToJPG());
                ++_CaptureCounter;
                break;
            default:
                break;
        }
        
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