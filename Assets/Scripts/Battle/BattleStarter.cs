using UnityEngine;

public class BattleStarter : MonoBehaviour
{
    [Header("Configuração da Luta")]
    [SerializeField] private int _quantidadeDeInimigos = 1;
    [SerializeField] private int _quantidadeDePlayers = 1; // Corrigido o "1;1"

    [SerializeField] private PlayersSlotManager _slotManager;

    [Header("UI Canvas Management")]
    public GameObject mainCanvas;
    public GameObject battleCanvas;

    [Header("Environment Management")]
    public GameObject worldEnvironment;
    public GameObject combatEnvironment;

    private void OnMouseDown()
    {
        Debug.Log("Iniciando batalha! Trocando UI e Ambientes.");

        // 1. Primeiro a gente ativa o "palco" (Ambiente de Combate)
        if (worldEnvironment != null) worldEnvironment.SetActive(false);
        if (combatEnvironment != null) combatEnvironment.SetActive(true);

        // 2. Depois a gente troca a "roupa" (UI)
        if (mainCanvas != null) mainCanvas.SetActive(false);
        if (battleCanvas != null) battleCanvas.SetActive(true);

        // 3. Agora que o palco existe, a gente manda os atores nascerem!
        if (_slotManager != null)
        {
            _slotManager.SpawnCombatentes(_quantidadeDePlayers, _quantidadeDeInimigos);
        }

        // 4. Por fim, avisa o Maestro (Manager) quem chamou e pede pra começar
        if (BattleManager.instance != null)
        {
            BattleManager.instance.npcThatCalled = this;
            BattleManager.instance.IniciarBatalha();

            // Desativa o clique pra não iniciar a mesma batalha duas vezes
            GetComponent<Collider2D>().enabled = false;
            this.enabled = false;
        }
    }

    public void ResetNPC()
    {
        this.enabled = true;
        if (GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = true;
        Debug.Log("<color=green>NPC resetado e pronto para outra!</color>");
    }
}