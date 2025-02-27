using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : ShownUI
{
    public override bool Shown
    {
        get { return gameObject.activeSelf; }
        set 
        { 
            if (!gameObject.activeSelf && value)
            {
                GameManager.Pause = true;
            }
            else if (gameObject.activeSelf && !value)
            {
                GameManager.Pause = false;
            }
            gameObject.SetActive(value); 
        }
    }

    public SelectItem gameSettingHeader; // 0
    public SelectItem volumeSettingHeader; // 1
    public SelectItem graphicsSettingHeader; // 2
    public SelectItem controllerSettingHeader; // 3
    public SelectItem keyboardSettingHeader; // 4

    private List<SelectItem> headers = new();

    public int selected = 0;

    void Awake()
    {
        if (headers.Count != 0)
            headers.Clear();
        headers.Add(gameSettingHeader);
        headers.Add(volumeSettingHeader);
        headers.Add(graphicsSettingHeader);
        headers.Add(controllerSettingHeader);
        headers.Add(keyboardSettingHeader);
        selected = 0;
    }

    void OnEnable()
    {
        if (headers.Count == 0)
        {
            headers.Add(gameSettingHeader);
            headers.Add(volumeSettingHeader);
            headers.Add(graphicsSettingHeader);
            headers.Add(controllerSettingHeader);
            headers.Add(keyboardSettingHeader);
            selected = 0;
        }
        HandleShownMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && selected >= 1)
        {
            selected -= 1;
            HandleShownMenu();
            SoundManager.instance.PlayUISelection();
        }
        else if (Input.GetKeyDown(KeyCode.E) && selected <= headers.Count - 2)
        {
            selected += 1;
            HandleShownMenu();
            SoundManager.instance.PlayUISelection();
        }
    }

    void HandleShownMenu()
    {
        if (selected >= 0 && selected < headers.Count)
        {
            for (int i = 0; i < headers.Count; i++)
            {
                if (i == selected)
                {
                    headers[i].selected = true;
                }
                else
                {
                    headers[i].selected = false;
                }
            }
        }
    }
}
