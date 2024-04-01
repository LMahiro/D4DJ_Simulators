using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomVoicePlayback : MonoBehaviour
{
    // 附加管理嘴巴开合的组件
    public Live2D.Cubism.Framework.MouthMovement.CubismMouthController mouthController;
    // 各语音时长，因为FMOD很难获取频谱
    public float[] eachVoiceDuration = new float[] { 7.7f, 10.627f, 6.648f, 6.065f, 5.937f, 9.382f };
    private float voiceDuration;

    // 文本框
    public TextMeshProUGUI textBox;
    private TypewriterEffect typewriterEffect;
    public string[] phoneticTexts = new string[] { "嗯...茶叶所剩无几了。我得让索菲亚帮我去买一些。",
        "这个红茶是谁家的呢？...超市？这么便宜的东西，艾尔西不喝！",
        "你最好不要惹艾尔西生气呦。不知道黑衣人会对你做什么呢♪",
        "前几天，维罗妮卡把头撞到了电线杆上。真的是活该呢♪",
        "这段视频看起来很不错呢。它给了我一些编舞的想法。",
        "诶嘿嘿、维罗妮卡在这大冷天冷的直打哆嗦！真是个傻瓜...啊、啊啾！"};

    // Live2D动画
    public Animator animator;

    private void Start()
    {
        typewriterEffect = textBox.GetComponent<TypewriterEffect>();
        PlayRandomVoice();
    }

    float timer;
    bool timing;
    private void Update()
    {
        if (timing)
        {
            timer += Time.deltaTime;
            if (voiceDuration < timer)
            {
                timing = false;
                timer = 0;
                mouthController.MouthOpening = 0f;
            }
        }
    }
    public void PlayRandomVoice()
    {
        if (timing) return;

        // 前含后不含
        int index = Random.Range(0, phoneticTexts.Length);

        SoundManager.Instance.Play_Vo("event:/Voice/Homepage voice/Elsie " + (index + 1));
        voiceDuration = eachVoiceDuration[index];
        textBox.text = phoneticTexts[index];
        typewriterEffect.StartTyping();

        // 启动计时器
        mouthController.MouthOpening = 1f;
        timing = true;
        animator.Play($"{index + 1}");
    }
}
