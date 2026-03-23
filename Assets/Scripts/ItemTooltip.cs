using TMPro;
using UnityEngine;

public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip instance;

    [Header("Referências Visuais")]
    [SerializeField] private GameObject tooltipPanel; // Arraste o fundo principal do tooltip aqui
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI statsText; // O texto de +Ataque, +Vida...

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        tooltipPanel.SetActive(false); // Garante que comece escondido
    }

    private void Update()
    {
        // Se estiver aparecendo, gruda na posição do mouse!
        if (tooltipPanel.activeSelf)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void ShowTooltip(ItemData item)
    {
        nameText.text = item.ItemName;
        descriptionText.text = item.Description;

        // O PULO DO GATO (Pattern Matching): É um equipamento?
        if (item is EquipmentData equipment)
        {
            statsText.gameObject.SetActive(true);

            // Monta o texto dos atributos (só adiciona se for maior que zero)
            string stats = "";
            if (equipment.bonusAttack > 0) stats += $"+{equipment.bonusAttack} Ataque\n";
            if (equipment.bonusHealth > 0) stats += $"+{equipment.bonusHealth} Vida\n";
            if (equipment.bonusSpeed > 0) stats += $"+{equipment.bonusSpeed} Velocidade";

            statsText.text = stats;
        }
        else
        {
            // Se for só uma madeira/pedra, esconde a caixa de status
            statsText.gameObject.SetActive(false);
        }

        tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}
