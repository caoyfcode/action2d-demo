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
            GameInput.InputType.Keyoard => UIManager.instance.inputIconMap.GetKeyboardIcon(inputName),
            GameInput.InputType.XboxController => UIManager.instance.inputIconMap.GetXboxIcon(inputName),
            GameInput.InputType.PsController => UIManager.instance.inputIconMap.GetPsIcon(inputName),
            GameInput.InputType.SwitchController => UIManager.instance.inputIconMap.GetSwitchIcon(inputName),
        };
    }
}
