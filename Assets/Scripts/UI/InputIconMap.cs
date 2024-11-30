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
    public Sprite invalidSprite = null;

    public Dictionary<string, Sprite> keyboardIcons = new();
    public Dictionary<string, Sprite> xboxIcons = new();
    public Dictionary<string, Sprite> psIcons = new();
    public Dictionary<string, Sprite> switchIcons = new();

    void OnEnable()
    {
        foreach (var pair in keyboardIconMap)
        {
            keyboardIcons.Add(pair.name, pair.sprite);
        }
        foreach (var pair in xboxIconMap)
        {
            xboxIcons.Add(pair.name, pair.sprite);
        }
        foreach (var pair in psIconMap)
        {
            psIcons.Add(pair.name, pair.sprite);
        }
        foreach (var pair in switchIconMap)
        {
            switchIcons.Add(pair.name, pair.sprite);
        }

        
    }

    public Sprite GetKeyboardIcon(string name)
    {
        return keyboardIcons.ContainsKey(name) ? keyboardIcons[name] : invalidSprite;
    }

    public Sprite GetXboxIcon(string name)
    {
        return xboxIcons.ContainsKey(name) ? xboxIcons[name] : invalidSprite;
    }

    public Sprite GetPsIcon(string name)
    {
        return psIcons.ContainsKey(name) ? psIcons[name] : invalidSprite;
    }

    public Sprite GetSwitchIcon(string name)
    {
        return switchIcons.ContainsKey(name) ? switchIcons[name] : invalidSprite;
    }
}
