using UnityEngine;
using UnityEngine.UI;

public class CharacterClickAnimation : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        // 添加点击事件监听
        Button button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(PlayClickAnimation);
    }

    void PlayClickAnimation()
    {
        animator.Rebind();
        animator.Play("CharacterClickAnimation");
    }
}
