using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayCode : MonoBehaviour
{
    [Header("拖入自身以及子对象下的对应组件")]
    public CanvasGroup canvasGroup;
    public Animator animator;
    public TMP_InputField normal_InputField;
    public TMP_InputField fullScreen_InputField;

    private string chart_Json;

    // 关闭面板
    public void _Button_Save()
    {
        try
        {
            EditChart_Base.Instance.chart = JsonMapper.ToObject<Chart>(chart_Json);
            EditChart_Base.Instance.Recalculate_Track_Position();
            EditChart_Base.Instance.RedrawNotes();
        }
        catch
        {
            PanelMannger.Instance.Create_PopUP("出现错误", "转换谱面数据出错\n无法使用修改后的谱面数据",
                                            new Set_PopUP_Button("确认", () => { }));
        }

        canvasGroup.interactable = false;
        StartCoroutine(WaitingForAnimation_ToHide());
    }   // 保存修改
    public void _Button_Close()
    {
        canvasGroup.interactable = false;
        StartCoroutine(WaitingForAnimation_ToHide());
    }   // 直接关闭
    IEnumerator WaitingForAnimation_ToHide()
    {
        animator.Play("PopUP_Hide");
        yield return new WaitForSeconds(0.16667f);      // 10 - 60帧的动画
        Destroy(this.gameObject);
    }

    // 开启面板显示代码
    private void Start()
    {
        try
        {
            chart_Json = JsonMapper.ToJson(EditChart_Base.Instance.chart);
            normal_InputField.text = chart_Json;
        }
        catch
        {
            normal_InputField.text = "Error";
            PanelMannger.Instance.Create_PopUP("出现错误", "转换谱面数据出错",
                                            new Set_PopUP_Button("确认", () => { }));
        }
    }

    // 输入框传入
    public void OnEndEdit(string input)
    {
        if (input == "") { return; }
        chart_Json = input;
    }

    // 全屏
    [Header("全屏失活父对象")]
    public GameObject normal_Father;
    public GameObject fullScreen_Father;
    public void _Button_FullScreen()
    {
        normal_Father.SetActive(false);
        fullScreen_Father.SetActive(true);
        fullScreen_InputField.text = chart_Json;
    }
    public void _Button_FullScreen_Return()
    {
        normal_Father.SetActive(true);
        fullScreen_Father.SetActive(false);
        normal_InputField.text = chart_Json;
    }

    // 说明按钮（跳转至github chart类的网页）
    public void _Button_Illustrate()
    {
        PanelMannger.Instance.Create_PopUP("等待返回",
            "您将会跳转至github上的Chart.cs文件\n当您返回到模拟器后，请点击\"确认\"按钮",
            new Set_PopUP_Button("确认", () => { }));

        Application.OpenURL("https://github.com/LMahiro/D4DJ-Simulators/blob/master/Chart/Chart.cs");
    }
}
