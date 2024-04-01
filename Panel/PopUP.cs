using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;


public class PopUP : MonoBehaviour
{
    [Header("拖入自身以及子对象下的对应组件")]
    public CanvasGroup canvasGroup;
    public Animator animator;
    public TextMeshProUGUI title;
    public TextMeshProUGUI content;

    public void Panel_Hide()
    {
        canvasGroup.interactable = false;
        SoundManager.Instance.Play_SE("event:/SE/Normal/True");
        StartCoroutine(WaitingForAnimation_ToHide());
    }
    IEnumerator WaitingForAnimation_ToHide()
    {
        animator.Play("PopUP_Hide");
        yield return new WaitForSeconds(0.16667f);      // 10 - 60帧的动画
        Destroy(this.gameObject);
    }



    [Header("用于复制用的按钮")]
    public GameObject button_For_Copy;

    [Header("复制用的按钮的父对象")]
    public GameObject button_Parent;
}

// 设置按钮专门提供的类
public class Set_PopUP_Button
{
    public Set_PopUP_Button(string buttonContent, UnityAction buttonClickAction_Instance)
    {
        this.buttonContent = buttonContent;
        this.buttonClickAction_Instance = buttonClickAction_Instance;
    }

    // 按钮显示文字
    public string buttonContent;
    // 按钮委托实例
    public UnityAction buttonClickAction_Instance;
}
