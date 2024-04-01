using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel_BGRandomSwitch : MonoBehaviour
{
    public List<Texture2D> textures = new List<Texture2D>();
    public RawImage bg_RawImage;
    private void Awake()
    {
        // 包含最小值，不包含最大值
        int randomIndex = Random.Range(0, textures.Count);
        bg_RawImage.texture = textures[randomIndex];
    }
}
