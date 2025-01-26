using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputIcon : MonoBehaviour
{

    public Image iconImage;
    public string inputName;
    public GameInput.InputType inputType; // just for test

    // Start is called before the first frame update
    void Start()
    {
        iconImage.sprite = inputType switch
        {
            GameInput.InputType.Keyoard => GameManager.instance.gamePreload.inputIconMap.GetKeyboardIcon(inputName),
            GameInput.InputType.XboxController => GameManager.instance.gamePreload.inputIconMap.GetXboxIcon(inputName),
            GameInput.InputType.PsController => GameManager.instance.gamePreload.inputIconMap.GetPsIcon(inputName),
            GameInput.InputType.SwitchController => GameManager.instance.gamePreload.inputIconMap.GetSwitchIcon(inputName),
        };
    }
}
