using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� ���� ��ȯ�� �� ����
public class J_SceneSetting : MonoBehaviour
{
    #region  �̱��� , don'tDestroyOnload
    public static J_SceneSetting Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
