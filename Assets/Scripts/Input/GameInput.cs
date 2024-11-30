using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : SingletonBase<GameInput>
{
    // Start is called before the first frame update
    public enum InputType
    {
        Keyoard,
        XboxController,
        PsController, // does not use
        SwitchController, // does not use
    }

    public static InputType CurrentInputType => instance.currentInputType;

    [SerializeField]
    InputType currentInputType = InputType.Keyoard;
    public InputMap inputMap;

    public float horizontal;
    public float vertical;

    // Start is called before the first frame update
    void Start()
    {
        inputMap.Init(currentInputType);
    }

    // Update is called once per frame
    void Update()
    {
        // TODO 借助 Input.GetKey 与 Input.GetButton 与 Input.GetAxis 实现
        // 其中手柄的各个按钮输入事先在 InputManager 中设置好
    }
}

[Serializable]
public class InputMap
{
    public string uiSwitchLeft; // default: Q, button_4(LB), only keyboard configable
    public string uiSwitchRight; // default: E, button_5(RB), only keyboard configable
    public string up; // default: W, left_stick up, only keyboard configable
    public string down; // default: S, left_stick down, only keyboard configable
    public string left; // default: A, left_stick left, only keyboard configable
    public string right; // default: D, left_stick right, only keyboard configable
    public string inventory; // default: P, button_6(view/select), only keyboard configable
    [NonSerialized]
    public string escape; // default: ESC, button_7(menu/start), not configable
    public string attack1; // default: J, button_2(X)
    public string attack2; // default: H, button_3(Y)
    public string attack3; // defalur: U, button_1(B)
    public string jump; // default: K, button_0(A)
    public string dash; // default: L, right_trigger(RT)
    public string heal; // default: R, button_5(RB)
    public string hook; // default: F, left_trigger(LT)
    public string fastMap; // default: TAB, button_4(LB)

    public void Init(GameInput.InputType inputType)
    {
        switch (inputType)
        {
            case GameInput.InputType.Keyoard:
                uiSwitchLeft = "Q";
                uiSwitchRight = "E";
                up = "W";
                down = "S";
                left = "A";
                right = "D";
                inventory = "P";
                escape = "ESC";
                attack1 = "J";
                attack2 = "H";
                attack3 = "U";
                jump = "K";
                dash = "L";
                heal = "R";
                hook = "F";
                fastMap = "TAB";
                break;
            default:
                uiSwitchLeft = "button_4";
                uiSwitchRight = "button_5";
                up = "left_stick";
                down = "left_stick";
                left = "left_stick";
                right = "left_stick";
                inventory = "button_6";
                escape = "button_7";
                attack1 = "button_2";
                attack2 = "button_3";
                attack3 = "button_1";
                jump = "button_0";
                dash = "right_trigger";
                heal = "button_5";
                hook = "left_trigger";
                fastMap = "button_4";
                break;
        }
    }

    public void SetUnconfigable(GameInput.InputType inputType)
    {
        switch (inputType)
        {
            case GameInput.InputType.Keyoard:
                escape = "ESC";
                break;
            default:
                uiSwitchLeft = "button_4";
                uiSwitchRight = "button_5";
                up = "left_stick";
                down = "left_stick";
                left = "left_stick";
                right = "left_stick";
                inventory = "button_6";
                escape = "button_7";
                break;
        }
    }
}
