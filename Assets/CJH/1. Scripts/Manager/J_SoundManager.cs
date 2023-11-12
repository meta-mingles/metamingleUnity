using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class J_SoundManager : MonoBehaviour
{
    public static J_SoundManager instance;

    public bool isPlaying = true;
    private void Awake()
    {
        if (instance == null & isPlaying)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }



        else
        {
            Destroy(gameObject);
        }

    }

    //브금, 환경음, 효과음
    [SerializeField] AudioSource bgm, amb, sfx;

    //오디오 리스트
    [SerializeField] List<AudioClip> audioList;

    public enum Sounds
    {
        BACKGROUND, AMBIENT,
    }


    //소리 재생
    public void PlaySound(Sounds sound)
    {
        if (sound == Sounds.BACKGROUND)
        {
            bgm.clip = audioList[(int)sound];
            bgm.Play();
        }
        else if (sound == Sounds.AMBIENT)
        {
            amb.clip = audioList[(int)sound];
            amb.Play();
        }
        else
        {
            sfx.PlayOneShot(audioList[(int)sound]);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        //배경 음악 재생
        PlaySound(Sounds.BACKGROUND);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
