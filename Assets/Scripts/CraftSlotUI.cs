using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class CraftSlotUI : MonoBehaviour
{
    [SerializeField] Image _itemIcon;
    [SerializeField] TextMeshProUGUI _itemAmountText;

    public void SetupSlot(ItemData item, int amount)
    {
        _itemIcon.sprite = item.Icon;
        _itemAmountText.text = amount.ToString();
    }


}