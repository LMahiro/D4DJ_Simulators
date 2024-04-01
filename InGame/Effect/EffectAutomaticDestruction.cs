using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAutomaticDestruction : MonoBehaviour
{
    // 自动销毁时间
    public float automaticDestructionTime = 1;

    private void Awake()
    {
        Destroy(this.gameObject, automaticDestructionTime);
    }
}
