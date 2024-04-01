using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround_Normal : MonoBehaviour
{
    public float image_Move_Speed;
    public RectTransform Canvas_RectTransform; Vector2 Canvas_Size;
    public RectTransform image_RectTransform;

    void Start()
    {
        Set_Image_RectTransform();
    }

    void FixedUpdate()
    {
        // 若修改了分辨率，将重新设置图片大小
        if (Canvas_RectTransform.sizeDelta != Canvas_Size)
            Set_Image_RectTransform();

        // 移动图片位置
        image_RectTransform.anchoredPosition += new Vector2(image_Move_Speed, -image_Move_Speed);

        // 无缝循环（256图片大小 / 0.6每单位像素乘数）
        if (image_RectTransform.anchoredPosition.x > 426.67f || image_RectTransform.anchoredPosition.y < -426.67f)
            image_RectTransform.anchoredPosition = Vector2.zero;
    }

    void Set_Image_RectTransform()
    {
        // 获取Canvas的大小
        Canvas_Size = Canvas_RectTransform.sizeDelta;

        // 根据Canvas的大小*2来铺满屏幕
        image_RectTransform.sizeDelta = new Vector2(Canvas_Size.x * 2, Canvas_Size.y * 2);

        // 移至相对于父对象的左上角
        image_RectTransform.anchoredPosition = Vector2.zero;
    }
}

