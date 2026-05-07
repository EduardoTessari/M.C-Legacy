using UnityEngine;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    // O [SerializeField] faz a mágica: a variável é PRIVADA (ninguém mexe via código),
    // mas a portinha no Inspector da Unity continua aberta para você arrastar o jogador!
    [SerializeField] private CharacterStats playerStats;

    [Header("Configurações de Texto")] // Só para organizar no Inspector
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI speedText;

    public void AtualizarTextosNaTela()
    {
        // Os Getters que você criou (CurrentAttack, etc) garantem que a UI 
        // só LEIA os dados, sem nunca conseguir alterar a força do boneco por acidente.
        attackText.text = playerStats.CurrentAttack.ToString();
        healthText.text = playerStats.CurrentHealth.ToString();
        speedText.text = playerStats.CurrentSpeed.ToString();
        defenseText.text = playerStats.CurrentDefense.ToString();
    }
}