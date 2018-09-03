using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechToText : MonoBehaviour {

    DictationRecognizer dictationRecognizer;


    // Use this for initialization
    void Start()
    {
        foreach (string device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }

        dictationRecognizer = new DictationRecognizer();

        dictationRecognizer.AutoSilenceTimeoutSeconds = 5f;
        dictationRecognizer.InitialSilenceTimeoutSeconds = 0f; 

        dictationRecognizer.DictationResult += onDictationResult;
        dictationRecognizer.DictationHypothesis += onDictationHypothesis;
        dictationRecognizer.DictationComplete += onDictationComplete;
        dictationRecognizer.DictationError += onDictationError;

        dictationRecognizer.Start();
    }

    void onDictationResult(string text, ConfidenceLevel confidence)
    {
        // write your logic here
        Debug.LogFormat("Dictation result: " + text);
    }

    void onDictationHypothesis(string text)
    {
        // write your logic here
        Debug.Log("Hypotheses");
        Debug.LogFormat("Dictation hypothesis: {0}", text);
    }

    void onDictationComplete(DictationCompletionCause cause)
    {
        // write your logic here
        if (cause != DictationCompletionCause.Complete)
            Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", cause);

        dictationRecognizer.Start();
    }

    void onDictationError(string error, int hresult)
    {
        // write your logic here
        Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
    }

    // Update is called once per frame
    void Update () {
        //Debug.Log("Started recognizer with PhraseRecognitionSystem as : " + PhraseRecognitionSystem.Status);

        //Debug.Log("Dictation Recognizer status : "  + dictationRecognizer.Status);
    }
}
