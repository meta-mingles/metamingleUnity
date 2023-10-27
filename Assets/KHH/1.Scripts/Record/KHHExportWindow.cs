using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KHHExportWindow : MonoBehaviour
{
    public enum ExportState
    {
        None,
        Processing,
        Generate,
        Exporting,
        Complete,
    }
    public ExportState exportState = ExportState.None;


    public TextMeshProUGUI progressText;

    //complete
    public CanvasGroup completeCG;
    public TextMeshProUGUI completeText;
    public Button confirmButton;
    public Button cancelButton;
    public TextMeshProUGUI cancelButtonText;

    // Start is called before the first frame update
    void Start()
    {
        if (confirmButton != null)
            confirmButton.onClick.AddListener(() =>
            {
                if (exportState == ExportState.Generate)
                    KHHEditManager.Instance.ExportVideo();
                else if (exportState == ExportState.Complete)
                {
                    exportState = ExportState.None;
                    gameObject.SetActive(false);
                }
            });

        if (cancelButton != null)
            cancelButton.onClick.AddListener(() =>
            {
                exportState = ExportState.None;
                gameObject.SetActive(false);
            });
    }

    //// Update is called once per frame
    //void Update()
    //{
    //}

    public void StateChange(ExportState state)
    {
        exportState = state;

        switch (exportState)
        {
            case ExportState.Processing:
                progressText.gameObject.SetActive(true);
                progressText.alpha = 1;
                progressText.text = "���� ������...";
                completeCG.gameObject.SetActive(false);
                completeCG.alpha = 0;
                break;
            case ExportState.Generate:
                progressText.text = "���� �����Ϸ�";
                progressText.DOFade(0, 0.4f).SetDelay(0.2f).OnComplete(() =>
                {
                    progressText.gameObject.SetActive(false);
                    completeCG.gameObject.SetActive(true);
                    completeCG.DOFade(1, 0.4f);

                    completeText.gameObject.SetActive(true);
                    confirmButton.gameObject.SetActive(true);
                    cancelButtonText.text = "���";
                });
                break;
            case ExportState.Exporting:
                progressText.gameObject.SetActive(true);
                progressText.alpha = 1;
                progressText.text = "���� �������� ��...";
                completeCG.gameObject.SetActive(false);
                completeCG.alpha = 0;
                break;
            case ExportState.Complete:
                progressText.text = "���� �������� �Ϸ�";
                //progressText.DOFade(0, 0.4f).SetDelay(0.2f).OnComplete(() =>
                //{
                //    progressText.gameObject.SetActive(false);
                //    completeCG.gameObject.SetActive(true);
                //    completeCG.DOFade(1, 0.4f);

                //});
                completeCG.gameObject.SetActive(true);
                completeCG.DOFade(1, 0.4f);
                completeText.gameObject.SetActive(false);
                confirmButton.gameObject.SetActive(false);
                cancelButtonText.text = "�ݱ�";
                break;
        }
    }
}
