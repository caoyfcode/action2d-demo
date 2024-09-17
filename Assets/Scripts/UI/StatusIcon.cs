using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusIcon : MonoBehaviour
{
    public float zeroWidth;
    public float oneWidth;
    public float ratio;
    public float movingRatio;
    public RectMask2D innerMask;
    public RectMask2D foregroundMask;

    // Start is called before the first frame update
    void Start()
    {
        movingRatio = ratio;
    }

    // Update is called once per frame
    void Update()
    {
        float innerWidth = movingRatio * (oneWidth - zeroWidth) + zeroWidth;
        float foregroundWidth = ratio * (oneWidth - zeroWidth) + zeroWidth;
        innerMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, innerWidth);
        foregroundMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, foregroundWidth);
    }
}
