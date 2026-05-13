using UnityEngine;

public class MineNode : MonoBehaviour
{
    [Header("Node Configuration")]
    [SerializeField] private ItemData resourceData;
    [SerializeField] private int maxAmount = 50;
    [SerializeField] private float respawnTime = 10f; // Tempo em segundos para voltar

    private int currentAmount;

    // Getters para a UI conseguir ler essas informações com segurança
    public ItemData ResourceData => resourceData;
    public int CurrentAmount => currentAmount;
    public int MaxAmount => maxAmount;
    public float RespawnTime => respawnTime; // Getter para o Manager ler

    void Start()
    {
        // Quando o jogo começa, a mina está cheia
        currentAmount = maxAmount;
    }

    // Função para extrair recurso com segurança (Encapsulamento!)
    public void ExtractOneResource()
    {
        if (currentAmount > 0)
        {
            currentAmount--;
        }
    }

    // CHAME ESSA FUNÇÃO NO ONCLICK DO BOTÃO DESSA MINA OU NO ONMOUSEDOWN
    private void OnMouseDown()
    {
        ResourcesManager.Instance.OpenMinePanel(this);
    }

    public void ResetNode()
    {
        currentAmount = maxAmount;
        gameObject.SetActive(true);
    }
}