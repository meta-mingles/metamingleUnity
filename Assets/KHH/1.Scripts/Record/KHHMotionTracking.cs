using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class KHHMotionTracking : MonoBehaviour
{
    public GameObject[] motionTracks;
    Dictionary<string, Transform> motionTrackTargetDic = new Dictionary<string, Transform>();
    Dictionary<string, Vector3> motionTrackPosDic = new Dictionary<string, Vector3>();

    private void Start()
    {
        foreach (var mt in motionTracks)
        {
            motionTrackTargetDic.Add(mt.name, mt.transform);
            motionTrackPosDic.Add(mt.name, new Vector3());
        }
    }

    private void Update()
    {
        foreach (KeyValuePair<string, Transform> kvp in motionTrackTargetDic)
        {
            if (motionTrackPosDic.ContainsKey(kvp.Key))
            {
                kvp.Value.position = Vector3.Lerp(kvp.Value.position, motionTrackPosDic[kvp.Key], 0.5f);
            }
        }
    }

    public void MotionTracking(string data)
    {
        List<string> stringsToRemove = new List<string>() { " ", "[", "]", "{", "}", "\'" };

        foreach (string s in stringsToRemove)
        {
            data = data.Replace(s, string.Empty);
        }

        string[] datas = data.Split(':', ',');
        if (datas.Length % 4 != 0)
            return;

        Debug.Log(data);

        int length = datas.Length / 4;
        for (int i = 0; i < length; i++)
        {
            string key = datas[i * 4];
            float x = float.Parse(datas[i * 4 + 1]);
            float y = float.Parse(datas[i * 4 + 2]);
            float z = float.Parse(datas[i * 4 + 3]);

            if (motionTrackPosDic.ContainsKey(key))
            {
                motionTrackPosDic[key] = new Vector3(-x, -y, z)*0.002f;
            }
        }
    }
}