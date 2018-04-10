using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using json = SimpleJSON;

public class DictationColorIndicator : MonoBehaviour, IDictationHandler {

    private new Renderer renderer;
    public TextMesh dictationOutputText;
    public TextMesh stepCountDisplayText;
    //public GameObject objectToBeManipulated;
    public TextMesh cameraStatusText;
    public int stepCount = 1;
    private bool isRecording;
    public string allSteps;
	// Use this for initialization
	void Awake () {
        renderer = GetComponent<Renderer>();
	}

    public void OnDictationStart() {
        //renderer = objectToBeManipulated.GetComponent<Renderer>();
        renderer.material.color = Color.red;
        dictationOutputText.color = Color.red;
        ToggleRecording();
    }

    public void OnNextStep() {
        stepCount++;
        stepCountDisplayText.text = "Step " + stepCount.ToString();
        //renderer = objectToBeManipulated.GetComponent<Renderer>();
        dictationOutputText.text = "Say, \"Start recording\" to record text.";
        cameraStatusText.text = "Camera ready";
        renderer.material.color = Color.white;
        dictationOutputText.color = Color.white;
    }

    public void OnDictationComplete(DictationEventData eventData)
    {
        dictationOutputText.text = eventData.DictationResult;

        isRecording = false;
        StartCoroutine(DictationInputManager.StopRecording());
        renderer.material.color = Color.green;
        dictationOutputText.color = Color.green;
    }

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
            isRecording = true;
            StartCoroutine(DictationInputManager.StartRecording(5f, 2f, 30));
            dictationOutputText.color = Color.red;
            renderer.material.color = Color.red;
        }
    }

    public void OnDictationHypothesis(DictationEventData eventData)
    {
        dictationOutputText.text = eventData.DictationResult;
    }

    public void OnDictationResult(DictationEventData eventData)
    {
        dictationOutputText.text = eventData.DictationResult;
    }

    public void OnDictationError(DictationEventData eventData)
    {
        isRecording = false;
        dictationOutputText.color = Color.red;
        renderer.material.color = Color.red;
        StartCoroutine(DictationInputManager.StopRecording());
    }
}
