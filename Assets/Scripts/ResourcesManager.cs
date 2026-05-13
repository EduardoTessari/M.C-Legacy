using UnityEngine;
using System.Collections;

public class ResourcesManager : MonoBehaviour
{
    public static ResourcesManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private ResourceMine uiMinePanel; // Arraste o SEU painel de UI para cá

    void Awake()
    {
        // Padrão Singleton para ser acessado de qualquer lugar facilmente
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OpenMinePanel(MineNode clickedNode)
    {
        // 1. LIGA O PAINEL PRIMEIRO! (Isso permite que a Unity rode a Coroutine)
        uiMinePanel.gameObject.SetActive(true);

        // 2. AGORA SIM, injeta os dados na UI (que já vai dar o StartMining automático)
        uiMinePanel.SetupMine(clickedNode);
    }

    // NOVA FUNÇÃO: Recebe a mina vazia e começa a contar
    public void StartNodeRespawn(MineNode depletedNode)
    {
        StartCoroutine(RespawnRoutine(depletedNode));
    }

    private IEnumerator RespawnRoutine(MineNode node)
    {
        // Espera o tempo configurado lá na pedra
        yield return new WaitForSeconds(node.RespawnTime);

        // Religa a pedra novinha em folha!
        node.ResetNode();
    }
}