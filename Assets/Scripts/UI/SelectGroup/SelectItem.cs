using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectItem : MonoBehaviour
{
    public Image border;
    public bool selected = false;
    public UnityEvent confirm;

    void Update()
    {
        float targetAlpha = selected ? 1f : 0f;
        if (targetAlpha != border.color.a)
        {
            Color targetColor = border.color;
            targetColor.a = Mathf.Lerp(border.color.a, targetAlpha, Mathf.Clamp01(Time.unscaledDeltaTime * 40f)); // 1/40s 内变换完
            border.color = targetColor;
        }
        if (selected && Input.GetKeyDown(KeyCode.K))
        {
            confirm?.Invoke();
        }
    }

    public void ImmediateSelcted(bool value)
    {
        selected = value;
        Color targetColor = border.color;
        targetColor.a = selected ? 1f : 0f;
        border.color = targetColor;
    }
}
