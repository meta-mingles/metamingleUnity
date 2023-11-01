using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;

public class KHHEditItemMotion : KHHEditItem
{
    public VNectModel model;

    Dictionary<float, List<(Vector3, Quaternion)>> recordDic;
    public Dictionary<float, List<(Vector3, Quaternion)>> RecordDic { get { return recordDic; } }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (model != null && screenEditor.IsPlaying)
        {
            playTime += Time.deltaTime;
            if (playTime > timeList[endIdx] - itemCorrectTime)
            {
                screenEditor.End();
                return;
            }

            //현재 시간에 가장 근접하는 키 데이터를 추론한다.
            for (int i = curIdx + 1; i < timeList.Count; i++)
            {
                if (timeList[i] - itemCorrectTime > playTime)
                {
                    curIdx = i - 1;
                    break;
                }
            }

            float ratio = (playTime - (timeList[curIdx] - itemCorrectTime)) / (timeList[curIdx + 1] - timeList[curIdx]);
            //모델 위치 조정
            for (int i = 0; i < model.JointPoints.Length; i++)
            {
                Vector3 pos = Vector3.Lerp(recordDic[timeList[curIdx]][i].Item1, recordDic[timeList[curIdx + 1]][i].Item1, ratio);
                Quaternion rot;
                if ((recordDic[timeList[curIdx]][i].Item2.normalized == Quaternion.identity) && (recordDic[timeList[curIdx + 1]][i].Item2.normalized == Quaternion.identity)) rot = Quaternion.identity; //recordList[timeList[curIdx + 1]][i].Item2;
                else rot = Quaternion.Lerp(recordDic[timeList[curIdx]][i].Item2, recordDic[timeList[curIdx + 1]][i].Item2, ratio);
                model.JointPoints[i].Pos3D = pos;
                model.JointPoints[i].InverseRotation = rot;
            }
        }
    }

    public void Init(VNectModel model)
    {
        this.model = model;
        type = KHHData.DataType.MotionData;
    }

    //public void Load(KHHEditItemMotion motion)
    //{
    //    timeList = motion.TimeList;
    //    recordDic = motion.RecordDic;
    //}

    public override void LoadRecordData(KHHScreenEditor editor, string fileName)
    {
        base.LoadRecordData(editor, fileName);
        string[,] motionData = CSVManager.Instance.ReadCsv(fileName);

        timeList = new List<float>();
        recordDic = new Dictionary<float, List<(Vector3, Quaternion)>>();

        for (int i = 1; i < motionData.GetLength(1); i++)
        {
            if (string.IsNullOrEmpty(motionData[0, i])) continue;
            float time = float.Parse(motionData[0, i]);
            List<(Vector3, Quaternion)> jointData = new List<(Vector3, Quaternion)>();
            for (int j = 1; j < motionData.GetLength(0); j++)
            {
                if (string.IsNullOrEmpty(motionData[j, i])) continue;
                string[] jointDatas = motionData[j, i].Split('_');
                if (jointDatas.Length != 7) continue;
                //Debug.Log(jointDatas[0] + "_" + jointDatas[1] + "_" + jointDatas[2]);
                Vector3 pos = new Vector3(float.Parse(jointDatas[0]), float.Parse(jointDatas[1]), float.Parse(jointDatas[2]));
                Quaternion rot = new Quaternion(float.Parse(jointDatas[3]), float.Parse(jointDatas[4]), float.Parse(jointDatas[5]), float.Parse(jointDatas[6]));
                jointData.Add((pos, rot));
            }

            timeList.Add(time);
            recordDic.Add(time, jointData);
        }

        startX = timeList[0] * lengthScale;
        endX = timeList[timeList.Count - 1] * lengthScale;
        maxLength = endX - startX;
        curIdx = 0;

        ////오디오
        //GameObject audioObject = new GameObject();
        //audioObject.transform.SetParent(this.transform);
        //audioObject.name = "AudioSource";
        //AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        //audioSource.playOnAwake = false;
        //yield return StartCoroutine(SaveLoadWav.Load(fileName, audioSource));

    }
}
