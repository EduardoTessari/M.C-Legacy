using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance; // Outro Singleton para facilitar nossa vida!

    [Header("Referências Visuais")]
    [SerializeField] private Transform gridParent; // Arraste o objeto que tem o Grid Layout aqui
    [SerializeField] private GameObject slotPrefab; // Arraste o seu Prefab do Slot aqui

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        // Toda vez que a mochila for aberta (ativada), 
        // ela se força a desenhar os itens atualizados!
        UpdateUI();
    }

    // A Mágica Acontece Aqui:
    public void UpdateUI()
    {
        // 1. Limpa o painel (destrói todos os slots antigos para não duplicar)
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        // 2. Pega a lista de itens da mochila
        var currentInventory = InventoryManager.instance.inventory;

        // 3. Para cada item na mochila, cria um quadradinho novo
        foreach (InventorySlot slot in currentInventory)
        {
            // Instancia o prefab dentro do Grid
            GameObject newSlotObj = Instantiate(slotPrefab, gridParent);

            // Pega o script do "Pincel" e manda ele se pintar
            InventorySlotUI slotUI = newSlotObj.GetComponent<InventorySlotUI>();
            if (slotUI != null)
            {
                slotUI.SetupSlot(slot);
            }
        }
    }
}