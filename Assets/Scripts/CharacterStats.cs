using UnityEngine;

public interface ICharacterStats
{
    int CurrentAttack { get; }
    int CurrentDefense { get; } // Nova integrante!
    int CurrentHealth { get; }
    int CurrentSpeed { get; }
}

public class CharacterStats : MonoBehaviour, ICharacterStats
{
    [Header("Atributos Base (Sem Equipamento)")]
    [SerializeField] private int _baseAttack;
    [SerializeField] private int _baseDefense; // Criada!
    [SerializeField] private int _baseHealth;
    [SerializeField] private int _baseSpeed;

    // Getters públicos
    public int CurrentAttack { get; private set; }
    public int CurrentDefense { get; private set; } // Criada!
    public int CurrentHealth { get; private set; }
    public int CurrentSpeed { get; private set; }

    // Getters rápidos corrigidos
    public int BaseAttack => _baseAttack;
    public int BaseDefense => _baseDefense; // Corrigido de Health para Defense
    public int BaseHealth => _baseHealth;
    public int BaseSpeed => _baseSpeed;

    [SerializeField] private PlayerStatsUI statsUI;

    private void Start()
    {
        UpdateStats();
    }

    public void UpdateStats()
    {
        // RESET: Volta ao valor base
        CurrentAttack = _baseAttack;
        CurrentDefense = _baseDefense; // Resetando Defesa
        CurrentHealth = _baseHealth;
        CurrentSpeed = _baseSpeed;

        // SOMA: Bônus de equipamentos
        if (gameObject.CompareTag("Player") && EquipmentManager.instance != null)
        {
            foreach (EquipmentData equip in EquipmentManager.instance.currentEquipment)
            {
                if (equip != null)
                {
                    CurrentAttack += equip.bonusAttack;
                    CurrentDefense += equip.bonusDefense; // Soma bônus de Defesa
                    CurrentHealth += equip.bonusHealth;
                    CurrentSpeed += equip.bonusSpeed;
                }
            }

            // --- Lógica de Bônus de Conjunto ---
            int pecasFuria = 0;
            int pecasDefesaEterna = 0;

            // Só contamos se for o Player, para economizar processamento nos inimigos
            if (gameObject.CompareTag("Player") && EquipmentManager.instance != null)
            {
                foreach (EquipmentData equip in EquipmentManager.instance.currentEquipment)
                {
                    if (equip == null) continue;

                    if (equip.equipSet == EquipamentSet.FuriaSanguinea) pecasFuria++;
                    if (equip.equipSet == EquipamentSet.DefesaEterna) pecasDefesaEterna++;
                }
            }

            // Aplicando os bônus de 4 peças (Ex: +20% no atributo final)
            if (pecasFuria >= 4)
                CurrentAttack = Mathf.RoundToInt(CurrentAttack * 1.20f);

            if (pecasDefesaEterna >= 4)
                CurrentDefense = Mathf.RoundToInt(CurrentDefense * 1.20f);
        }

        // UI: Avisa a tela
        if (statsUI != null)
        {
            statsUI.AtualizarTextosNaTela();
        }
    }

    // MÉTODO EXTRA PARA O DEBUFF:
    // Permite que o BattleManager mude a defesa durante a luta sem mudar o equipamento
    public void AlterarDefesaTemporariamente(int quantidade)
    {
        CurrentDefense += quantidade;
        if (CurrentDefense < 0) CurrentDefense = 0; // Defesa nunca é negativa
    }

    public void ApplyDifficultyMultiplier(float multiplier)
    {
        // Se o multiplicador for 1, não muda nada.
        // Se for 1.5, ele ganha +50% de status!
        _baseHealth = (int)(BaseHealth * multiplier);
        CurrentHealth = _baseHealth;

        _baseAttack = (int)(BaseAttack * multiplier);
        _baseDefense = (int)(BaseDefense * multiplier);
        _baseSpeed = (int)(_baseSpeed * multiplier); // Velocidade escalando também!

        // Atualiza todos os "Currents" de uma vez só!
        UpdateStats();
    }
}