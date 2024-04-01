using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForReturnPanel : MonoBehaviour
{
    void Start()
    {
        PanelMannger.Instance.Button_Hide_All();
    }

    public void _Back()
    {
        PanelMannger.Instance.GoBackTo_PreviousLeve();
        PanelMannger.Instance.Button_Show_All();
    }
}
