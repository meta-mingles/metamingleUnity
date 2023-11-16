using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KHHSelectManager : MonoBehaviour
{
    public Button createButton;

    public Transform videoItemParent;
    public KHHVideoItem videoItemPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (createButton != null)
            createButton.onClick.AddListener(() =>
            {
                //임시 제목 생성
                int index = 0;
                string workingTitle = $"제목 없는 동영상 {index}";
                while (System.IO.Directory.Exists(workingTitle))
                {
                    index++;
                    workingTitle = $"제목 없는 동영상 {index}";
                    System.IO.Directory.CreateDirectory(workingTitle);
                }

                KHHEditData.Open(workingTitle);
                GlobalValue.PrevSceneName = SceneManager.GetActiveScene().name;
                GlobalValue.CurSceneName = "ToolCapture";
                SceneManager.LoadScene("ToolCapture");
            });

        string[] directorys = System.IO.Directory.GetDirectories(Application.persistentDataPath);
        for (int i = 0; i < directorys.Length; i++)
            CreateVideoItem(directorys[i]);
    }

    void CreateVideoItem(string path)
    {
        var item = Instantiate(videoItemPrefab, videoItemParent);
        item.SetData(path);
    }


    //// Update is called once per frame
    //void Update()
    //{

    //}
}
