using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 通用脚本：选项面板切换
/// 将脚本挂载在任意对象上，一个Toggle对应一个滚动视图，通过字典储存
/// 并通过遍历为各个Toggle对象添加监听事件，当发现切换后，通过字典查找并切换滚动视图的失活
/// </summary>
public class Options_panel_switching : MonoBehaviour
{
    [System.Serializable]
    public class ToggleScrollViewPair
    {
        public Toggle toggle;
        public ScrollRect scrollView;
    }

    public List<ToggleScrollViewPair> toggleScrollViewPairs = new List<ToggleScrollViewPair>();

    private Dictionary<Toggle, ScrollRect> toggleScrollViewDictionary = new Dictionary<Toggle, ScrollRect>();

    void Start()
    {
        foreach (var pair in toggleScrollViewPairs)
        {
            // 将Toggle和对应的滚动视图添加到字典中
            toggleScrollViewDictionary.Add(pair.toggle, pair.scrollView);

            // 为每个Toggle添加监听事件
            pair.toggle.onValueChanged.AddListener((isOn) => OnToggleValueChanged(pair.toggle, isOn));
        }

        // 初始化，显示第一个Toggle对应的滚动视图
        Toggle firstToggle = toggleScrollViewPairs.Count > 0 ? toggleScrollViewPairs[0].toggle : null;
        OnToggleValueChanged(firstToggle, firstToggle != null ? firstToggle.isOn : false);
    }

    void OnToggleValueChanged(Toggle toggle, bool isOn)
    {
        // 根据Toggle从字典中获取对应的滚动视图
        if (toggleScrollViewDictionary.TryGetValue(toggle, out ScrollRect scrollView))
        {
            // 设置滚动视图的激活状态
            scrollView.gameObject.SetActive(isOn);
        }
    }
}
