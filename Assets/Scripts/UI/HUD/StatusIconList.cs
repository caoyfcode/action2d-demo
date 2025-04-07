using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusIconList : MonoBehaviour
{
    public ValueBar valueBar;
    public GameObject childPrefab;
    public int Total
    {
        set
        {
            UpdateChildCount(value);
            valueBar.maxValue = value;
        }
        get => (int)valueBar.maxValue;
    }
    public float Value
    {
        set
        {
            valueBar.value = value;
        }
        get => valueBar.value;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < transform.childCount && i < Total; i++)
        {
            var child = transform.GetChild(i).GetComponent<StatusIcon>();
            child.outerRatio = Mathf.Clamp01(valueBar.OuterValue - i);
            child.innerRatio = Mathf.Clamp01(valueBar.InnerValue - i);
        }
    }

    void UpdateChildCount(int num)
    {
        // TODO 添加新增的动画
        int childCount = transform.childCount;
        if (childCount < num)
        {
            for (int i = 0; i < num - childCount; i++)
            {
                Instantiate(childPrefab, transform);
            }
        }
        else if (childCount > num)
        {
            for (int i = childCount - 1; i >= num; i--)
            {
                var child = transform.GetChild(i);
                DestroyImmediate(child.gameObject);
            }
        }
    }
}
