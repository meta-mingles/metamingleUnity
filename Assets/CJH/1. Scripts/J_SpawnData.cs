using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class EnemySpawnData
{
    //아이디
    public int id = 0;    //에너미      ---H
    //스텝
    public int step = 0;  //스테이지 별  ---S
    //생성시간
    public float createTime = 0f;    //--- V
    //위치
    public Vector3 position = Vector3.zero; //---Color
    //회전방향
    public Vector3 rotation = Vector3.zero;
    //기본 장비상태
    public int defaultWeapon = 0;
}

[Serializable]
public class StageSpawnData
{
    public string SceneName = "";
    public EnemySpawnData[] enemySpawnDatas;
}

[CreateAssetMenu(fileName = "J_SpawnData", menuName = "Scriptable Object/J_SpawnData")]
public class J_SpawnData : ScriptableObject
{
    public int stageCount = 0;
    public GameObject pistol;
    public GameObject bat;
    public GameObject katana;
    public StageSpawnData[] stageSpawnDatas;

    public GameObject GetWeapon(int defaultWeapon)
    {
        switch (defaultWeapon)
        {
            case 1:
                return pistol;
            case 2:
                return bat;
            case 3:
                return katana;
        }

        return null;
    }
}