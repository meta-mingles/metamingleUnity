using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KHHVideoItem : MonoBehaviour
{
    Button button;
    Button deleteButton;
    TextMeshProUGUI nameText;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null) button.onClick.AddListener(() =>
        {
            KHHEditData.Open(nameText.text);
            GlobalValue.PrevSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            GlobalValue.CurSceneName = "ToolCapture";
            UnityEngine.SceneManagement.SceneManager.LoadScene("ToolCapture");
        });

        deleteButton = transform.Find("DeleteButton").GetComponent<Button>();
        if (deleteButton != null) deleteButton.onClick.AddListener(DeleteButtonEvent);

        nameText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetData(string path)
    {
        nameText.text = System.IO.Path.GetFileName(path);

        if (System.IO.File.Exists(path + "/thumbnail.png"))
        {
            Texture2D texture = new Texture2D(0, 0);
            texture.LoadImage(System.IO.File.ReadAllBytes(path + "/thumbnail.png"));
            button.image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
    }

    void DeleteButtonEvent()
    {
        System.IO.Directory.Delete(Application.persistentDataPath + "/" + nameText.text, true);
        Destroy(gameObject);
    }
}
