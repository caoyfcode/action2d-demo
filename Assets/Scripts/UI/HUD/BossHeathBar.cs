using UnityEngine;
using UnityEngine.UI;


public class BossHeathBar : MonoBehaviour
{
    public ValueBar valueBar;
    public RectTransform innerRectTransform;
    public RectTransform foregroundRectTransform;

    public RectTransform contentRectTransform;
    public bool Shown
    {
        set
        {
            gameObject.SetActive(value);
        }
        get
        {
            return gameObject.activeSelf;
        }
    }

    void Start()
    {
        Shown = false;
    }

    void Update()
    {
        innerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, valueBar.InnerRatio * contentRectTransform.rect.width);
        foregroundRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, valueBar.OuterRatio * contentRectTransform.rect.width);
    }
}