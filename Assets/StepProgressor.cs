using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepProgressor : MonoBehaviour
{

    TextMesh textMesh;
    int stepCtr = 0;

    // Use this for initialization
    void Start()
    {
        textMesh = GetComponent<TextMesh>();
    }
    public void NextStep()
    {
        stepCtr++;
        textMesh.color = Color.blue;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
