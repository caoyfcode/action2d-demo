using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusIconList : MonoBehaviour
{
    public GameObject childPrefab;
    public int Total
    {
        set
        {
            UpdateChildCount(value);
        }
        get => transform.childCount;
    }
    public float value;
    private float movingValue;

    // Start is called before the first frame update
    void Start()
    {
        movingValue = value;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(value - movingValue) < Mathf.Epsilon)
        {
            movingValue = value;
        }
        else
        {
            movingValue += (value - movingValue) * Mathf.Clamp01(Time.deltaTime / 0.2f);
        }
        // FIXME: 子结点数量变化并不是即使的，而是要等到帧末, 因而可能导致问题
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).GetComponent<StatusIcon>();
            child.ratio = Mathf.Clamp01(value - i);
            child.movingRatio = Mathf.Clamp01(movingValue - i);
        }
    }

    void UpdateChildCount(int num)
    {
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
            for (int i = childCount - 1; i >= num; i++)
            {
                var child = transform.GetChild(i);
                Destroy(child.gameObject);
            }
        }
    }
}
