using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goto_Designated_Interface : MonoBehaviour
{
    public GameObject goto_Panel_OBJ;
    public void _Goto_Designated_Interface()
    {
        PanelMannger.Instance.SwitchTo_NewPane(goto_Panel_OBJ);
        SoundManager.Instance.Play_SE();
    }
}
