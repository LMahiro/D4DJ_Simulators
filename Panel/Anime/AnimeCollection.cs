using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimeCollection : MonoBehaviour
{
    private void Awake()
    {
        for (int i = 0; i < animeCollection.Length; i++)
        {
            int index = i;
            animeCollection[i].onClick.AddListener(() => { WatchEpisode(index); });
        }
    }

    [Header("拖入播放器面板")]
    public GameObject animePanel;

    [Header("各集数Youtube链接")]
    public string[] animeURL;

    [Header("主标题")]
    public string title;

    [Header("各集数副标题")]
    public string[] subTitle;

    [Header("各集数按钮")]
    public Button[] animeCollection;

    public void WatchEpisode(int index)
    {
#if UNITY_ANDROID
        // 安卓端的测试服务器已被关闭，无法解析视频
        PanelMannger.Instance.Create_PopUP("不受支持", "我们无法为安卓设备继续获取视频并播放\n安卓端的测试服务器已被关闭，无法解析视频" +
            "\n\n<size=28>为避免侵权，本软件播放的视频均来源于官方D4DJ EN Youtube频道公开的视频\n模拟器仅获取视频地址并播放</size>",
            new Set_PopUP_Button("取消播放", () => { }),
            new Set_PopUP_Button("<size=28>跳转至浏览器播放</size>", () => { PanelMannger.Instance.SwitchTo_WebPage($"{animeURL[index]}"); }),
            new Set_PopUP_Button("<size=28>依然用模拟器播放</size>", () =>
            {
                animePanel.GetComponentInChildren<YoutubePlayer.YoutubePlayer>().youtubeUrl = animeURL[index];
                animePanel.GetComponent<Panel_Self>().panel_Title = title;
                animePanel.GetComponent<Panel_Self>().panel_SubTitle = subTitle[index];
                PanelMannger.Instance.SwitchTo_NewPane(animePanel);
            }));
#else
        animePanel.GetComponentInChildren<YoutubePlayer.YoutubePlayer>().youtubeUrl = animeURL[index];
        animePanel.GetComponent<Panel_Self>().panel_Title = title;
        animePanel.GetComponent<Panel_Self>().panel_SubTitle = subTitle[index];
        PanelMannger.Instance.SwitchTo_NewPane(animePanel);
#endif
    }
}
