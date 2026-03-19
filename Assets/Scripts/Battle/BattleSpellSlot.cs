using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // FUNDAMENTAL para detectar o mouse

public class BattleSpellSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _iconImage;
    private SpellBase _mySpell;

    public void Setup(SpellBase spell)
    {
        _mySpell = spell;
        _iconImage.sprite = spell.Icon;
    }

    public void OnClick()
    {
        BattleManager.instance.SelectSpell(_mySpell);
    }

    // --- TOOLTIP AQUI ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_mySpell != null && SpellTooltip.instance != null)
        {
            SpellTooltip.instance.ShowTooltip(_mySpell);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SpellTooltip.instance != null)
        {
            SpellTooltip.instance.HideTooltip();
        }
    }
}