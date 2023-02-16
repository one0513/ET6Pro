using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CMeshProLabel))]
public class CMeshProColor:MonoBehaviour
{
    //public Color BLColor = Color.white;
    //public Color TLColor = Color.white;
    //public Color BCColor = Color.white;
    //public Color TCColor = Color.white;
    //public Color BRColor = Color.white;
    //public Color TRColor = Color.white;

    public List<Color> bColors = new List<Color>() { new Color(1,1,1,1), new Color(1, 1, 1, 1) };
    public List<Color> tColors = new List<Color>() { new Color(1, 1, 1, 1), new Color(1, 1, 1, 1)};

    public bool isEnable = true;
    public bool isContinuity = true;  //连续性

    private CMeshProLabel tmp;

    public CMeshProLabel Tmp {
        get
        {
            if (tmp == null)
            {
                tmp = GetComponent<CMeshProLabel>();
            }
            return tmp;
        }
    }

    public void SetEnabled(bool isEnable)
    {
        if(this.isEnable!=isEnable)
        {
            this.isEnable = isEnable;
            Refresh();
        }
    }

    public void SetColor(List<object> bColors, List<object> tColors)
    {
        this.bColors = bColors.ConvertAll<Color>(Input=>(Color)Input);
        this.tColors = tColors.ConvertAll<Color>(Input => (Color)Input);
        SetEnabled(true);
        Refresh();
    }

    public void Refresh()
    {
        Tmp.enableVertexGradient = this.isEnable;
        //Tmp.UpdateAll();
    }
}
