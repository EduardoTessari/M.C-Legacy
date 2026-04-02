using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour
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
}