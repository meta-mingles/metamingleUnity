using System;
using TMPro;
using UnityEngine;

public class J_InteractiveMovieItem : MonoBehaviour
{
    public Action<int> onClickInteractive;

    public TMP_Text choice1;
    public TMP_Text choice2;

    //인터렉티브버튼으로 여는 함수
    public void OnClickInteractiveButton(int btnIdx)
    {
        if (onClickInteractive != null)
        {
            onClickInteractive(btnIdx);
        }
    }
    //인터렉티브 데이터 셋팅
    public void SetInteractiveInfo(Action<int> action, string m, string n)
    {
        gameObject.SetActive(true);
        onClickInteractive = action;

        choice1.text = m;
        choice2.text = n;
    }
}
