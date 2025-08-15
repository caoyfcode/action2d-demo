using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class FootHint : MonoBehaviour
{
    public enum FootHintType {
        Confirm, // 确认
        Cancel, // 取消
        Teleport, // 传送
        Equip, // 装备
        EquipSwitch, // 切换装备
        ZoomUp, // 放大
        ZoomDown, // 缩小
        // Unequip, // 卸下
    }
    public List<FootHintType> hints = new List<FootHintType>();
    public GameObject hintPrefab;

    void Update()
    {
        int count = transform.childCount;
        for (int i = 0; i < hints.Count && i < count; i++)
        {
            SetChildHint(transform.GetChild(i).gameObject, hints[i]);
        }
        for (int i = count; i < hints.Count; i++)
        {
            SetChildHint(Instantiate(hintPrefab, transform), hints[i]);
        }
        for (int i = count - 1; i >= hints.Count; i--) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void SetChildHint(GameObject child, FootHintType hintType) {
        Image image = child.GetComponentInChildren<Image>();
        Text text = child.GetComponentInChildren<Text>();
        switch (hintType) {
            case FootHintType.Confirm:
                image.sprite = GameInputManager.Ins.GetInputIcon(GameInputManager.InputActionEnum.UiConfirm);
                text.text = "确认";
                break;
            case FootHintType.Cancel:
                image.sprite = GameInputManager.Ins.GetInputIcon(GameInputManager.InputActionEnum.UiCancel);
                text.text = "取消";
                break;
            case FootHintType.Teleport:
                image.sprite = GameInputManager.Ins.GetInputIcon(GameInputManager.InputActionEnum.UiConfirm); // A
                text.text = "传送";
                break;
            case FootHintType.Equip:
                image.sprite = GameInputManager.Ins.GetInputIcon(GameInputManager.InputActionEnum.UiConfirm);
                text.text = "装备";
                break;
            case FootHintType.EquipSwitch:
                image.sprite = GameInputManager.Ins.GetInputIcon(GameInputManager.InputActionEnum.UiConfirm);
                text.text = "切换";
                break;
            case FootHintType.ZoomUp:
                image.sprite = GameInputManager.Ins.GetInputIcon(GameInputManager.InputActionEnum.UiConfirm);
                text.text = "放大";
                break;
            case FootHintType.ZoomDown:
                image.sprite = GameInputManager.Ins.GetInputIcon(GameInputManager.InputActionEnum.Attack1);
                text.text = "缩小";
                break;
        }
    }

    public void SetHints(List<FootHintType> hints) {
        this.hints = hints;
    }
}
