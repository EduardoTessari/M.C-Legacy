using System.Collections.Generic;
using UnityEngine;

// [System.Serializable] faz essa classe aparecer no Inspector da Unity!
[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity;

    // Construtor para facilitar a criação do slot
    public InventorySlot(ItemData newItem, int newQuantity)
    {
        item = newItem;
        quantity = newQuantity;
    }
}

public class InventoryManager : MonoBehaviour
{
    // Padrão Singleton: Permite que qualquer script acesse o inventário fácil
    public static InventoryManager instance;

    [Header("Mochila do Player")]
    public List<InventorySlot> inventory = new List<InventorySlot>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddItem(ItemData itemToAdd, int amount)
    {
        // PASSO 1: O item é empilhável? (Dica: chame a variável isStackable do ItemData)
        // Se SIM, faça um 'foreach' na lista 'inventory' procurando se o slot.item == itemToAdd.
        // PASSO 2: Se achou no loop, faça a quantity do slot receber + amount, 
        // e use um 'return;' para parar a função por aqui.
        // PASSO 3: Se não achou (ou se não for empilhável), crie um novo InventorySlot.
        // Adicione esse novo slot na lista 'inventory'

        if (itemToAdd.IsStackable)
        {
            foreach (InventorySlot slot in inventory)
            {
                if (slot.item == itemToAdd)
                {
                    slot.quantity += amount;

                    // AVISO AQUI: Antes de sair, avise a UI!
                    if (InventoryUI.instance != null) InventoryUI.instance.UpdateUI();


                    return; // Para a função aqui, já que o item foi adicionado.
                }
            }
        }

        // Se chegou aqui, é um item novo
        InventorySlot newSlot = new InventorySlot(itemToAdd, amount);
        inventory.Add(newSlot);

        if (InventoryUI.instance != null)
        {
            // AVISO AQUI TAMBÉM: Para quando o item for novidade na mochila
            InventoryUI.instance.UpdateUI();
        }



    }
}