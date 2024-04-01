using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Panel_Self : MonoBehaviour
{
    private float fadeDuration = 0.2f; // 淡入淡出的时间
    public string panel_Title;
    public string panel_SubTitle;
    public string panel_BackGround = "Normal";

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        // 默认设置全透明
        canvasGroup.alpha = 0f;

        switch (panel_BackGround)
        {
            case "Normal":
                BackGroundMannger.Instance.SwitchTo_Normal();
                break;
            case "Edit":
                BackGroundMannger.Instance.SwitchTo_Edit();
                break;
            case "Black":
                BackGroundMannger.Instance.SwitchTo_Black();
                break;
            case "Anime":
                BackGroundMannger.Instance.SwitchTo_Anime();
                break;
            default:    // 没有输入则不切换
                break;
        }
    }

    /// <summary>
    /// 全局可访问的 面板淡入效果
    /// </summary>
    public void Public_FadeIn()
    {
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// 全局可访问的 面板淡出效果
    /// </summary>
    public void Public_FadeOut()
    {
        StartCoroutine(FadeOut());
    }

    /// <summary>
    /// 私有：淡入面板
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeIn()
    {
        canvasGroup.interactable = false;

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        canvasGroup.interactable = true;
    }

    /// <summary>
    /// 私有：淡出面板。请注意：淡出逻辑执行结束后会删除自身
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeOut()
    {
        canvasGroup.interactable = false;

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;

        Destroy(this.gameObject);
    }
}
