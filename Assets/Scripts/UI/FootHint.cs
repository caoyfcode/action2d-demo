using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FootHint : MonoBehaviour
{
    public enum FootHintType {
        Confirm, // 确认
        Cancel, // 取消
        Teleport, // 传送
        Equip1, // 装备1
        Equip2, // 装备2
        Equip3, // 装备3
        // Unequip, // 卸下
    }
    public List<FootHintType> hints = new List<FootHintType>();
    public GameObject hintPrefab;

    void Start()
    {
        SetHints(hints);
    }

    public void SetHints(List<FootHintType> hints) {
        int count = transform.childCount;
        if (count > hints.Count) {
            for (int i = count - 1; i >= hints.Count; i--) {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < hints.Count; i++) {
            GameObject hint = null;
            if (i >= count) {
                hint = Instantiate(hintPrefab, transform);
            } else {
                hint = transform.GetChild(i).gameObject;
            }
            Image image = hint.GetComponentInChildren<Image>();
            Text text = hint.GetComponentInChildren<Text>();
            switch (hints[i]) {
                case FootHintType.Confirm:
                    image.sprite = GameManager.instance.gamePreload.inputIconMap.GetXboxIcon("button_0"); // A
                    text.text = "确认";
                    break;
                case FootHintType.Cancel:
                    image.sprite = GameManager.instance.gamePreload.inputIconMap.GetXboxIcon("button_1"); // B
                    text.text = "取消";
                    break;
                case FootHintType.Teleport:
                    image.sprite = GameManager.instance.gamePreload.inputIconMap.GetXboxIcon("button_0"); // A
                    text.text = "传送";
                    break;
                case FootHintType.Equip1:
                    image.sprite = GameManager.instance.gamePreload.inputIconMap.GetXboxIcon("button_2");
                    text.text = "装备1";
                    break;
                case FootHintType.Equip2:
                    image.sprite = GameManager.instance.gamePreload.inputIconMap.GetXboxIcon("button_3");
                    text.text = "装备2";
                    break;
                case FootHintType.Equip3:
                    image.sprite = GameManager.instance.gamePreload.inputIconMap.GetXboxIcon("button_1");
                    text.text = "装备3";
                    break;
            }
        }
    }
}
