using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 面板切换的管理类。
/// uiStack存储面板(预设体)，每个面板预设体都挂载了ScreenTransition脚本
/// 请注意：Public_FadeOut()淡出面板包含了删除自身的逻辑
/// </summary>
public class PanelMannger : MonoBehaviour
{
    private static PanelMannger instance;
    public static PanelMannger Instance => instance;

    [Header("主界面预设体")]
    public GameObject main_Panel_OBJ;
    [Header("菜单面板预设体")]
    public GameObject menu_OBJ;
    [Header("左上角标题")]
    public TextMeshProUGUI title;
    public TextMeshProUGUI subTitle;

    // 自身的Canvas Group
    private CanvasGroup canvasGroup_Self;
    // UI管理Stack
    public Stack<GameObject> uiStack = new Stack<GameObject>();


    private void Start()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);

        canvasGroup_Self = GetComponent<CanvasGroup>();

        GoBackTo_MainInterface();
    }

    /// <summary>
    /// 切换到新面板
    /// </summary>
    /// <param name="nextPanel">新面板(预设体)</param>
    public void SwitchTo_NewPane(GameObject nextPanel_OBJ)
    {
        Cooling();
        SoundManager.Instance.Stop_Vo();

        Panel_Self nextPanel = Instantiate(nextPanel_OBJ).GetComponent<Panel_Self>();

        if (uiStack.Count > 0)
        {
            GameObject currentPanel = uiStack.Peek();
            // 淡出当前面板
            GameObject.Find(currentPanel.name + "(Clone)").GetComponent<Panel_Self>().Public_FadeOut();
        }

        uiStack.Push(nextPanel_OBJ);
        nextPanel.Public_FadeIn(); // 淡入新面板

        // 更新当前标题
        title.text = nextPanel.panel_Title;
        subTitle.text = nextPanel.panel_SubTitle;
    }

    /// <summary>
    /// 返回上一级
    /// </summary>
    public void GoBackTo_PreviousLeve()
    {
        Cooling();

        if (uiStack.Count > 1)
        {
            GameObject currentPanel = uiStack.Pop();
            // 淡出当前面板
            GameObject.Find(currentPanel.name + "(Clone)").GetComponent<Panel_Self>().Public_FadeOut();

            if (uiStack.Count > 0)
            {
                GameObject previousPanel = uiStack.Peek();

                Panel_Self lastPanel = Instantiate(previousPanel).GetComponent<Panel_Self>();
                // 淡入上一个面板
                lastPanel.Public_FadeIn();

                // 更新当前标题
                title.text = lastPanel.panel_Title;
                subTitle.text = lastPanel.panel_SubTitle;
            }
            else
                GoBackTo_MainInterface();
        }
        else
        {
            Create_PopUP("确认", "确认退出模拟器？",
                new Set_PopUP_Button("取消", () => { }),
                new Set_PopUP_Button("确定", () => { Application.Quit(); }));
        }
    }

    /// <summary>
    /// 返回主界面
    /// </summary>
    public void GoBackTo_MainInterface()
    {
        Cooling();

        if (uiStack.Count > 0)
        {
            GameObject currentPanel = uiStack.Pop();
            // 淡出当前面板
            GameObject.Find(currentPanel.name + "(Clone)").GetComponent<Panel_Self>().Public_FadeOut();
        }

        uiStack.Clear();
        // 返回主界面
        SwitchTo_NewPane(main_Panel_OBJ);
    }

    /// <summary>
    /// 打开菜单面板
    /// </summary>
    /// <param name="settingsPanel_OBJ">菜单面板(预设体)</param>
    public void Open_MenuPanel()
    {
        Cooling();

        // 淡入菜单面板，不参与UI面板的一系列逻辑
        Instantiate(menu_OBJ).GetComponent<Panel_Self>().Public_FadeIn();
    }

    /// <summary>
    /// 重新设置当前面板标题
    /// </summary>
    /// <param name="title">新标题内容</param>
    /// <param name="subTitle">新副标题内容</param>
    public void Reset_Title(string title, string subTitle)
    {
        this.title.text = title;
        this.subTitle.text = subTitle;
        // 如果在这里获取了Panel_Self脚本，那么后续该面板的标题都将会设置为修改的标题
    }


    /// <summary>
    /// 按钮冷却，防止点击太快Find来不及删除
    /// </summary>
    private void Cooling()
    {
        if (coroutine_Cooling == null)
            coroutine_Cooling = StartCoroutine(CooldownTime());
        else
        {   //若已在冷却，则重新计算冷却时间
            StopCoroutine(coroutine_Cooling);
            coroutine_Cooling = StartCoroutine(CooldownTime());
        }
    }
    Coroutine coroutine_Cooling;
    private IEnumerator CooldownTime()
    {
        canvasGroup_Self.interactable = false;
        yield return new WaitForSeconds(0.5f);
        canvasGroup_Self.interactable = true;

        coroutine_Cooling = null;
    }


    /// <summary>
    /// 打开切换至网页的新面板，并打开网页
    /// </summary>
    /// <param name="webLink">网页链接</param>
    public GameObject waittingForReturn_OBJ;
    public void SwitchTo_WebPage(string webLink)
    {
        Application.OpenURL(webLink);
        SwitchTo_NewPane(waittingForReturn_OBJ);
    }





    [Header("拖入按钮对象")]
    public GameObject[] button_All;     // 0 Back  1 Home  2 Menu

    public void Button_Hide_All()
    {
        for (int i = 0; i < button_All.Length; i++)
            button_All[i].SetActive(false);
    }
    public void Button_Back_Only()
    {
        button_All[0].SetActive(true);
        for (int i = 1; i < button_All.Length; i++)
            button_All[i].SetActive(false);
    }
    public void Button_Hide_Home()
    {
        button_All[1].SetActive(false);
    }
    public void Button_Show_All()
    {
        for (int i = 0; i < button_All.Length; i++)
            button_All[i].SetActive(true);
    }



    [Header("拖入标题对象")]
    public GameObject[] title_All;      // 0 Title  1 SubTitle

    public void Title_Hide_All()
    {
        for (int i = 0; i < title_All.Length; i++)
            title_All[i].SetActive(false);
    }
    public void Title_Show_All()
    {
        for (int i = 0; i < title_All.Length; i++)
            title_All[i].SetActive(true);
    }




    [Header("拖入NowLoading面板预设体")]
    public GameObject nowLoading_OBJ;
    private GameObject nowLoading_OBJ_Instance;
    public void Show_NowLoading_Panel()
    {
        if (nowLoading_OBJ_Instance == null)
            nowLoading_OBJ_Instance = GameObject.Instantiate<GameObject>(nowLoading_OBJ);
        else Debug.LogWarning("Panel_Mannger：已存在NowLoading面板！");
    }
    public void Hide_NowLoading_Panel()
    {
        if (nowLoading_OBJ_Instance != null)
        {
            Destroy(nowLoading_OBJ_Instance);
            nowLoading_OBJ_Instance = null;
        }
        else Debug.LogWarning("Panel_Mannger：不存在NowLoading面板，请先显示后再调用隐藏！");
    }



    [Header("拖入弹出菜单(PopUP)预设体")]
    public GameObject popUP_OBJ;
    /// <summary>
    /// 创建一个弹出面板
    /// </summary>
    /// <param name="title">面板的标题</param>
    /// <param name="content">面板的内容</param>
    /// <param name="set_PopUP_Button">面板的按钮相关操作，Set_PopUP_Button类中包含按钮名称、按钮操作。根据Set_PopUP_Button[]的长度确定按钮数量</param>
    public void Create_PopUP(string title, string content, params Set_PopUP_Button[] set_PopUP_Button)
    {
        PopUP popUP_Script = Instantiate<GameObject>(popUP_OBJ).GetComponent<PopUP>();  // 获取预设体原本挂载的脚本

        popUP_Script.title.text = title;                                                // 可以直接设置的值
        popUP_Script.content.text = content;

        GameObject button_For_Copy = popUP_Script.button_For_Copy;                      // 得到脚本上所依附的对象
        GameObject button_Parent = popUP_Script.button_Parent;

        for (int i = 0; i < set_PopUP_Button.Length; i++)                               // 循环设置按钮逻辑
        {
            // 复制出一个按钮
            Button button = Instantiate<GameObject>(button_For_Copy, button_Parent.transform).GetComponent<Button>();

            // 按钮必须有标题和按下行为
            if (set_PopUP_Button[i].buttonContent != null
                && set_PopUP_Button[i].buttonClickAction_Instance != null)
            {
                button.GetComponentInChildren<TextMeshProUGUI>().text = set_PopUP_Button[i].buttonContent;

                button.onClick.AddListener(set_PopUP_Button[i].buttonClickAction_Instance); // 添加自定义方法
                button.onClick.AddListener(popUP_Script.Panel_Hide);                        // 添加自带的关闭方法
            }
            else Debug.LogWarning($"SetTheNumberOf_Buttons：第{i}个按钮缺失标题或委托！");
        }

        Destroy(button_For_Copy);   // 删除原始自带的复制用的按钮
    }
}
