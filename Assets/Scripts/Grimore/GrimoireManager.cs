using System.Collections.Generic;
using UnityEngine;

public class GrimoireManager : MonoBehaviour
{
    // --- SINGLETON ---
    public static GrimoireManager instance;

    [Header("Database - All Game Spells")]
    [Tooltip("Arraste TODOS os Assets de SpellBase criados para aqui.")]
    [SerializeField] private List<SpellBase> allSpellsInGame = new List<SpellBase>();

    [Header("Player Progress")]
    [Tooltip("Magias que o jogador já desbloqueou na torre.")]
    [SerializeField] private List<SpellBase> unlockedSpells = new List<SpellBase>();

    [Header("Current Loadout (Battle Bar)")]
    [Tooltip("As magias que aparecerão nos botões da UI de batalha.")]
    [SerializeField] private List<SpellBase> equippedSkillset = new List<SpellBase>();
    [SerializeField] private int maxSkills = 5;

    void Awake()
    {
        // Garante que só exista um Manager e que ele seja acessível globalmente
        if (instance == null)
        {
            instance = this;
            // Descomente a linha abaixo se quiser que o progresso persista entre cenas
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Encapsulamento (Getters Públicos)

    // O UIController usa isso para desenhar o Grid completo
    public List<SpellBase> GetAllSpells()
    {
        return allSpellsInGame;
    }

    // O BattleManager usa isso para saber o que colocar nos botões
    public List<SpellBase> GetEquippedSkills()
    {
        return new List<SpellBase>(equippedSkillset);
    }

    // Verifica se uma magia está na lista de desbloqueadas (usado para o visual cinza/colorido)
    public bool IsSpellUnlocked(SpellBase spell)
    {
        return unlockedSpells.Contains(spell);
    }

    #endregion

    #region Lógica de Progresso e Equipamento

    // Chamado quando você ganha uma magia nova na torre
    public void UnlockNewSpell(SpellBase newSpell)
    {
        if (newSpell != null && !unlockedSpells.Contains(newSpell))
        {
            unlockedSpells.Add(newSpell);
            Debug.Log($"<color=green>Sucesso:</color> {newSpell.SpellName} desbloqueada!");
        }
    }

    // Chamado quando você clica numa magia no Grimório para levar para a luta
    public void EquipSpell(SpellBase spell)
    {
        if (IsSpellUnlocked(spell) && equippedSkillset.Count < maxSkills && !equippedSkillset.Contains(spell))
        {
            equippedSkillset.Add(spell);
            Debug.Log($"<color=cyan>Equipada:</color> {spell.SpellName}");
        }
        else
        {
            Debug.LogWarning("Não foi possível equipar: ou o limite foi atingido ou a magia já está equipada/bloqueada.");
        }
    }

    // Chamado para remover uma magia da barra ativa
    public void UnequipSpell(SpellBase spell)
    {
        if (equippedSkillset.Contains(spell))
        {
            equippedSkillset.Remove(spell);
            Debug.Log($"<color=yellow>Removida:</color> {spell.SpellName}");
        }
    }

    #endregion
}