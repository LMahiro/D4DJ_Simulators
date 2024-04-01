using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private static HealthBar instance;
    public static HealthBar Instance => instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);

        // 获得闪烁动画
        fill_Flashing_Animator = fill_Image.GetComponent<Animator>();
        empty_Flashing_Animator = empty_Image.GetComponent<Animator>();
        // 获得生命值条原始高度
        fixed_HealthBarFill_Height = fill_Image.rectTransform.sizeDelta.y;
    }

    // 当前生命值
    public float health = 1000;
    public void AddHealth(float addValue)
    {
        health += addValue;

        Update_HealthBar();
        Update_HealthNumbers();
    }
    public void RemoveHealth(float removeValue)
    {
        health -= removeValue;

        Update_HealthBar();
        Update_HealthNumbers();
    }


    [Header("生命值填充图片")]
    public Image fill_Image;
    [Header("生命值填充图片各阶段颜色")]
    public Sprite[] fill_Image_Color = new Sprite[3];     // 0为绿色，1为黄色，2为红色
    private Animator fill_Flashing_Animator;
    // 当前生命值宽度
    private float current_HealthBarFill_Width;
    // 当前生命值高度
    private float fixed_HealthBarFill_Height;

    [Header("生命值填充底部图片")]
    public Image empty_Image;
    [Header("生命值填充底部图片各阶段颜色")]
    public Sprite[] empty_Image_Color = new Sprite[2];    // 0为灰色，1为红色
    private Animator empty_Flashing_Animator;

    private void Update_HealthBar()
    {
        // 计算生命值条宽度
        if (0 < health && health <= 1000) current_HealthBarFill_Width = (health / 1000) * 299.2144f;
        else if (1000 < health) current_HealthBarFill_Width = 299.2144f;
        else current_HealthBarFill_Width = 0;

        // 设置生命值条填充图片的宽度
        image_sizeDelta.Set(current_HealthBarFill_Width, fixed_HealthBarFill_Height);
        fill_Image.rectTransform.sizeDelta = image_sizeDelta;

        // 生命值条填充图片的颜色
        if (600 < health) fill_Image.sprite = fill_Image_Color[0];
        else if (300 < health && health <= 600) fill_Image.sprite = fill_Image_Color[1];
        else fill_Image.sprite = fill_Image_Color[2];

        // 生命值条填充图片的闪烁
        if (health <= 300) fill_Flashing_Animator.Play("Fill_Flashing");
        else fill_Flashing_Animator.Play("Fill_ResetTransparency");

        // Empty图片修改、播放动画
        if (0 < health)
        {
            empty_Image.sprite = empty_Image_Color[0];
            empty_Flashing_Animator.Play("Empty_ResetTransparency");
        }
        else
        {
            empty_Image.sprite = empty_Image_Color[1];
            empty_Flashing_Animator.Play("Empty_Flashing");
        }
    }
    private Vector2 image_sizeDelta;


    [Header("生命值数字：物体")]
    public Image[] healthNumber_Obj = new Image[4];             // 0为右数第一个数字，1为右数第二个数字，以此类推
    [Header("生命值数字：Sprite")]
    public Sprite[] healthNumber_Sprite = new Sprite[12];       // 0-9同原数字，10为红色0, 11为透明图片

    public void Update_HealthNumbers()
    {
        health_String = ((int)health).ToString();
        healthCharacterLength = health_String.Length;

        if (health <= 0)            // x x x 0
        {
            for (int i = 1; i < 4; i++)
                healthNumber_Obj[i].sprite = healthNumber_Sprite[11];   // 透明图片
            healthNumber_Obj[0].sprite = healthNumber_Sprite[10];       // 红色0图片
        }
        else if (health < 10)       // x x x 1
        {
            for (int i = 1; i < 4; i++)                                     // 透明图片
                healthNumber_Obj[i].sprite = healthNumber_Sprite[11];
            healthNumber_Obj[0].sprite = healthNumber_Sprite[GetDigit(0)];  // 获取对应位的数字
        }
        else if (health < 100)      // x x 2 1
        {
            for (int i = 2; i < 4; i++)
                healthNumber_Obj[i].sprite = healthNumber_Sprite[11];
            for (int i = 0; i < 2; i++)
                healthNumber_Obj[i].sprite = healthNumber_Sprite[GetDigit(i)];
        }
        else if (health < 1000)     // x 3 2 1
        {
            healthNumber_Obj[3].sprite = healthNumber_Sprite[11];
            for (int i = 0; i < 3; i++)
                healthNumber_Obj[i].sprite = healthNumber_Sprite[GetDigit(i)];
        }
        else if (health < 10000)    // 4 3 2 1
        {
            for (int i = 0; i < 4; i++)
                healthNumber_Obj[i].sprite = healthNumber_Sprite[GetDigit(i)];
        }
        else                        // 9 9 9 9
        {
            for (int i = 0; i < 4; i++)
                healthNumber_Obj[i].sprite = healthNumber_Sprite[9];    // 显示数字9
        }
    }
    private string health_String;
    private int healthCharacterLength;

    // 获取数字的指定位上的数字
    private int GetDigit(int position)
    {
        char digitChar = health_String[healthCharacterLength - 1 - position];
        return int.Parse(digitChar.ToString());
    }
}
