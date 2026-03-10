using UnityEngine;

public class BattleStarter : MonoBehaviour
{
    [Header("UI Canvas Management")]
    public GameObject mainCanvas;
    public GameObject battleCanvas;

    [Header("Environment Management")]
    public GameObject worldEnvironment;  // Onde ficam o chão, seu player andando e os NPCs do mapa
    public GameObject combatEnvironment; // Onde ficam os seus 20 slots fixos de batalha

    private void OnMouseDown()
    {
        Debug.Log("Iniciando batalha! Trocando UI e Ambientes.");

        // 2. Troca o Mundo (3D/2D)
        if (worldEnvironment != null) worldEnvironment.SetActive(false);
        if (combatEnvironment != null) combatEnvironment.SetActive(true);

        // 1. Troca a Interface (UI)
        if (mainCanvas != null) mainCanvas.SetActive(false);
        if (battleCanvas != null) battleCanvas.SetActive(true);

        // 3. A MÁGICA AQUI: O cenário já está ativado, então mandamos o Manager puxar os lutadores!
        if (BattleManager.instance != null)
        {
            BattleManager.instance.IniciarBatalha();
        }
    }
}