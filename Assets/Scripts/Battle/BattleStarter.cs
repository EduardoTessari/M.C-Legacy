using UnityEngine;

public class BattleStarter : MonoBehaviour
{
    [Header("Configuração da Luta")]
    [SerializeField] private int _quantidadeDePlayers = 1; 

    [SerializeField] private PlayersSlotManager _slotManager;

    [Header("UI Canvas Management")]
    public GameObject mainCanvas;
    public GameObject battleCanvas;

    [Header("Environment Management")]
    public GameObject worldEnvironment;
    public GameObject combatEnvironment;

    // A função recebe a maleta (LevelData) do Botão da Interface
    public void StartLevelBattle(LevelData levelData)
    {
        Debug.Log($"Iniciando batalha do {levelData.levelName}! Trocando UI e Ambientes.");

        // 1. Ativa o Ambiente de Combate
        if (worldEnvironment != null) worldEnvironment.SetActive(false);
        if (combatEnvironment != null) combatEnvironment.SetActive(true);

        // 2. Troca a UI
        if (mainCanvas != null) mainCanvas.SetActive(false);
        if (battleCanvas != null) battleCanvas.SetActive(true);

        // 3. Spawna a galera passando a maleta pro SlotManager ler os inimigos
        if (_slotManager != null)
        {
            _slotManager.SpawnCombatentes(_quantidadeDePlayers, levelData);
        }

        // 4. Avisa o Manager pra começar a luta e entrega a maleta pra ele saber os Drops!
        if (BattleManager.instance != null)
        {
            BattleManager.instance.IniciarBatalha(levelData);
        }
    }
}