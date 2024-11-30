using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 96x96 大小的单个物品
public class ItemBox : MonoBehaviour
{
    public enum UnSelectedShape
    {
        NoBorder,
        TransparentBorder,
    }

    public Image border;

    public UnSelectedShape unSelectedShape = UnSelectedShape.NoBorder;
    public bool seletced = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
