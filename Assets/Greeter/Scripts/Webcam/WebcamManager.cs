using UnityEngine;
using UnityEngine.UI;

public class WebcamManager : MonoBehaviour
{
    RawImage rawimage;

    WebCamDevice[] devices;

    WebCamTexture _CamTex;

    int _CaptureCounter = 0;

    private Button m_CaptureButton;

    string m_RuntimeImage;

    // Delegate and Events to update photo counter
    public delegate void PhotoCapture();
    public static event PhotoCapture RegisterPhoto;

    public void OnPhotoCapture()
    {
        RegisterPhoto();
    }


    void Start()
    {
        devices = WebCamTexture.devices;

        rawimage = GameObject.FindGameObjectWithTag("WebcamScreen").GetComponent<RawImage>();

        m_RuntimeImage = Application.dataPath + "/Images/Runtime/";

        m_CaptureButton = GameObject.Find("Capture").GetComponent<Button>();
        m_CaptureButton.onClick.AddListener(() => {
            TakeSnapshot();
        });

        // Stream from the first camera device
        StreamCamera(devices[0].name);

    }

    public void TakeSnapshot()
    {

        string buttonTag = m_CaptureButton.gameObject.tag;
        Texture2D snap = new Texture2D(_CamTex.width, _CamTex.height);

        switch (buttonTag)
        {
            case "DetectionButton":

                snap.SetPixels(_CamTex.GetPixels());
                snap.Apply();
                m_RuntimeImage = Application.dataPath + Constants.PREFIX_DETECTION_IMAGES_PATH;
                System.IO.File.WriteAllBytes(m_RuntimeImage + "main.jpg", snap.EncodeToJPG());
                Debug.Log("Capture button pressed during detection");
                break;

            case "RegistrationButton":
                
                snap.SetPixels(_CamTex.GetPixels());
                snap.Apply();
                string newPersonFolder = Application.dataPath + Constants.PREFIX_TRAIN_IMAGES_PATH + Constants.PREFIX_TRAIN_IMAGE_NAME + _CaptureCounter.ToString();
                Folders.Create(newPersonFolder);
                System.IO.File.WriteAllBytes(newPersonFolder + "/" + _CaptureCounter.ToString() + ".jpg", snap.EncodeToJPG());
                ++_CaptureCounter;
                Debug.Log("Capture button pressed during registration");
                
                OnPhotoCapture();
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

    public void StopCamera()
    {
        _CamTex.Stop();
    }

    public string GetRuntimeImagePath()
    {
        return m_RuntimeImage;
    }
}