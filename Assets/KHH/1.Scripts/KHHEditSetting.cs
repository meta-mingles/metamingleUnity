using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class KHHEditSetting : MonoBehaviour
{
    public Button closePanel;
    public Button confirmButton;
    public Button costomButton;
    public DropDownList microphoneDropDownList;
    List<DropDownListItem> dropDownListItems;

    string microphoneName;

    // Start is called before the first frame update
    void Awake()
    {
        closePanel.onClick.AddListener(CloseEvent);
        confirmButton.onClick.AddListener(CloseEvent);
        costomButton.onClick.AddListener(CostomButtonEvent);
        microphoneDropDownList.OnSelectionChanged.AddListener(DropDownEvent);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        StartCoroutine(SetDropDownList());
    }

    IEnumerator SetDropDownList()
    {
        string[] microphoneNames = Microphone.devices;
        if (microphoneNames.Length == 0)
        {
            microphoneName = null;
            microphoneDropDownList.SetActive(false);
            yield break;
        }

        yield return null;

        microphoneDropDownList.SetActive(true);
        microphoneName = MicrophoneRecorder.Instance.MicrophoneName;

        if (dropDownListItems != null) dropDownListItems.Clear();
        else dropDownListItems = new List<DropDownListItem>();

        for (int i = 0; i < microphoneNames.Length; i++)
            dropDownListItems.Add(new DropDownListItem(microphoneNames[i], i.ToString()));

        //microphoneDropDownList.Items = dropDownListItems;
        microphoneDropDownList.RefreshItems(microphoneNames);
        microphoneDropDownList.SelectItemIndex(dropDownListItems.FindIndex(x => x.Caption == microphoneName));
    }

    void CloseEvent()
    {
        MicrophoneRecorder.Instance.MicrophoneName = microphoneName;
        PlayerPrefs.SetString("MicrophoneName", microphoneName);

        gameObject.SetActive(false);
    }

    void CostomButtonEvent()
    {
        //커스터마이즈 씬 열기
        GlobalValue.PrevSceneName = SceneManager.GetActiveScene().name;
        GlobalValue.CurSceneName = "Customization";
        SceneManager.LoadScene("Customization");
    }

    void DropDownEvent(int idx)
    {
        microphoneName = dropDownListItems[idx].Caption;
    }
}
