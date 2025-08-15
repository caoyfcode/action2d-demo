using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputIcon : MonoBehaviour
{

    public Image iconImage;
    public GameInputManager.InputActionEnum inputAction;
    public string inputName;
    public GameInputManager.InputType inputType; // just for test

    // Start is called before the first frame update
    void Start()
    {
        // iconImage.sprite = inputType switch
        // {
        //     GameInputManager.InputType.Keyoard => GameManager.instance.gamePreload.inputIconMap.GetKeyboardIcon(inputName),
        //     GameInputManager.InputType.XboxController => GameManager.instance.gamePreload.inputIconMap.GetXboxIcon(inputName),
        //     GameInputManager.InputType.PsController => GameManager.instance.gamePreload.inputIconMap.GetPsIcon(inputName),
        //     GameInputManager.InputType.SwitchController => GameManager.instance.gamePreload.inputIconMap.GetSwitchIcon(inputName),
        // };
        
    }

    void Update()
    {
        iconImage.sprite = GameInputManager.Ins.GetInputIcon(inputAction);
    }
}
