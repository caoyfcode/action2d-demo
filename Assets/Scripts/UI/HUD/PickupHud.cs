using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupHud : MonoBehaviour
{
    public Image icon;
    public Text text;
    public Image background;

    public bool HudShown
    {
        set
        {
            icon.gameObject.SetActive(value);
            text.gameObject.SetActive(value);
            background.gameObject.SetActive(value);
        }
    }

    public readonly Queue<InventoryItem> items = new();

    private float _currentAlpha = 0;
    private float Alpha
    {
        get => _currentAlpha;
        set
        {
            _currentAlpha = value;
            Color tmp = icon.color;
            tmp.a = _currentAlpha;
            icon.color = tmp;

            tmp = text.color;
            tmp.a = _currentAlpha;
            text.color = tmp;

            tmp = background.color;
            tmp.a = _currentAlpha;
            background.color = tmp;
        }
    }
    private State state = State.Hidden;
    private float shownTimer = 0;

    enum State
    {
        Hidden,
        Showing,
        Shown,
        Hiding
    }

    public void AddItem(InventoryItem item)
    {
        items.Enqueue(item);
    }

    void Start()
    {
        state = State.Hidden;
        items.Clear();
        HudShown = false;
    }

    void Update()
    {
        if (state == State.Hidden)
        {
            if (items.Count > 0)
            {
                state = State.Showing;
                Alpha = 0;
                HudShown = true;
                InventoryItem item = items.Dequeue();
                text.text = "x " + item.quantity;
                icon.sprite = WorldManager.instance.itemData.GetItemSprite(item.id);
            }
            else
            {
                HudShown = false;
            }
        }
        else if (state == State.Showing)
        {
            if (Alpha < 1)
            {
                Alpha = Mathf.Clamp01(Alpha + Time.deltaTime * 4);
            }
            if (Alpha >= 1)
            {
                Alpha = 1;
                state = State.Shown;
                shownTimer = 0.5f;
            }
        }
        else if (state == State.Shown)
        {
            Alpha = 1;
            shownTimer -= Time.deltaTime;
            if (shownTimer <= 0)
            {
                state = State.Hiding;
            }
        }
        else if (state == State.Hiding)
        {
            if (Alpha > 0)
            {
                Alpha = Mathf.Clamp01(Alpha - Time.deltaTime * 4);
            }
            if (Alpha <= 0)
            {
                Alpha = 0;
                state = State.Hidden;
                HudShown = false;
            }
        }
    }
}
