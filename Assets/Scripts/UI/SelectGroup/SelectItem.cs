using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectItem : MonoBehaviour
{
    public enum UnSelectedShape
    {
        NoBorder,
        DarkBorder,
    }

    public Image border;

    public UnSelectedShape unSelectedShape = UnSelectedShape.NoBorder;
    public bool selected = false;

    protected virtual void Update()
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

    public void SetSelectedInstantly(bool selected)
    {
        this.selected = selected;
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
        border.color = targetColor;
    }
}
