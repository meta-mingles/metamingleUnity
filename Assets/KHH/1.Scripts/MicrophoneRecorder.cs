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

    public void StartRecordMicrophone()
    {
        recordClip = Microphone.Start(microphoneName, true, 100, 44100);
    }

    public void StopRecordMicrophone(string fileName)
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

            recordClip = AudioClip.Create(fileName, cutSamples.Length, 1, 44100, false);

            recordClip.SetData(cutSamples, 0);

            SaveLoadWav.Save(fileName, recordClip);
        }
    }
}
