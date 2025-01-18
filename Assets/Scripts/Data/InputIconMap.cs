using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InputIconMap", menuName = "ScriptableObject/InputIconMap", order = 1)]
public class InputIconMap : ScriptableObject
{
    [Serializable]
    class NameSpritePair
    {
        public string name;
        public Sprite sprite;
    }

    [SerializeField]
    private List<NameSpritePair> keyboardIconMap = new();
    [SerializeField]
    private List<NameSpritePair> xboxIconMap = new();
    [SerializeField]
    private List<NameSpritePair> psIconMap = new();
    [SerializeField]
    private List<NameSpritePair> switchIconMap = new();

    public Dictionary<string, Sprite> keyboardIcons = new();
    public Dictionary<string, Sprite> xboxIcons = new();
    public Dictionary<string, Sprite> psIcons = new();
    public Dictionary<string, Sprite> switchIcons = new();

    void OnEnable()
    {
        keyboardIcons.Clear();
        foreach (var pair in keyboardIconMap)
        {
            keyboardIcons.Add(pair.name, pair.sprite);
        }
        xboxIcons.Clear();
        foreach (var pair in xboxIconMap)
        {
            xboxIcons.Add(pair.name, pair.sprite);
        }
        psIcons.Clear();
        foreach (var pair in psIconMap)
        {
            psIcons.Add(pair.name, pair.sprite);
        }
        switchIcons.Clear();
        foreach (var pair in switchIconMap)
        {
            switchIcons.Add(pair.name, pair.sprite);
        }
    }

    public Sprite GetKeyboardIcon(string name)
    {
        return keyboardIcons.ContainsKey(name) ? keyboardIcons[name] : null;
    }

    public Sprite GetXboxIcon(string name)
    {
        return xboxIcons.ContainsKey(name) ? xboxIcons[name] : null;
    }

    public Sprite GetPsIcon(string name)
    {
        return psIcons.ContainsKey(name) ? psIcons[name] : null;
    }

    public Sprite GetSwitchIcon(string name)
    {
        return switchIcons.ContainsKey(name) ? switchIcons[name] : null;
    }

    // 将 unity InputManager 的键盘名字转换为图标名字
    public static string KeyboardInputNameToIconName(string name)
    {
        if (name == "left shift" || name == "right shift")
        {
            return "shift";
        }
        else if (name == "left ctrl" || name == "right ctrl")
        {
            return "ctrl";
        }
        else if (name == "left alt" || name == "right alt")
        {
            return "alt";
        }
        return name;
    }

    public static string[] KeyboardIconNameHasTwoVariants(string name)
    {
        if (name == "shift")
        {
            return new string[] { "left shift", "right shift" };
        }
        else if (name == "ctrl")
        {
            return new string[] { "left ctrl", "right ctrl" };
        }
        else if (name == "alt")
        {
            return new string[] { "left alt", "right alt" };
        }
        return null;
    }
}
