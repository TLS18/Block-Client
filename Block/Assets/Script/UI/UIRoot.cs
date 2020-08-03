using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoot : MonoBehaviour
{
    void Start()
    {
        PanelMgr.instance.OpenPanel<TitlePanel>("");
    }

}
