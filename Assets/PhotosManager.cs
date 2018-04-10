

/*using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.VR.WSA.WebCam;

/// <summary>
/// Manages taking and saving photos.
/// </summary>
public class PhotosManager : MonoBehaviour
{
    /// <summary>
    /// Displays status information of the camera.
    /// </summary>
    public static TextMesh Info;

    /// <summary>
    /// Actual camera instance.
    /// </summary>
    public static PhotoCapture capture;

    /// <summary>
    /// Captured camera frame.
    /// </summary>
    public static PhotoCaptureFrame frame;

    /// <summary>
    /// Target texture for image.
    /// </summary>
    public static Texture2D targetTexture;

    /// <summary>
    /// True, if the camera is ready to take photos.
    /// </summary>
    private static bool isReady = false;

    /// <summary>
    /// The path to the image in the applications local folder.
    /// </summary>
    private static string currentImagePath;

    /// <summary>
    /// The path to the users picture folder.
    /// </summary>
    private static string pictureFolderPath;

    private static void Start()
    {
        Assert.IsNotNull(Info, "The PhotoManager requires a text mesh.");

        Info.text = "Camera off";
        PhotoCapture.CreateAsync(true, OnPhotoCaptureCreated);

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

    private static void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        capture = captureObject;

        Resolution resolution = PhotoCapture.SupportedResolutions.OrderByDescending(res => res.width * res.height).First();

        CameraParameters c = new CameraParameters(WebCamMode.PhotoMode);
        c.hologramOpacity = 1.0f;
        c.cameraResolutionWidth = resolution.width;
        c.cameraResolutionHeight = resolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        capture.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    private static void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        isReady = result.success;
        Info.text = "Camera ready";
    }


    /// <summary>
    /// Take a photo and save it to a temporary application folder.
    /// </summary>
    public static IEnumerator TakePhoto() //Texture2D targetTexture
    {
        if (isReady)
        {
            string file = string.Format(@"Image_{0:yyyy-MM-dd_hh-mm-ss-tt}.jpg", DateTime.Now);
            currentImagePath = System.IO.Path.Combine(Application.persistentDataPath, file);

            capture.TakePhotoAsync(currentImagePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);
            //frame.UploadImageDataToTexture(targetTexture);
        }
        else
        {
            Debug.LogWarning("The camera is not yet ready.");
        }

        return null;
    }

    /// <summary>
    /// Stop the photo mode.
    /// </summary>
    public static void StopCamera()
    {
        if (isReady)
        {
            capture.StopPhotoModeAsync(OnPhotoModeStopped);
        }
    }

    private static void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
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

    private static void OnPhotoModeStopped(PhotoCapture.PhotoCaptureResult result)
    {
        capture.Dispose();
        capture = null;
        isReady = false;

        Info.text = "Camera off";
    }

}*/