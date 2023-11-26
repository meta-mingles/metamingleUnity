using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KHHVideoItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        deleteButton.gameObject.SetActive(true);
        deleteButton.interactable = true;
        deleteButton.image.DOKill();
        deleteButton.image.DOFade(1, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        deleteButton.interactable = false;
        deleteButton.image.DOKill();
        deleteButton.image.DOFade(0, 0.2f).OnComplete(() => deleteButton.gameObject.SetActive(false));
    }
}
