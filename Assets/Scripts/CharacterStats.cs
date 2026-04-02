using Unity.VisualScripting;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Atributos Base (Sem Equipamento)")]
    // [SerializeField] deixa aparecer na Unity, mas o 'private' impede outros scripts de bagunçarem
    [SerializeField] private int _baseAttack;
    [SerializeField] private int _baseHealth;
    [SerializeField] private int _baseSpeed;

    // Getters públicos para qualquer um LER, mas o 'private set' garante que só ESTE script pode ALTERAR
    public int CurrentAttack { get; private set; }
    public int CurrentHealth { get; private set; }
    public int CurrentSpeed { get; private set; }

    // Getters rápidos (usando Arrow Functions) caso outro script precise ler os status base originais
    public int BaseAttack => _baseAttack;
    public int BaseDefense => _baseHealth;
    public int BaseSpeed => _baseSpeed;

    [SerializeField] private TMPro.TextMeshProUGUI[] statusText;

    [SerializeField] private PlayerStatsUI statsUI;

    private void Start()
    {
        // Assim que o jogo começa ou o personagem nasce, 
        // os atributos atuais são iguais aos base.
        UpdateStats();
    }

    public void UpdateStats()
    {
        // RESET: Volta ao valor pelado
        CurrentAttack = _baseAttack;
        CurrentHealth = _baseHealth;
        CurrentSpeed = _baseSpeed;

        // SOMA: Se o gerente existir, vamos olhar o que ele tem
        if (EquipmentManager.instance != null)
        {
            foreach (EquipmentData equip in EquipmentManager.instance.currentEquipment)
            {
                if (equip != null)
                {
                    CurrentAttack += equip.bonusAttack;
                    CurrentHealth += equip.bonusHealth;
                    CurrentSpeed += equip.bonusSpeed;
                }
            }
        }

        // UI: Avisa a tela
        if (statsUI != null)
        {
            statsUI.AtualizarTextosNaTela();
        }
    }
}