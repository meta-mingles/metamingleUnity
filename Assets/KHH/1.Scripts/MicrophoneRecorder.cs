using System;
using UnityEngine;

public class MicrophoneRecorder : MonoBehaviour
{
    public static MicrophoneRecorder Instance;

    public bool IsRecording { get { return Microphone.IsRecording(microphoneName); } }

    AudioClip recordClip;

    string microphoneName;
    public string MicrophoneName { get { return microphoneName; } set { microphoneName = value; } }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetMicrophone();
    }

    void SetMicrophone()
    {
        if (Microphone.devices.Length == 0)
        {
            microphoneName = null;
            return;
        }

        microphoneName = PlayerPrefs.GetString("MicrophoneName", Microphone.devices[0]);
        for (int i = 0; i < Microphone.devices.Length; i++)
            if (Microphone.devices[i] == microphoneName)
                return;
        microphoneName = Microphone.devices[0];
    }

    public void StartRecordMicrophone()
    {
        recordClip = Microphone.Start(microphoneName, true, 100, 44100);
    }

    public void StopRecordMicrophone(string fileName)
    {
        int lastTime = Microphone.GetPosition(microphoneName);

        if (lastTime == 0)
            return;
        else
        {
            Microphone.End(microphoneName);

            float[] samples = new float[recordClip.samples];

            recordClip.GetData(samples, 0);

            float[] cutSamples = new float[lastTime];

            Array.Copy(samples, cutSamples, cutSamples.Length); //Length-1

            recordClip = AudioClip.Create(fileName, cutSamples.Length, 1, 44100, false);

            recordClip.SetData(cutSamples, 0);

            SaveLoadWav.Save(KHHVideoData.FileMotionPath + "/" + fileName + ".wav", recordClip);
        }
    }
}
