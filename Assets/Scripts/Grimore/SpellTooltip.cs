using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SpellTooltip : MonoBehaviour
{
    public static SpellTooltip instance;

    [SerializeField] private TextMeshProUGUI _nameText, _levelText, _targetsText, _descText;

    void Awake()
    {
        instance = this;
        gameObject.SetActive(false); // Começa escondido
    }

    public void ShowTooltip(SpellBase spell)
    {
        gameObject.SetActive(true);
        _nameText.text = spell.SpellName;
        _levelText.text = $"Level: {spell.Level}";
        _targetsText.text = $"Alvos: {spell.MaxTargets}";
        _descText.text = spell.Description;
    }

    public void HideTooltip() => gameObject.SetActive(false);
}