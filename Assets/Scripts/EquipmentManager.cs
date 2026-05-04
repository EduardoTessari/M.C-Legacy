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

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            CharacterStats stats = playerObj.GetComponent<CharacterStats>();
            stats.UpdateStats();
        }

        // O Manager passa passando um "rádio" para todos os quadradinhos da UI
        foreach (EquipmentSlotUI slot in uiSlots)
        {
            slot.UpdateSlotUI();
        }
    }

    // Método para desequipar baseado no índice do slot (0=Arma, 1=Armadura, etc)
    public void Unequip(int slotIndex)
    {
        // 1. Verifica se existe mesmo algo nesse slot para não dar erro
        if (currentEquipment[slotIndex] != null)
        {
            EquipmentData itemToReturn = currentEquipment[slotIndex];

            // Devolve para a mochila
            InventoryManager.instance.AddItem(itemToReturn, 1);

            // 2. Esvazia a gaveta (Passa para null)
            currentEquipment[slotIndex] = null;

            Debug.Log("Você desequipou: " + itemToReturn.name);

            // 3. O Manager avisa o motor de atributos para recalcular
            // Como o slot agora é null, o foreach do CharacterStats vai somar 0, 
            // efetivamente subtraindo o bônus!
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                CharacterStats stats = playerObj.GetComponent<CharacterStats>();
                stats.UpdateStats();
            }

            // 4. Avisa a UI para limpar a imagem do slot
            foreach (EquipmentSlotUI slot in uiSlots)
            {
                slot.UpdateSlotUI();
            }
        }
    }
}