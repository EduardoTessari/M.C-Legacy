using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceMine : MonoBehaviour
{
    [Header("Mine Settings")]
    [SerializeField] private float gatherTime = 3f;

    [Header("UI References")]
    [SerializeField] private Image slotIcon;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Slider timeSlider;

    private MineNode activeNode; // A mina que o jogador está olhando agora!
    private Coroutine miningCoroutine;
    private bool isMining = false;

    // Essa é a função mágica que o Manager chama para "configurar" o painel
    public void SetupMine(MineNode node)
    {
        activeNode = node;

        // Atualiza a foto e o texto baseado na mina que foi clicada
        slotIcon.sprite = activeNode.ResourceData.Icon;
        timeSlider.value = 0f;
        UpdateUI();

        // Se já estava minerando outra coisa, para tudo
        StopMining();

        // E começa a minerar a nova mina clicada
        StartMining();
    }

    public void StartMining()
    {
        // Só começa se não estiver minerando E se a mina do mundo ainda tiver recurso
        if (isMining || activeNode.CurrentAmount <= 0) return;

        isMining = true;
        miningCoroutine = StartCoroutine(MineCoroutine());
        Debug.Log($"Mining {activeNode.ResourceData.ItemName} started!");
    }

    public void StopMining()
    {
        if (!isMining) return;

        if (miningCoroutine != null)
        {
            StopCoroutine(miningCoroutine);
        }

        isMining = false;
        timeSlider.value = 0f;
        Debug.Log("Mining stopped.");
    }

    IEnumerator MineCoroutine()
    {
        // Enquanto a mina lá no mapa tiver recurso...
        while (activeNode.CurrentAmount > 0)
        {
            float elapsedTime = 0f;

            while (elapsedTime < gatherTime)
            {
                elapsedTime += Time.deltaTime;
                timeSlider.value = elapsedTime / gatherTime;
                yield return null;
            }

            ExtractResource();
        }

        MineDepleted();
    }

    void ExtractResource()
    {
        // 1. Tira da mina lá do mapa
        activeNode.ExtractOneResource();

        // 2. Atualiza a tela
        UpdateUI();

        // 3. Manda pro inventário (usando o dado que veio da mina)
        InventoryManager.instance.AddItem(activeNode.ResourceData, 1);

        // 1. Pega o horário atual e formata como "Hora:Minuto:Segundo"
        string timeString = System.DateTime.Now.ToString("HH:mm:ss");

        // 2. Monta a mensagem no formato [Hora]: O que aconteceu
        GameLogManager.Instance.AddLogMessage($"[{timeString}]: <color=orange>+1 {activeNode.ResourceData.ItemName}</color>");

        Debug.Log($"<color=orange>+1 {activeNode.ResourceData.ItemName}</color>");
    }

    void UpdateUI()
    {
        // Lê as informações diretamente do Node
        amountText.text = $"{activeNode.CurrentAmount}/{activeNode.MaxAmount}";
    }

    void MineDepleted()
    {
        StopMining();

        // 1. Desativa a pedra no mundo
        activeNode.gameObject.SetActive(false);

        // 2. Avisa o Manager para começar a contar o tempo
        ResourcesManager.Instance.StartNodeRespawn(activeNode);

        // 3. Fecha o painel de UI (já que não tem mais o que minerar)
        gameObject.SetActive(false);
    }
}