using Unity.VisualScripting;
using UnityEngine;

public class CollectItem : MonoBehaviour
{
    [SerializeField] ItemData ItemData; // Referência ao ScriptableObject do item que queremos coletar
    [SerializeField] int amount = 1; // Quantidade a ser coletada (pode ser 1 para itens não empilháveis, ou mais para empilháveis)

    public void OnMouseDown()
    {
        Collect();
    }

    private void Collect()
    {
        InventoryManager.instance.AddItem(ItemData, amount); // Pega o ItemData do objeto e adiciona 1 unidade no inventário
        Debug.Log("Coletado: " + ItemData.ItemName + " x" + amount); // Log para confirmar a coleta (opcional);
    }
}
