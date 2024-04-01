using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarLine : MonoBehaviour
{
    void Update()
    {
        if (this.transform.position.y < NoteLaneManager.Instance.notMovingLine.position.y)
            Destroy(this.gameObject);
    }
}
