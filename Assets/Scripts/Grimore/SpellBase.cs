using UnityEngine;

[CreateAssetMenu(fileName = "NewSpell", menuName = "Grimoire/Spell")]
public class SpellBase : ScriptableObject
{
    [Header("Identification")]
    [SerializeField] private string spellID;
    [SerializeField] private string spellName;
    [TextArea][SerializeField] private string description;
    [SerializeField] private Sprite icon;

    [Header("Settings")]
    [SerializeField] private int level = 1;
    [SerializeField] private bool isPassive;
    [SerializeField] private int manaCost;
    [SerializeField] private int maxTargets;
    [SerializeField] private float multiplicadorDano;
    [SerializeField] private bool isHealing;

    [Header("Visuals & Logic")]
    [SerializeField] private GameObject vfxPrefab;
    [SerializeField] private string targetTag = "Enemy";

    // --- Getters ---
    public string SpellName => spellName;

    public string Description => description;
    public Sprite Icon => icon;
    public bool IsPassive => isPassive;
    public string TargetTag => targetTag;
    public GameObject VfxPrefab => vfxPrefab;
    public int ManaCost => manaCost;
    public int MaxTargets => maxTargets;

    public int Level => level;
    public bool IsHealing => isHealing;
    public float MultiplicadorDano => multiplicadorDano;

    public float GetCurrentPower() => multiplicadorDano; // Ajustei o scaling ;)
}