using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class EnemySpawnData
{
    //���̵�
    public int id = 0;    //���ʹ�      ---H
    //����
    public int step = 0;  //�������� ��  ---S
    //�����ð�
    public float createTime = 0f;    //--- V
    //��ġ
    public Vector3 position = Vector3.zero; //---Color
    //ȸ������
    public Vector3 rotation = Vector3.zero;
    //�⺻ ������
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