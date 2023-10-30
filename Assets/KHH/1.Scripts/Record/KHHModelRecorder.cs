using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KHHModelRecorder : MonoBehaviour
{
    public VNectModel model;
    public KHHScreenEditor screenEditor;

    bool isRecording = false;

    //��ȭ
    List<string[]> recordData;
    float recordTime = 0.0f;

    //���
    float playTime = 0.0f;
    int curIdx = 0;
    List<float> timeList;
    public List<float> TimeList { get { return timeList; } }
    Dictionary<float, List<(Vector3, Quaternion)>> recordDic;
    public Dictionary<float, List<(Vector3, Quaternion)>> RecordDic { get { return recordDic; } }

    // Update is called once per frame
    void Update()
    {
        if (model != null && screenEditor.IsPlaying)
        {
            playTime += Time.deltaTime;
            if (playTime > timeList[timeList.Count - 1])
            {
                screenEditor.IsPlaying = false;
                return;
            }

            //���� �ð��� ���� �����ϴ� Ű �����͸� �߷��Ѵ�.
            for (int i = curIdx + 1; i < timeList.Count; i++)
            {
                if (timeList[i] > playTime)
                {
                    curIdx = i - 1;
                    break;
                }
            }

            float ratio = (playTime - timeList[curIdx]) / (timeList[curIdx + 1] - timeList[curIdx]);
            //�� ��ġ ����
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

    private void LateUpdate()
    {
        if (model != null && isRecording)
        {
            recordTime += Time.deltaTime;

            //�� ��ȭ
            string[] curJointData = new string[PositionIndex.Count.Int() + 1];
            curJointData[0] = recordTime.ToString();
            for (int i = 1; i < curJointData.Length; i++)
                curJointData[i] = $"{model.JointPoints[i - 1].Pos3D.x}_{model.JointPoints[i - 1].Pos3D.y}_{model.JointPoints[i - 1].Pos3D.z}_{model.JointPoints[i - 1].InverseRotation.x}_{model.JointPoints[i - 1].InverseRotation.y}_{model.JointPoints[i - 1].InverseRotation.z}_{model.JointPoints[i - 1].InverseRotation.w}";
            recordData.Add(curJointData);
        }
    }

    //��ȭ ����
    public void StartRecord()
    {
        isRecording = true;
        recordTime = 0.0f;
        recordData = new List<string[]>();
        string[] datas = new string[PositionIndex.Count.Int() + 1];
        datas[0] = "time";
        for (int i = 1; i < datas.Length; i++)
            datas[i] = ((PositionIndex)(i - 1)).ToString();
        recordData.Add(datas);

        //������ ��ġ ����
        string[] curJointData = new string[PositionIndex.Count.Int() + 1];
        curJointData[0] = recordTime.ToString();
        for (int i = 1; i < curJointData.Length; i++)
            curJointData[i] = $"{model.JointPoints[i - 1].Pos3D.x}_{model.JointPoints[i - 1].Pos3D.y}_{model.JointPoints[i - 1].Pos3D.z}_{model.JointPoints[i - 1].InverseRotation.x}_{model.JointPoints[i - 1].InverseRotation.y}_{model.JointPoints[i - 1].InverseRotation.z}_{model.JointPoints[i - 1].InverseRotation.w}";
        recordData.Add(curJointData);
    }

    public void StopRecord(string fileName)
    {
        isRecording = false;
        //TestFileName = fileName;
        CSVManager.Instance.WriteCsv(fileName, recordData);
    }

    public void Load(KHHEditItemMotion motion)
    {
        timeList = motion.TimeList;
        recordDic = motion.RecordDic;
    }
}
