using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    // Singleton para a gente acessar ele de qualquer lugar, igual fizemos no Inventário
    public static EquipmentManager instance;

    // Um array que vai guardar o que o jogador está vestindo.
    // O tamanho dele será exatamente a quantidade de opções que tem no seu Enum EquipmentType!
    public EquipmentData[] currentEquipment;

    public EquipmentSlotUI[] uiSlots;

    private CharacterStats playerStats;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Esse truquezinho de código conta quantos itens tem no seu Enum (Weapon, Armor, Helmet, Boots)
        // e já cria o array do tamanho exato (4 slots).
        int numSlots = System.Enum.GetNames(typeof(EquipmentType)).Length;
        currentEquipment = new EquipmentData[numSlots];

        // Busca o motor de atributos na cena
        playerStats = FindAnyObjectByType<CharacterStats>();
    }

    // A mágica de equipar acontece aqui
    public void Equip(EquipmentData newItem)
    {
        // Como Enums são, por baixo dos panos, números (Weapon=0, Armor=1, Helmet=2, Boots=3),
        // a gente usa o tipo do item para saber em qual gaveta do array ele vai!
        int slotIndex = (int)newItem.equipType;

        // Se já tiver um equipamento naquele espaço (ex: já estou usando uma espada de madeira e quero botar a de ferro)
        if (currentEquipment[slotIndex] != null)
        {
            EquipmentData oldItem = currentEquipment[slotIndex];

            // Aqui a gente manda a arma velha de volta pra mochila!
            InventoryManager.instance.AddItem(oldItem, 1);
        }

        // Veste o equipamento novo
        currentEquipment[slotIndex] = newItem;

        // Aqui a gente consome o item novo da mochila, já que ele foi pro corpo
        InventoryManager.instance.RemoveItem(newItem, 1);

        Debug.Log("Você equipou: " + newItem.name);

        // O Manager avisa o motor de atributos para recalcular
        if (playerStats != null)
        {
            playerStats.UpdateStats();
        }

        // O Manager passa passando um "rádio" para todos os quadradinhos da UI
        foreach (EquipmentSlotUI slot in uiSlots)
        {
            slot.UpdateSlotUI();
        }
    }
}