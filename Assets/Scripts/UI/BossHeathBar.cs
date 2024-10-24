using UnityEngine;
using UnityEngine.UI;


public class BossHeathBar : MonoBehaviour
{
    public ValueBar valueBar;
    public RectMask2D innerMask;
    public RectMask2D foregroundMask;

    public RectTransform contentRectTransform;


    void Update()
    {
        innerMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, valueBar.InnerRatio * contentRectTransform.rect.width);
        foregroundMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, valueBar.OuterRatio * contentRectTransform.rect.width);
    }
}