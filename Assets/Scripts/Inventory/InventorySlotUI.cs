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
        if (currentItem != null && TooltipManager.instance != null)
        {
            // Criamos a string de forma dinâmica aqui
            string header = $"<b>{currentItem.ItemName}</b>";
            string content = currentItem.Description;

            // Se for magia de status, adicionamos a info extra
            // Tenta converter o ItemData para EquipmentData
            EquipmentData equip = currentItem as EquipmentData;

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
        // Só desliga o manager
        if (TooltipManager.instance != null)
            TooltipManager.instance.Hide();
    }

    // A função que o botão vai chamar
    public void OnSlotClick()
    {
        // Se o slot estiver vazio, não faz nada
        if (currentItem == null) return;

        // O teste de identidade: é equipamento?
        if (currentItem is EquipmentData equipamento)
        {
            if (TooltipManager.instance != null)
            {
                TooltipManager.instance.Hide();
            }
                
            EquipmentManager.instance.Equip(equipamento);
        }
        
        else
        {
            Debug.Log("O item " + currentItem.name + " não pode ser usado.");
        }
    }
}