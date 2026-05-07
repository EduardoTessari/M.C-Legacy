using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // A SUA "CHAVE" ESTÁ AQUI!
    // Lá na Unity, você vai clicar no slot do capacete e escolher "Helmet" nessa caixinha.
    public EquipmentType mySlotType;

    public Image itemIcon;

    public void UpdateSlotUI()
    {
        // 1. Qual é o meu número? (A chave entra em ação)
        int myIndex = (int)mySlotType;

        // 2. Olha no armário do Manager usando a minha chave
        EquipmentData itemNoMeuSlot = EquipmentManager.instance.currentEquipment[myIndex];

        // 3. Se tiver item lá, mostra a foto. Se não tiver, apaga a foto.
        if (itemNoMeuSlot != null)
        {
            itemIcon.sprite = itemNoMeuSlot.Icon;
            itemIcon.enabled = true;
        }
        else
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }
    }

    public void OnClickUnequip()
    {
        // Chama o Manager pedindo para desequipar o tipo de item deste slot
        // Castamos o Enum para int para o Manager saber o índice (ex: Weapon vira 0)
        EquipmentManager.instance.Unequip((int)mySlotType);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 1. Buscamos o item que está no array agora usando o nosso índice
        int myIndex = (int)mySlotType;
        EquipmentData currentItem = EquipmentManager.instance.currentEquipment[myIndex];

        // 2. Se o slot não estiver vazio, mostramos o Tooltip
        if (currentItem != null && TooltipManager.instance != null)
        {
            string header = $"<b>{currentItem.ItemName}</b>";
            string content = currentItem.Description;

            
            content += "\n"; // Espaço entre descrição e atributos

            if (currentItem.bonusAttack > 0)
                content += $"\n<color=#FF5555>Ataque: +{currentItem.bonusAttack}</color>";

            if (currentItem.bonusDefense > 0)
                content += $"\n<color=#5555FF>Defesa: +{currentItem.bonusDefense}</color>";
            if (currentItem.bonusHealth > 0)
                content += $"\n<color=#FF5555>Vida: +{currentItem.bonusHealth}</color>";

            if (currentItem.bonusSpeed > 0)
                content += $"\n<color=#5555FF>Velocidade: +{currentItem.bonusSpeed}</color>";

            if (currentItem.equipSet != EquipamentSet.None)
                content += $"\n<color=#FFD700>Set: {currentItem.equipSet}</color>";

            TooltipManager.instance.Show($"{header}\n<size=80%>{content}</size>");
        }


    }


    public void OnPointerExit(PointerEventData eventData)
    {
        // Só desliga o manager
        if (TooltipManager.instance != null)
            TooltipManager.instance.Hide();
    }
}