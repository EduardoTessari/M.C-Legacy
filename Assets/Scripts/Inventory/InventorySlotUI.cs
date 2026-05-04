using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // FUNDAMENTAL para o mouse funcionar
using TMPro;

// Adicionamos as interfaces IPointerEnterHandler e IPointerExitHandler
public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityText;

    private ItemData currentItem; // Precisamos guardar quem é o item para passar pro Tooltip!

    public void SetupSlot(InventorySlot slotData)
    {
        currentItem = slotData.item; // Salva o item na memória do quadradinho

        iconImage.sprite = currentItem.Icon;
        iconImage.enabled = true;

        if (currentItem.IsStackable && slotData.quantity > 1)
        {
            quantityText.text = slotData.quantity.ToString();
        }
        else
        {
            quantityText.text = "";
        }
    }

    // ==========================================
    // A FOFOCA: O MOUSE ENTROU OU SAIU
    // ==========================================
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null && ItemTooltip.instance != null)
        {
            ItemTooltip.instance.ShowTooltip(currentItem);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ItemTooltip.instance != null)
        {
            ItemTooltip.instance.HideTooltip();
        }
    }

    // A função que o botão vai chamar
    public void OnSlotClick()
    {
        // Se o slot estiver vazio, não faz nada
        if (currentItem == null) return;

        // O teste de identidade: é equipamento?
        if (currentItem is EquipmentData equipamento)
        {
            EquipmentManager.instance.Equip(equipamento);

            if (ItemTooltip.instance != null)
            {
                ItemTooltip.instance.HideTooltip();
            }
        }
        else
        {
            Debug.Log("O item " + currentItem.name + " não pode ser usado.");
        }
    }
}