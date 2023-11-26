using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KHHEditItemMotion : KHHEditItem
{
    VNectModel model;
    public VNectModel Model { set { model = value; } }
    VisualizerTest hand;
    public VisualizerTest Hand { set { hand = value; } }
    //AudioSource audioSource;

    protected int curIdx = 0;
    protected int startIdx = 0;
    protected int endIdx = 0;

    protected List<float> timeList;
    protected List<float> handTimeList;
    Dictionary<float, List<(Vector3, Quaternion)>> recordDic;
    Dictionary<float, List<Quaternion>> handRecordDic;

    KHHEditItemSound pairSound;
    public KHHEditItemSound PairSound { set { pairSound = value; } }
    //private void Awake()
    //{
    //    audioSource = GetComponent<AudioSource>();
    //}

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (model != null && screenEditor.IsPlaying)
        {
            float actionTime = screenEditor.playTime - delayTime;
            if (actionTime > timeList[endIdx] - itemCorrectTime)
            {
                //audioSource.Stop();
                return;
            }

            //현재 시간에 가장 근접하는 키 데이터를 추론한다.
            for (int i = curIdx + 1; i < timeList.Count; i++)
            {
                if (timeList[i] - itemCorrectTime > actionTime)
                {
                    curIdx = i - 1;
                    break;
                }
            }

            float ratio = (actionTime - (timeList[curIdx] - itemCorrectTime)) / (timeList[curIdx + 1] - timeList[curIdx]);
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

            //손 위치 조정
            ratio = (actionTime - (handTimeList[curIdx] - itemCorrectTime)) / (handTimeList[curIdx + 1] - handTimeList[curIdx]);
            for (int i = 0; i < VisualizerTest.JointNums.Length; i++)
            {
                Quaternion rot;
                if ((handRecordDic[handTimeList[curIdx]][i].normalized == Quaternion.identity) && (handRecordDic[handTimeList[curIdx + 1]][i].normalized == Quaternion.identity)) rot = Quaternion.identity; //recordList[timeList[curIdx + 1]][i].Item2;
                else rot = Quaternion.Lerp(handRecordDic[handTimeList[curIdx]][i], handRecordDic[handTimeList[curIdx + 1]][i], ratio);

                //if (hand.HandJoints[i].Transform != null)
                hand.HandJoints[VisualizerTest.JointNums[i]].Transform.localRotation = rot;
            }
        }

        if (isSelected)
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                KHHEditManager.Instance.StopButtonEvent();
                Remove();
            }
        }
    }

    //public void Load(KHHEditItemMotion motion)
    //{
    //    timeList = motion.TimeList;
    //    recordDic = motion.RecordDic;
    //}

    public override void PlayStart()
    {
        base.PlayStart();

        //시작 시간 찾기
        float itemStartTime = (startX + changeLeftX) / lengthScale;
        float itemEndTime = (endX + changeRightX) / lengthScale;

        startIdx = 0;
        endIdx = timeList.Count - 1;

        for (int i = 0; i < timeList.Count; i++)
        {
            if (timeList[i] < itemStartTime)
                startIdx = i;
            if (timeList[i] < itemEndTime)
                endIdx = i;
        }

        curIdx = startIdx;
        //사운드 시작 지점 설정
        //audioSource.time = itemCorrectTime;
        //audioSource.PlayDelayed(delayTime);
    }

    public override void PlayStop()
    {
        base.PlayStop();
        //audioSource.Stop();
    }

    public override void Remove()
    {
        PlayerPrefs.DeleteKey($"{KHHEditData.VideoTitle}M");
        PlayerPrefs.DeleteKey($"{KHHEditData.VideoTitle}MCX");
        PlayerPrefs.DeleteKey($"{KHHEditData.VideoTitle}MCLX");
        PlayerPrefs.DeleteKey($"{KHHEditData.VideoTitle}MCRX");
        PlayerPrefs.DeleteKey($"{KHHEditData.VideoTitle}MVCX");
        PlayerPrefs.DeleteKey($"{KHHEditData.VideoTitle}MVCLX");
        PlayerPrefs.DeleteKey($"{KHHEditData.VideoTitle}MVCRX");
        PlayerPrefs.DeleteKey($"{KHHEditData.VideoTitle}MVV");
        screenEditor.RemoveItem(this);
        screenEditor.RemoveItem(pairSound);
        Destroy(gameObject);
        Destroy(pairSound.gameObject);
    }

    public override void LoadItemData(KHHScreenEditor editor, string filePath, string fileName, UnityAction action)
    {
        base.LoadItemData(editor, filePath, fileName, action);
        string[,] motionData = CSVManager.Instance.ReadCsv(filePath + ".csv");
        string[,] handData = CSVManager.Instance.ReadCsv(filePath + "_2.csv");

        timeList = new List<float>();
        handTimeList = new List<float>();
        recordDic = new Dictionary<float, List<(Vector3, Quaternion)>>();
        handRecordDic = new Dictionary<float, List<Quaternion>>();

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

        for (int i = 1; i < handData.GetLength(1); i++)
        {
            if (string.IsNullOrEmpty(handData[0, i])) continue;
            float time = float.Parse(handData[0, i]);
            List<Quaternion> jointData = new List<Quaternion>();
            for (int j = 1; j < handData.GetLength(0); j++)
            {
                if (string.IsNullOrEmpty(handData[j, i]))
                {
                    jointData.Add(quaternion.identity);
                    continue;
                }
                string[] jointDatas = handData[j, i].Split('_');
                Quaternion rot;
                rot = new Quaternion(float.Parse(jointDatas[0]), float.Parse(jointDatas[1]), float.Parse(jointDatas[2]), float.Parse(jointDatas[3]));
                jointData.Add(rot);
            }

            handTimeList.Add(time);
            handRecordDic.Add(time, jointData);
        }

        curIdx = 0;
        startX = timeList[0] * lengthScale;
        endX = timeList[timeList.Count - 1] * lengthScale;
        maxLength = endX - startX;

        action?.Invoke();
        Set();
        //오디오 로드
        //StartCoroutine(CoLoadAudioData(fileName, action));
    }

    //IEnumerator CoLoadAudioData(string fileName, UnityAction action)
    //{
    //    yield return StartCoroutine(SaveLoadWav.Load(KHHVideoData.FileMotionPath + "/" + fileName + ".wav", audioSource));
    //    action?.Invoke();
    //    Set();
    //}

    protected override void DragEndLeft()
    {
        KHHEditVideoState.MotionChangeLeftX = changeLeftX;
    }

    protected override void DragEndMiddle()
    {
        KHHEditVideoState.MotionChangeX = changePosX;
    }

    protected override void DragEndRight()
    {
        KHHEditVideoState.MotionChangeRightX = changeRightX;
    }

    public override void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        foreach (var outline in outlines)
            outline.enabled = true;
        if (pairSound != null) pairSound.IsSelected = true;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        foreach (var outline in outlines)
            outline.enabled = false;
        if (pairSound != null) pairSound.IsSelected = false;
    }
}
