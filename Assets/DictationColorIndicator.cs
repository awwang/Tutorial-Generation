using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using SimpleJSON;

/// <summary>
/// Handles displayed text (dictation result, camera status, step number).
/// Additionally writes to JSON file.
/// </summary>
public class DictationColorIndicator : MonoBehaviour, IDictationHandler {

    /// <summary>
    /// Renderer of DictationIndicator object
    /// </summary>
    private new Renderer renderer;

    /// <summary>
    /// TextMesh displaying dictation result
    /// </summary>
    public TextMesh dictationOutputText;

    /// <summary>
    /// Step number displayed
    /// </summary>
    public TextMesh stepCountDisplayText;
    //public GameObject objectToBeManipulated;

    /// <summary>
    /// Camera status displayed to user
    /// </summary>
    public TextMesh cameraStatusText;

    /// <summary>
    /// Step count (initialized at 1)
    /// </summary>
    public int stepCount = 1;

    /// <summary>
    /// Recording status
    /// </summary>
    private bool isRecording;

    /// <summary>
    /// String representing JSON file with recorded steps
    /// </summary>
    private static string blankJSON = System.IO.File.ReadAllText(@"Assets\BlankJSON.JSON");
    JSONNode data = SimpleJSON.JSON.Parse(blankJSON);

    /// <summary>
    /// Get renderer at start
    /// </summary>
    void Awake () {
        renderer = GetComponent<Renderer>();
	}

    /// <summary>
    /// Beginning dictation on voice command
    /// </summary>
    public void OnDictationStart() {
        renderer.material.color = Color.red;
        dictationOutputText.color = Color.red;
        ToggleRecording(); // toggle dictation recording
    }

    /// <summary>
    /// Begin next step. Reset text and increment counter.
    /// </summary>
    public void OnNextStep() {
        stepCount++; //increment step counter
        stepCountDisplayText.text = "Step " + stepCount.ToString(); // update step number
        dictationOutputText.text = "Say, \"Start recording\" to record text."; // reset instructional text
        cameraStatusText.text = "Camera ready"; // reset camera status
        renderer.material.color = Color.white;
        dictationOutputText.color = Color.white;
    }

    /// <summary>
    /// When dictation is complete, end recording.
    /// </summary>
    public void OnDictationComplete(DictationEventData eventData)
    {
        dictationOutputText.text = eventData.DictationResult; // record result

        isRecording = false; // recording status false
        StartCoroutine(DictationInputManager.StopRecording()); // end recording
        renderer.material.color = Color.green;
        dictationOutputText.color = Color.green;
    }

    /// <summary>
    /// Toggle recording (begin if not recording, end if still recording)
    /// </summary>
    private void ToggleRecording()
    {
        if (isRecording)
        {
            isRecording = false;
            StartCoroutine(DictationInputManager.StopRecording());
            Debug.LogWarning("recording stopped");
            dictationOutputText.color = Color.green;
        }
        else
        {
            isRecording = true; // set recording status to true
            StartCoroutine(DictationInputManager.StartRecording(5f, 2f, 30)); // begin recording
            dictationOutputText.color = Color.red;
            renderer.material.color = Color.red;
        }
    }

    /// <summary>
    /// When dictation manager gets hypothesis, set text to that hypothesis.
    /// </summary>
    public void OnDictationHypothesis(DictationEventData eventData)
    {
        dictationOutputText.text = eventData.DictationResult;
    }

    /// <summary>
    /// When dictation manager gets result, set text to that result.
    /// </summary>
    public void OnDictationResult(DictationEventData eventData)
    {
        dictationOutputText.text = eventData.DictationResult;
    }

    /// <summary>
    /// If dictation manager encounters error, end recording.
    /// </summary>
    public void OnDictationError(DictationEventData eventData)
    {
        isRecording = false; // set recording status to false
        dictationOutputText.color = Color.red;
        renderer.material.color = Color.red;
        StartCoroutine(DictationInputManager.StopRecording()); // end recording
    }
}
