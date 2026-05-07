using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image _itemIcon;
    [SerializeField] TextMeshProUGUI _itemAmountText;

    private ItemData _currentItem; // Variável para guardar o item

    public void SetupSlot(ItemData item, int amount)
    {
        _currentItem = item; // Guarda a referência aqui!
        _itemIcon.sprite = item.Icon;
        _itemAmountText.text = amount.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_currentItem != null)
        {
            string header = $"<b>{_currentItem.ItemName}</b>";
            string content = _currentItem.Description;

            // Tenta converter o ItemData para EquipmentData
            EquipmentData equip = _currentItem as EquipmentData;

            // Se a conversão funcionou, significa que este item É um equipamento
            if (equip != null)
            {
                content += "\n"; // Espaço entre descrição e atributos

                if (equip.bonusAttack > 0)
                    content += $"\n<color=#FF5555>Ataque: +{equip.bonusAttack}</color>";

                if (equip.bonusDefense > 0)
                    content += $"\n<color=#5555FF>Defesa: +{equip.bonusDefense}</color>";
                if (equip.bonusHealth > 0)
                    content += $"\n<color=#FF5555>Vida: +{equip.bonusHealth}</color>";

                if (equip.bonusSpeed > 0)
                    content += $"\n<color=#5555FF>Velocidade: +{equip.bonusSpeed}</color>";

                if (equip.equipSet != EquipamentSet.None)
                    content += $"\n<color=#FFD700>Set: {equip.equipSet}</color>";
            }

            TooltipManager.instance.Show($"{header}\n<size=80%>{content}</size>");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipManager.instance != null)
            TooltipManager.instance.Hide();
    }
}