using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneRecorder : MonoBehaviour
{
    AudioClip recordClip;
    public string[] microphoneNames;
    public string microphoneName;

    // Start is called before the first frame update
    void Start()
    {
        microphoneNames = Microphone.devices;
    }

    // update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            StartRecordMicrophone();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            StopRecordMicrophone();
    }

    void StartRecordMicrophone()
    {
        recordClip = Microphone.Start(microphoneName, true, 100, 44100);
    }

    void StopRecordMicrophone()
    {
        int lastTime = Microphone.GetPosition(null);

        if (lastTime == 0)
            return;
        else
        {
            Microphone.End(Microphone.devices[0]);

            float[] samples = new float[recordClip.samples];

            recordClip.GetData(samples, 0);

            float[] cutSamples = new float[lastTime];

            Array.Copy(samples, cutSamples, cutSamples.Length - 1);

            recordClip = AudioClip.Create("Notice", cutSamples.Length, 1, 44100, false);

            recordClip.SetData(cutSamples, 0);

            SavWav.Save("Notice.wav", recordClip);
        }
    }
}
