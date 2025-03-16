using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO 改名 GameInputManager
public class GameInputManager : SingletonBase<GameInputManager>
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

    public KeyboardSettings keyboardSettings = null;
    public ControllerSettings controllerSettings = null;

    public InputActionMap actionMap;
    public InputAction uiSwitch;
    public InputAction move;
    public InputAction inventory;
    public InputAction pause;
    public InputAction attack1;
    public InputAction attack2;
    public InputAction attack3; // also as ui_cancel
    public InputAction jump; // also as ui_confirm
    public InputAction dash;
    public InputAction heal;
    public InputAction hook;

    // 之后全都不用 .triggered/.WasPerformedThisFrame() 而是用 .WasPressedThisFrame()，因为对于连续量(摇杆,扳机)前者仍会连续触发
    public static Vector2 Move
    {
        get
        {
            // 限制移动的斜率
            Vector2 v = instance.move.ReadValue<Vector2>();
            if (v.x == 0 || v.y == 0)
            {
                return v;
            }
            float angle = Mathf.Atan(Mathf.Abs(v.y/v.x)) * Mathf.Rad2Deg;
            if (angle < 20)
            {
                return new Vector2(v.x, 0);
            }
            else if (angle > 70)
            {
                return new Vector2(0, v.y);
            }
            else
            {
                return v;
            }
        }
    }
    public static Vector2 MoveTrigger => instance.move.WasPressedThisFrame() ? Move : Vector2.zero;
    public static float Horizontal => Move.x;
    public static float Vertical => Move.y;
    public static bool UiSwitchLeft => instance.uiSwitch.WasPressedThisFrame() && instance.uiSwitch.ReadValue<float>() < 0;
    public static bool UiSwitchRight => instance.uiSwitch.WasPressedThisFrame() && instance.uiSwitch.ReadValue<float>() > 0;
    // public static float UiSwitch => instance.uiSwitch.ReadValue<float>();
    public static bool UiConfirm => instance.jump.WasPressedThisFrame();
    public static bool UiCancel => instance.attack1.WasPressedThisFrame() || instance.attack3.WasPressedThisFrame() || Keyboard.current.escapeKey.wasPressedThisFrame;
    public static bool Inventory => instance.inventory.WasPressedThisFrame();
    public static bool Pause => instance.pause.WasPressedThisFrame();
    public static bool Attack1 => instance.attack1.WasPressedThisFrame();
    public static bool Attack2 => instance.attack2.WasPressedThisFrame();
    public static bool Attack3 => instance.attack3.WasPressedThisFrame();
    public static bool Jump => instance.jump.WasPressedThisFrame();
    public static bool JumpHold => instance.jump.ReadValue<float>() > 0;
    public static bool Dash => instance.dash.WasPressedThisFrame();
    public static bool Heal => instance.heal.WasPressedThisFrame();
    public static bool Hook => instance.hook.WasPressedThisFrame();

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        actionMap = new InputActionMap("GameInput");
        uiSwitch = actionMap.AddAction("UI_Switch");
        uiSwitch.AddCompositeBinding("Axis")
            .With("Negative", "<Keyboard>/q")
            .With("Positive", "<Keyboard>/e")
            .With("Negative", "<Gamepad>/leftShoulder")
            .With("Positive", "<Gamepad>/rightShoulder");
        move = actionMap.AddAction("Move", binding: "<Gamepad>/leftStick", processors: "stickDeadzone(min=0.5,max=1)");
        move.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d")
            .With("Up", "<Gamepad>/dpad/up")
            .With("Down", "<Gamepad>/dpad/down")
            .With("Left", "<Gamepad>/dpad/left")
            .With("Right", "<Gamepad>/dpad/right");
        inventory = actionMap.AddAction("Inventory", binding: "<Gamepad>/select");
        inventory.AddBinding("<Keyboard>/p");
        pause = actionMap.AddAction("Pause", binding: "<Gamepad>/start");
        pause.AddBinding("<Keyboard>/escape");
        attack1 = actionMap.AddAction("Attack1", binding: "<Gamepad>/buttonWest");
        attack1.AddBinding("<Keyboard>/j");
        attack2 = actionMap.AddAction("Attack2", binding: "<Gamepad>/buttonNorth");
        attack2.AddBinding("<Keyboard>/h");
        attack3 = actionMap.AddAction("Attack3", binding: "<Gamepad>/buttonEast");
        attack3.AddBinding("<Keyboard>/u");
        jump = actionMap.AddAction("Jump", binding: "<Gamepad>/buttonSouth");
        jump.AddBinding("<Keyboard>/k");
        dash = actionMap.AddAction("Dash", binding: "<Gamepad>/rightTrigger");
        dash.AddBinding("<Keyboard>/l");
        heal = actionMap.AddAction("Heal", binding: "<Gamepad>/leftShoulder"); // 也许 LB 用来交互?
        heal.AddBinding("<Keyboard>/r");
        hook = actionMap.AddAction("Hook", binding: "<Gamepad>/leftTrigger");
        hook.AddBinding("<Keyboard>/f");
        actionMap.Enable();
    }

    public void LoadSettings(KeyboardSettings keyboard, ControllerSettings controller) {
        keyboardSettings = keyboard;
        controllerSettings = controller;
        actionMap.Disable();

        foreach (var action in actionMap)
        {
            while (action.bindings.Count > 0)
            {
                action.ChangeBinding(0).Erase();
            }
        }
        uiSwitch.AddCompositeBinding("Axis")
            .With("Negative", "<Keyboard>/" + keyboardSettings.uiSwitchLeft)
            .With("Positive", "<Keyboard>/" + keyboardSettings.uiSwitchRight)
            .With("Negative", "<Gamepad>/" + controller.uiSwitchLeft)
            .With("Positive", "<Gamepad>/" + controller.uiSwitchRight);
        move.AddBinding(path: "<Gamepad>/leftStick", processors: "stickDeadzone(min=0.5,max=1)");
        move.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/" + keyboardSettings.up)
            .With("Down", "<Keyboard>/" + keyboardSettings.down)
            .With("Left", "<Keyboard>/" + keyboardSettings.left)
            .With("Right", "<Keyboard>/" + keyboardSettings.right)
            .With("Up", "<Gamepad>/dpad/up")
            .With("Down", "<Gamepad>/dpad/down")
            .With("Left", "<Gamepad>/dpad/left")
            .With("Right", "<Gamepad>/dpad/right");
        inventory.AddBinding("<Gamepad>/" + controllerSettings.inventory);
        inventory.AddBinding("<Keyboard>/" + keyboardSettings.inventory);
        pause.AddBinding("<Gamepad>/" + controllerSettings.pause);
        pause.AddBinding("<Keyboard>/escape");
        attack1.AddBinding("<Gamepad>/" + controllerSettings.attack1);
        attack1.AddBinding("<Keyboard>/" + keyboardSettings.attack1);
        attack2.AddBinding("<Gamepad>/" + controllerSettings.attack2);
        attack2.AddBinding("<Keyboard>/" + keyboardSettings.attack2);
        attack3.AddBinding("<Gamepad>/" + controllerSettings.attack3);
        attack3.AddBinding("<Keyboard>/" + keyboardSettings.attack3);
        jump.AddBinding("<Gamepad>/" + controllerSettings.jump);
        jump.AddBinding("<Keyboard>/" + keyboardSettings.jump);
        dash.AddBinding("<Gamepad>/" + controllerSettings.dash);
        dash.AddBinding("<Keyboard>/" + keyboardSettings.dash);
        heal.AddBinding("<Gamepad>/" + controllerSettings.heal);
        heal.AddBinding("<Keyboard>/" + keyboardSettings.heal);
        hook.AddBinding("<Gamepad>/" + controllerSettings.hook);
        hook.AddBinding("<Keyboard>/" + keyboardSettings.hook);
        StartCoroutine(EnableEndFrame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator EnableEndFrame()
    {
        yield return new WaitForEndOfFrame();
        actionMap.Enable();
    }
}

[Serializable]
public class KeyboardSettings {
    public string uiSwitchLeft = "q";
    public string uiSwitchRight = "e";
    public string up = "w";
    public string down = "s";
    public string left = "a";
    public string right = "d";
    public string inventory = "p";
    public const string pause = "escape";
    public string attack1 = "j";
    public string attack2 = "h";
    public string attack3 = "u";
    public string jump = "k";
    public string dash = "l";
    public string heal = "r";
    public string hook = "f";
}

[Serializable]
public class ControllerSettings {
    public string uiSwitchLeft = "leftShoulder"; // LB
    public string uiSwitchRight = "rightShoulder"; // RB
    public const string move = "leftStick";
    public string inventory = "select"; // view/select
    public string pause = "start"; // menu/start
    public string attack1 = "buttonWest"; // X
    public string attack2 = "buttonNorth"; // Y
    public string attack3 = "buttonEast"; // B
    public string jump = "buttonSouth"; // A
    public string dash = "rightTrigger"; // RT
    public string heal = "leftShoulder"; // LB
    public string hook = "leftTrigger"; // LT
}