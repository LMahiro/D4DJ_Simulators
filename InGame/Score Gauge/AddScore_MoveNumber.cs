using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddScore_MoveNumber : MonoBehaviour
{
    [Header("加分移动数字：物体")]
    public Image[] addScore_MoveNumber_Obj = new Image[8];  // 0为最左侧数字，1为左数第二个数字，依次上涨
    [Header("拖入自身Animator控件")]
    public Animator animator;
}
