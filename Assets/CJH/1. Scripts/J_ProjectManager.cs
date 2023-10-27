using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_ProjectManager : MonoBehaviour
{
    public static J_ProjectManager instance;

    public UserInfo userInfo;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
