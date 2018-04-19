using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.VR.WSA.WebCam;

/// <summary>
/// Manages taking and saving photos.
/// </summary>
public class ImageCapture : MonoBehaviour
{
    /// <summary>
    /// Displays status information of the camera.
    /// </summary>
    public TextMesh Info;

    /// <summary>
    /// Actual camera instance.
    /// </summary>
    private PhotoCapture capture;
    private Texture2D targetTexture = null;
    public GameObject quad;

    /// <summary>
    /// True, if the camera is ready to take photos.
    /// </summary>
    private bool isReady = false;

    /// <summary>
    /// The path to the image in the applications local folder.
    /// </summary>
    private string currentImagePath;

    /// <summary>
    /// The path to the users picture folder.
    /// </summary>
    private string pictureFolderPath;

    private void Start()
    {
        Assert.IsNotNull(Info, "The PhotoManager requires a text mesh.");

        Info.text = "Camera off";
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);

#if NETFX_CORE
        getPicturesFolderAsync();
#endif

    }

#if NETFX_CORE

    private async void getPicturesFolderAsync() {
        Windows.Storage.StorageLibrary picturesStorage = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Pictures);
        pictureFolderPath = picturesStorage.SaveFolder.Path;
    }

#endif

    private void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {

        capture = captureObject;

        Resolution resolution = PhotoCapture.SupportedResolutions.OrderByDescending(res => res.width * res.height).First();
        targetTexture = new Texture2D(resolution.width, resolution.height);

        CameraParameters c = new CameraParameters(WebCamMode.PhotoMode);
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = resolution.width;
        c.cameraResolutionHeight = resolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        capture.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        isReady = result.success;
        Info.text = "Camera ready";
    }


    /// <summary>
    /// Take a photo and save it to a temporary application folder.
    /// </summary>
    public void TakePhoto()
    {
        if (isReady)
        {
            string file = string.Format(@"Image_{0:yyyy-MM-dd_hh-mm-ss-tt}.jpg", DateTime.Now);
            currentImagePath = System.IO.Path.Combine(Application.persistentDataPath, file);

            capture.TakePhotoAsync(currentImagePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);
            capture.TakePhotoAsync(OnCapturedPhotoToTexture);
        }
        else
        {
            Debug.LogWarning("The camera is not yet ready.");
        }
    }

    /// <summary>
    /// Stop the photo mode.
    /// </summary>
    public void StopCamera()
    {
        if (isReady)
        {
            capture.StopPhotoModeAsync(OnPhotoModeStopped);
        }
    }

    private void OnCapturedPhotoToTexture(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        // Copy the raw image data into our target texture
        photoCaptureFrame.UploadImageDataToTexture(targetTexture);

        // Create a gameobject that we can apply our texture to
        //GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Renderer quadRenderer = quad.GetComponent<Renderer>() as Renderer;
        //quadRenderer.material = new Material(Shader.Find("UnlitShader"));

        //quad.transform.parent = this.transform;
        //quad.transform.localPosition = new Vector3(0.0f, 0.0f, 3.0f);
        quadRenderer.material.SetTexture("_MainTex", targetTexture);
    }
    private void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
#if NETFX_CORE
            try 
            {
                if(pictureFolderPath != null)
                {
                    System.IO.File.Move(currentImagePath, System.IO.Path.Combine(pictureFolderPath, "Camera Roll", System.IO.Path.GetFileName(currentImagePath)));
                    Info.text = "Saved photo in camera roll";
                }
                else 
                {
                    Info.text = "Saved photo to temp";
                }
            } 
            catch(Exception e) 
            {
                Info.text = "Failed to move to camera roll";
            }
#else
            Info.text = "Saved photo";
            Debug.Log("Saved image at " + currentImagePath);
#endif

        }
        else
        {
            Info.text = "Failed to save photo";
            Debug.LogError(string.Format("Failed to save photo to disk ({0})", result.hResult));
        }
    }

    private void OnPhotoModeStopped(PhotoCapture.PhotoCaptureResult result)
    {
        capture.Dispose();
        capture = null;
        isReady = false;

        Info.text = "Camera off";
    }

}
/*using UnityEngine;
using json = SimpleJSON;

public class ImageCapture : MonoBehaviour
{

    private new Renderer renderer;

    // Use this for initialization
    void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    public void OnSnap()
    {
        StartCoroutine(PhotosManager.TakePhoto());
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Renderer quadRenderer = quad.GetComponent<Renderer>() as Renderer;
        quadRenderer.material = new Material(Shader.Find("Custom/Unlit/UnlitTexture"));

        quad.transform.parent = this.transform;
        quad.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

        quadRenderer.material.SetTexture("_MainTex", PhotosManager.targetTexture);
        Debug.LogWarning("image taken");
    }
}
*/
