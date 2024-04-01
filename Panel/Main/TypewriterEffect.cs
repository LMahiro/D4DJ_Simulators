using UnityEngine;
using TMPro;

// TMP打字机效果
public class TypewriterEffect : MonoBehaviour
{
    public float typingSpeed = 10f; // 打字速度，每秒显示的字符数
    public bool startTypingOnStart = true; // 是否在启动时开始打字

    private TMP_Text textMeshPro;
    private int visibleCharacterCount = 0;
    private float elapsedTime = 0.0f;

    void Start()
    {
        textMeshPro = GetComponent<TMP_Text>();

        if (startTypingOnStart)
            StartTyping();

    }

    void Update()
    {
        // 如果未显示完所有字符
        if (visibleCharacterCount < textMeshPro.text.Length)
        {
            // 计时器，每过1/typingSpeed秒增加一个可见字符
            elapsedTime += Time.deltaTime;

            // 检查是否达到显示字符的时间间隔
            if (elapsedTime >= 1.0f / typingSpeed)
            {
                // 重置计时器
                elapsedTime = 0f;

                // 增加可见字符数
                visibleCharacterCount++;

                // 将TMP_Text组件的maxVisibleCharacters属性设置为可见字符数，实现逐字显示
                textMeshPro.maxVisibleCharacters = visibleCharacterCount;
            }
        }
    }

    // 重新开始打字效果
    public void StartTyping()
    {
        // 重置可见字符数
        visibleCharacterCount = 0;

        // 将TMP_Text组件的maxVisibleCharacters属性设置为零，即不显示任何字符
        textMeshPro.maxVisibleCharacters = visibleCharacterCount;
    }
}
