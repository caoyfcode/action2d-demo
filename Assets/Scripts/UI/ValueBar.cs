using System;
using UnityEngine;

public class ValueBar : MonoBehaviour
{
    public float maxValue;
    public float value; // real value
    public float OuterValue {get; private set;}
    public float InnerValue {get; private set;}
    public float OuterRatio => Mathf.Clamp01(maxValue > Mathf.Epsilon ? OuterValue / maxValue : 1);
    public float InnerRatio => Mathf.Clamp01(maxValue > Mathf.Epsilon ? InnerValue / maxValue : 1);

    public enum State { Decrease, Increase, Constant }
    public State CurrentState {get; private set;}

    void Start()
    {
        OuterValue = value;
        InnerValue = value;
    }


    void Update()
    {
        if (OuterValue < value && Mathf.Abs(OuterValue - value) > Mathf.Epsilon)
        {
            CurrentState = State.Increase;
        }
        else if (InnerValue > value && Mathf.Abs(InnerValue - value) > Mathf.Epsilon)
        {
            CurrentState = State.Decrease;
        }
        else
        {
            CurrentState = State.Constant;
        }

        switch (CurrentState)
        {
            case State.Decrease:
                OuterValue = value;
                InnerValue += (value - InnerValue) * Mathf.Clamp01(Time.deltaTime / 0.2f);
                break;
            case State.Increase:
                OuterValue += (value - OuterValue) * Mathf.Clamp01(Time.deltaTime / 0.1f);
                InnerValue = OuterValue;
                break;
            case State.Constant:
                OuterValue = value;
                InnerValue = OuterValue;
                break;
        }
    }
}