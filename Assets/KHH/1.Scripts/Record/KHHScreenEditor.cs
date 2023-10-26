using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KHHScreenEditor : MonoBehaviour
{
    bool fileLoaded = false;
    public bool FileLoaded { get { return fileLoaded; } }

    bool isPlaying = false;
    public bool IsPlaying { get { return isPlaying; } set { isPlaying = value; } }

    public KHHModelRecorder recorder;

    List<AudioSource> audioSourceList;
    List<AudioClip> audioClipList;

    // Start is called before the first frame update
    void Start()
    {
        audioSourceList = new List<AudioSource>();
        audioClipList = new List<AudioClip>();
    }

    //드롭 아이템이 편집 영역에 드랍되었을 때 호출
    public void OnDropItem(GameObject dropItem)
    {
        fileLoaded = false;

        //드롭 아이템의 파일 이름을 얻어온다.
        string fileName = dropItem.GetComponent<KHHMotionData>().FileName;

        StartCoroutine(LoadFile(fileName));
    }

    public IEnumerator LoadFile(string fileName)
    {
        //모델 레코더에 파일 이름을 전달한다.
        recorder.LoadRecordData(fileName);

        GameObject audioObject = new GameObject();
        audioObject.transform.SetParent(this.transform);
        audioObject.name = "AudioSource";
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        audioSourceList.Add(audioSource);

        yield return StartCoroutine(SaveLoadWav.Load(fileName, audioSource));

        audioClipList.Add(audioSource.clip);

        fileLoaded = true;
    }

    public void Play()
    {
        if (fileLoaded)
        {
            isPlaying = true;
            recorder.StartPlay();
            for (int i = 0; i < audioSourceList.Count; i++)
            {
                audioSourceList[i].clip = audioClipList[i];
                audioSourceList[i].Play();
            }
        }
    }

    public void Stop()
    {
        if (fileLoaded)
        {
            isPlaying = false;
            recorder.StopPlay();
            for (int i = 0; i < audioSourceList.Count; i++)
            {
                audioSourceList[i].Stop();
            }
        }
    }
}
