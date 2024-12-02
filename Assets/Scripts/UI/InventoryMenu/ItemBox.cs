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
        DarkBorder,
    }

    public Image border;
    public Image itemImage; // TODO 暂不显示

    public UnSelectedShape unSelectedShape = UnSelectedShape.NoBorder;
    public bool selected = false;

    void Update()
    {
        Color targetColor = Color.white;
        if (!selected)
        {
            switch (unSelectedShape)
            {
                case UnSelectedShape.NoBorder:
                    targetColor.a = 0f;
                    break;
                case UnSelectedShape.DarkBorder:
                    targetColor.r = 0.5f;
                    targetColor.g = 0.5f;
                    targetColor.b = 0.5f;
                    break;
            }
        }
        if (border.color != targetColor)
        {
            targetColor.r = Mathf.Lerp(border.color.r, targetColor.r, Mathf.Clamp01(Time.unscaledDeltaTime * 40f)); // 1/40s 内变换完
            targetColor.g = Mathf.Lerp(border.color.g, targetColor.g, Mathf.Clamp01(Time.unscaledDeltaTime * 40f)); // 1/40s 内变换完
            targetColor.b = Mathf.Lerp(border.color.b, targetColor.b, Mathf.Clamp01(Time.unscaledDeltaTime * 40f)); // 1/40s 内变换完
            targetColor.a = Mathf.Lerp(border.color.a, targetColor.a, Mathf.Clamp01(Time.unscaledDeltaTime * 40f)); // 1/40s 内变换完
        }
        border.color = targetColor;
    }
}
